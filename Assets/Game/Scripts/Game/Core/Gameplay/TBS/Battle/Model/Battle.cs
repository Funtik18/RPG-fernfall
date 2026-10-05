using R3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class Battle
    {
        public event Action OnStarted;
        public event Action OnFinished;
        public event Action OnTurned;
        public event Action OnChanged;
        public event Action< BattleRound > OnRoundStarted;
        public event Action< BattleRound > OnRoundChanged;
        public event Action< BattleRound > OnRoundFinished;

        public bool IsFinished { get; private set; }
        public BattleResult Result { get; private set; }
        public List< BattleTeam > Teams { get; }
        public List< UnitController > Units { get; }
        public BattleRound CurrentRound { get; private set; }
        public ReactiveProperty< int > RoundNumber { get; private set; } = new( 0 );

        private readonly BattleRoundFactory _battleRoundFactory;
        private readonly BattleOutcomeEvaluator _outcomeEvaluator;
        
        public Battle(
            List< BattleTeam > teams,
            BattleRoundFactory battleRoundFactory,
            BattleOutcomeEvaluator outcomeEvaluator
            )
        {
            Teams = teams ?? throw new ArgumentNullException( nameof(teams) );
            _battleRoundFactory = battleRoundFactory ?? throw new ArgumentNullException( nameof(battleRoundFactory) );
            _outcomeEvaluator = outcomeEvaluator ?? throw new ArgumentNullException( nameof(outcomeEvaluator) );

            Units = new();
            foreach ( var team in teams )
            {
                Units.AddRange( team.Units );
            }
        }

        public void Start()
        {
            if ( IsFinished ) return;

            OnStarted?.Invoke();
            StartNextRound();
        }

        public void EndCurrentTurn()
        {
            if ( IsFinished ) return;
            if ( CurrentRound == null ) return;
            if ( CurrentRound.CurrentTurn == null ) return;

            CurrentRound.CurrentTurn.Finish();

            OnChanged?.Invoke();
            OnTurned?.Invoke();
        }

        public BattleTeam GetTeam( UnitController unit ) => Teams.Find( t => t.Units.Contains( unit ) );
        
        private void StartNextRound()
        {
            RoundNumber.Value++;

            for ( int i = 0; i < Units.Count; i++ )
            {
                Units[ i ].Restore();
            }
            
            CurrentRound = _battleRoundFactory.Create( this, RoundNumber.Value, Units );
            CurrentRound.OnChanged += RoundChangedHandler;
            CurrentRound.OnFinished += RoundFinishedHandler;
            CurrentRound.OnUnitCompleted += RoundUnitCompletedHandler;
            OnRoundStarted?.Invoke( CurrentRound );

            if ( TryFinishByOutcome() )
            {
                return;
            }

            CurrentRound.Start();
            
            OnChanged?.Invoke();
        }

        private void RoundChangedHandler( BattleRound round )
        {
            if ( IsFinished )
            {
                return;
            }

            OnRoundChanged?.Invoke( round );
            OnChanged?.Invoke();
        }

        private void RoundUnitCompletedHandler( UnitController unit )
        {
            TryFinishByOutcome();
        }

        private void RoundFinishedHandler( BattleRound round )
        {
            round.OnChanged -= RoundChangedHandler;
            round.OnFinished -= RoundFinishedHandler;
            round.OnUnitCompleted -= RoundUnitCompletedHandler;
            OnRoundFinished?.Invoke( round );

            if ( IsFinished )
            {
                return;
            }

            if ( TryFinishByOutcome() )
            {
                return;
            }

            StartNextRound();
        }

        public bool TryFinishByOutcome()
        {
            if ( IsFinished )
            {
                return true;
            }

            if ( !_outcomeEvaluator.TryEvaluate( this, out var result ) )
            {
                return false;
            }

            Finish( result );
            return true;
        }

        private void Finish( BattleResult result )
        {
            if ( IsFinished )
                return;

            IsFinished = true;
            Result = result;

            OnFinished?.Invoke();
        }
    }
}
