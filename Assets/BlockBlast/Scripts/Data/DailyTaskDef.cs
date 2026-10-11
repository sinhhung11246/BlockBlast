using System;

namespace BlockBlast.Data
{
    public enum DailyTaskType { PlayGames, PlacePieces, ClearLines, ScorePoints, ReachCombo }

    /// <summary>One daily task: reach <see cref="target"/> of <see cref="type"/> today to earn <see cref="reward"/> coins.</summary>
    [Serializable]
    public struct DailyTaskDef
    {
        public DailyTaskType type;
        public int target;
        public int reward;

        public string Description => type switch
        {
            DailyTaskType.PlayGames => $"Play {target} games",
            DailyTaskType.PlacePieces => $"Place {target} blocks",
            DailyTaskType.ClearLines => $"Clear {target} lines",
            DailyTaskType.ScorePoints => $"Score {target} points",
            DailyTaskType.ReachCombo => $"Reach combo {target}",
            _ => type.ToString(),
        };
    }
}
