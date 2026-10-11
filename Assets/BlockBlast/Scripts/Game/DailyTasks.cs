using System;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>
    /// Today's tasks, picked from GameConfig.dailyTaskPool with the date as seed, so every player gets
    /// the same set and it changes at local midnight. Progress and claims are saved in PlayerPrefs.
    /// </summary>
    public sealed class DailyTasks
    {
        const string Prefix = "BlockBlast.Daily.";

        readonly GameConfig cfg;
        string day;
        int[] picks = Array.Empty<int>();

        public DailyTasks(GameConfig config) => cfg = config;

        public int Count { get { Refresh(); return picks.Length; } }
        public DailyTaskDef this[int i] { get { Refresh(); return cfg.dailyTaskPool[picks[i]]; } }
        public int Progress(int i) => Mathf.Min(PlayerPrefs.GetInt(Prefix + "P" + i, 0), this[i].target);
        public bool Done(int i) => Progress(i) >= this[i].target;
        public bool Claimed(int i) => PlayerPrefs.GetInt(Prefix + "C" + i, 0) == 1;

        /// <summary>True when a finished task waits to be claimed.</summary>
        public bool AnyClaimable
        {
            get
            {
                for (int i = 0; i < Count; i++) if (Done(i) && !Claimed(i)) return true;
                return false;
            }
        }

        /// <summary>Adds progress to today's tasks of this type; ReachCombo keeps the best value instead.</summary>
        public void Report(DailyTaskType type, int amount)
        {
            if (amount <= 0) return;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].type != type || Claimed(i)) continue;
                int now = PlayerPrefs.GetInt(Prefix + "P" + i, 0);
                PlayerPrefs.SetInt(Prefix + "P" + i, type == DailyTaskType.ReachCombo ? Mathf.Max(now, amount) : now + amount);
            }
            PlayerPrefs.Save();
        }

        /// <returns>Coins granted; 0 when the task is not done or already claimed.</returns>
        public int Claim(int i)
        {
            if (!Done(i) || Claimed(i)) return 0;
            PlayerPrefs.SetInt(Prefix + "C" + i, 1);
            CoinStore.Add(this[i].reward); // saves PlayerPrefs
            return this[i].reward;
        }

        /// <summary>Starts a new day's tasks once the date changes.</summary>
        void Refresh()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            if (day == today) return;
            day = today;

            int pool = cfg.dailyTaskPool != null ? cfg.dailyTaskPool.Length : 0;
            var random = new System.Random(int.Parse(today));
            picks = new int[Mathf.Min(cfg.dailyTaskCount, pool)];
            for (int i = 0; i < picks.Length; i++)
            {
                int pick;
                do pick = random.Next(pool); while (Array.IndexOf(picks, pick, 0, i) >= 0);
                picks[i] = pick;
            }

            if (PlayerPrefs.GetString(Prefix + "Day") == today) return;
            PlayerPrefs.SetString(Prefix + "Day", today);
            for (int i = 0; i < picks.Length; i++)
            {
                PlayerPrefs.SetInt(Prefix + "P" + i, 0);
                PlayerPrefs.SetInt(Prefix + "C" + i, 0);
            }
            PlayerPrefs.Save();
        }
    }
}
