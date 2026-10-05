using System;
using System.Collections.Generic;

namespace Fire.UI.Scroll
{
    public enum ScrollStatus
    {
        Villein,
        Cottar,
        Famulus,
        Dependent,
        Freeholder
    }

    public enum ScrollCondition
    {
        Intact,
        Burned,
        Rewritten
    }

    [Serializable]
    public class ScrollRecordData
    {
        public string Name;
        public string Manor;
        public ScrollStatus Status;
        public string DependentOn = "";
        public string Holding;
        public string Rent;
        public string WeekWorkDays;
        public List<string> Debts = new List<string>();
        public List<string> Amercements = new List<string>();
        public ScrollCondition Condition;
    }

    // Placeholder data standing in for E:\FireObsidian\Fire\Docs\Narrative\Scroll_Record_Format.md section 4,
    // adapted so per-status line counts match the wireframe test target (Freeholder ~3, Villein 7+).
    public static class ScrollRecordSeedData
    {
        public static List<ScrollRecordData> GetSample()
        {
            return new List<ScrollRecordData>
            {
                new ScrollRecordData
                {
                    Name = "Джон, сын Уильяма",
                    Manor = "Фоббинг",
                    Status = ScrollStatus.Villein,
                    Holding = "Держит небольшой надел земли.",
                    Rent = "Платит оброк четыре шиллинга в год.",
                    WeekWorkDays = "Обязан работать на хозяина три дня в неделю.",
                    Debts = new List<string>
                    {
                        "Отдаёт часть урожая хозяину.",
                        "Обязан молоть зерно только на господской мельнице.",
                        "Дополнительно выходит на барщину в страду (жатва)."
                    },
                    Amercements = new List<string>
                    {
                        "Оштрафован на шесть пенсов за то, что не вышел на общую работу в жатву."
                    },
                    Condition = ScrollCondition.Intact
                },
                new ScrollRecordData
                {
                    Name = "Роберт Кок",
                    Manor = "Фоббинг",
                    Status = ScrollStatus.Cottar,
                    Holding = "Надел меньше среднего.",
                    Rent = "Платит оброк два шиллинга в год.",
                    WeekWorkDays = "Обязан работать на хозяина два дня в неделю.",
                    Debts = new List<string>(),
                    Amercements = new List<string>
                    {
                        "Оштрафован за потраву чужого луга.",
                        "Оштрафован за то, что молол зерно не на господской мельнице.",
                        "Оштрафован за брань перед управляющим."
                    },
                    Condition = ScrollCondition.Intact
                },
                new ScrollRecordData
                {
                    Name = "Джон с Опушки",
                    Manor = "Фоббинг",
                    Status = ScrollStatus.Cottar,
                    Holding = "Не держит земли.",
                    Rent = "Оброка не платит — работает за пропитание.",
                    WeekWorkDays = "Работает на хозяина один день в неделю круглый год.",
                    Debts = new List<string>(),
                    Amercements = new List<string>(),
                    Condition = ScrollCondition.Intact
                },
                new ScrollRecordData
                {
                    Name = "Агнес, дочь Томаса",
                    Manor = "Фоббинг",
                    Status = ScrollStatus.Dependent,
                    DependentOn = "Томас Бакер",
                    Holding = "Живёт в доме отца, своей земли не держит.",
                    Rent = "Оброка не платит.",
                    WeekWorkDays = "Барщины не несёт, работает в хозяйстве отца.",
                    Debts = new List<string>
                    {
                        "Отец обязан заплатить хозяину пошлину, если она выйдет замуж."
                    },
                    Amercements = new List<string>
                    {
                        "Отец оштрафован за то, что она забеременела вне брака."
                    },
                    Condition = ScrollCondition.Intact
                },
                new ScrollRecordData
                {
                    Name = "Уильям Смит",
                    Manor = "Фоббинг",
                    Status = ScrollStatus.Freeholder,
                    Holding = "Держит надел.",
                    Rent = "Платит оброк десять шиллингов в год.",
                    WeekWorkDays = "Обязательной работы на хозяина не несёт.",
                    Debts = new List<string>(),
                    Amercements = new List<string>(),
                    Condition = ScrollCondition.Intact
                }
            };
        }
    }
}
