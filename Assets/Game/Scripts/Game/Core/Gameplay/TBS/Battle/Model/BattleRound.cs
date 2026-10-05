using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleRound
    {
        public event Action< BattleRound > OnChanged;
        public event Action< BattleRound > OnFinished;

        public event Action< BattleTurn > OnTurnStarted;
        public event Action< BattleTurn > OnTurnFinished;
        public event Action< UnitController > OnUnitCompleted;

        public int Number { get; }

        public IReadOnlyList< BattleTurn > Turns => _turns;
        private readonly List< BattleTurn > _turns;

        public BattleTurn CurrentTurn { get; private set; }

        public bool IsFinished { get; private set; }

        private int _turnIndex = -1;

        public BattleRound( int number, List< BattleTurn > turns )
        {
            Number = number;
            _turns = turns ?? throw new ArgumentNullException( nameof(turns) );
        }

        public void Start()
        {
            StartNextTurn();
        }

        private void StartNextTurn()
        {
            _turnIndex++;

            if ( _turnIndex >= _turns.Count )
            {
                Finish();
                return;
            }

            CurrentTurn = _turns[ _turnIndex ];

            CurrentTurn.OnChanged += TurnChangedHandler;
            CurrentTurn.OnFinished += TurnFinishedHandler;
            CurrentTurn.OnUnitCompleted += TurnUnitCompletedHandler;

            CurrentTurn.Start();

            OnTurnStarted?.Invoke( CurrentTurn );
            OnChanged?.Invoke( this );
        }

        private void TurnChangedHandler( BattleTurn turn )
        {
            OnChanged?.Invoke( this );
        }

        private void TurnUnitCompletedHandler( UnitController unit )
        {
            OnUnitCompleted?.Invoke( unit );
        }

        private void TurnFinishedHandler( BattleTurn turn )
        {
            turn.OnChanged -= TurnChangedHandler;
            turn.OnFinished -= TurnFinishedHandler;
            turn.OnUnitCompleted -= TurnUnitCompletedHandler;

            OnTurnFinished?.Invoke( turn );

            StartNextTurn();
        }

        private void Finish()
        {
            if ( IsFinished ) return;
            IsFinished = true;

            OnFinished?.Invoke( this );
        }
    }
}
