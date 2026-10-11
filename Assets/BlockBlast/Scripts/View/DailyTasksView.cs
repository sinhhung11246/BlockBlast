using System;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Daily Tasks popup: one TaskRow per task plus the time left until the tasks reset.</summary>
    public sealed class DailyTasksView : MonoBehaviour
    {
        public enum Action { None, Close, Claim }

        [SerializeField] SpriteRenderer panel, close;
        [SerializeField] TextMesh timer;
        [SerializeField] TaskRowView[] rows;

        public bool IsOpen => gameObject.activeSelf;
        public int RowCount => rows.Length;

        public void Show(bool visible) => gameObject.SetActive(visible);

        public void SetRow(int i, TaskRowView.State state) => rows[i].Set(state);

        /// <summary>Hides rows past the day's task count.</summary>
        public void HideRowsFrom(int count)
        {
            for (int i = count; i < rows.Length; i++) rows[i].Hide();
        }

        /// <param name="row">The row whose Claim button was tapped; only set for <see cref="Action.Claim"/>.</param>
        public Action Tap(Vector2 world, out int row)
        {
            row = -1;
            if (SpriteFactory.Hits(close, world) || !SpriteFactory.Hits(panel, world)) return Action.Close;
            for (int i = 0; i < rows.Length; i++)
            {
                if (!rows[i].HitsClaim(world)) continue;
                row = i;
                return Action.Claim;
            }
            return Action.None;
        }

        void Update()
        {
            TimeSpan left = DateTime.Today.AddDays(1) - DateTime.Now;
            timer.text = $"{(int)left.TotalHours:00}:{left.Minutes:00}:{left.Seconds:00}";
        }
    }
}
