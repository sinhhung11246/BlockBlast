using System;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Daily Tasks popup: one row per task with reward, description, progress bar and a Claim button
    /// (grey until done, a check once claimed), plus the time left until the tasks reset.
    /// </summary>
    public sealed class DailyTasksView : MonoBehaviour
    {
        public enum Action { None, Close, Claim }

        /// <summary>What a row shows; filled by the controller from DailyTasks.</summary>
        public struct RowState
        {
            public string text;
            public int reward, progress, target;
            public bool claimed;
            public bool Done => progress >= target;
        }

        sealed class Row
        {
            public GameObject root;
            public TextMesh text, reward, buttonText;
            public SpriteRenderer fill, button, done;
        }

        GameConfig cfg;
        SpriteRenderer panel, close;
        TextMesh timer;
        Row[] rows;

        public bool IsOpen => gameObject.activeSelf;

        public void Build(GameConfig config, Camera cam)
        {
            cfg = config;
            SpriteFactory.Dim("Dim", transform, cfg.settingsDim, 50, cam);
            panel = SpriteFactory.Create("Panel", transform, cfg.tasksPanel, Vector2.zero, cfg.tasksPanelWidth, 51);
            SpriteFactory.Create("Icon", transform, cfg.tasksIcon, cfg.tasksIconPlace.pos, cfg.tasksIconPlace.size, 52);
            SpriteFactory.Text("Title", transform, cfg.font, "Daily Tasks", cfg.tasksTitle.pos, cfg.tasksTitle.size, cfg.popupTextColor, 52);
            close = SpriteFactory.Create("Close", transform, cfg.closeButton, cfg.tasksClose.pos, cfg.tasksClose.size, 52);
            SpriteFactory.Create("Clock", transform, cfg.clockIcon, cfg.tasksClock.pos, cfg.tasksClock.size, 52);
            timer = SpriteFactory.Text("Timer", transform, cfg.font, "", cfg.tasksTimer.pos, cfg.tasksTimer.size, cfg.timerColor, 52);

            rows = new Row[cfg.dailyTaskCount];
            for (int i = 0; i < rows.Length; i++) rows[i] = BuildRow(i);
            gameObject.SetActive(false);
        }

        Row BuildRow(int i)
        {
            var root = new GameObject("Row" + i).transform;
            root.SetParent(transform, false);
            root.localPosition = cfg.firstRowPos + Vector2.down * (cfg.rowPitch * i);

            var row = new Row { root = root.gameObject };
            SpriteFactory.Create("Background", root, cfg.taskRow, Vector2.zero, cfg.rowWidth, 52);
            SpriteFactory.Create("Tile", root, cfg.taskRewardTile, cfg.rowTile.pos, cfg.rowTile.size, 53);
            SpriteFactory.Create("Coin", root, cfg.coin, cfg.rowCoin.pos, cfg.rowCoin.size, 54);
            row.reward = SpriteFactory.Text("Reward", root, cfg.font, "", cfg.rowReward.pos, cfg.rowReward.size, cfg.taskTextColor, 55);
            row.text = SpriteFactory.Text("Task", root, cfg.font, "", cfg.rowText.pos, cfg.rowText.size, cfg.taskTextColor, 53);
            SpriteFactory.Create("Bar", root, cfg.taskBar, cfg.rowBar.pos, cfg.rowBar.size, 53);
            row.fill = SpriteFactory.Create("Fill", root, cfg.taskBarFill, cfg.rowBar.pos, BarWidth, 54);
            row.button = SpriteFactory.Create("Claim", root, cfg.claimButton, cfg.rowButton.pos, cfg.rowButton.size, 53);
            row.buttonText = SpriteFactory.Text("ClaimText", root, cfg.font, "Claim", cfg.rowButtonText.pos, cfg.rowButtonText.size, cfg.popupTextColor, 54);
            row.done = SpriteFactory.Create("Done", root, cfg.taskDone, cfg.rowDone.pos, cfg.rowDone.size, 53);
            return row;
        }

        float BarWidth => cfg.rowBar.size - 2f * cfg.rowBarFillInset;

        public void Show(bool visible) => gameObject.SetActive(visible);

        public void SetRow(int i, RowState state)
        {
            if (i >= rows.Length) return;
            Row row = rows[i];
            row.root.SetActive(true);
            row.text.text = state.text;
            row.reward.text = state.reward.ToString();

            // Fill grows from the bar's left edge.
            float p = state.target > 0 ? Mathf.Clamp01((float)state.progress / state.target) : 1f;
            row.fill.enabled = p > 0f;
            SpriteFactory.SetSprite(row.fill, cfg.taskBarFill, BarWidth);
            row.fill.transform.localScale = Vector3.Scale(row.fill.transform.localScale, new Vector3(p, 1f, 1f));
            float left = cfg.rowBar.pos.x - BarWidth * 0.5f;
            row.fill.transform.localPosition = new Vector2(left + BarWidth * p * 0.5f, cfg.rowBar.pos.y);

            row.done.enabled = state.claimed;
            row.button.enabled = !state.claimed;
            row.buttonText.gameObject.SetActive(!state.claimed);
            SpriteFactory.SetSprite(row.button, state.Done ? cfg.claimButton : cfg.claimButtonOff, cfg.rowButton.size);
        }

        /// <summary>Hides rows past the day's task count.</summary>
        public void HideRowsFrom(int count)
        {
            for (int i = count; i < rows.Length; i++) rows[i].root.SetActive(false);
        }

        /// <param name="row">The row whose Claim button was tapped; only set for <see cref="Action.Claim"/>.</param>
        public Action Tap(Vector2 world, out int row)
        {
            row = -1;
            if (SpriteFactory.Hits(close, world) || !SpriteFactory.Hits(panel, world)) return Action.Close;
            for (int i = 0; i < rows.Length; i++)
            {
                if (!rows[i].root.activeSelf || !rows[i].button.enabled || !SpriteFactory.Hits(rows[i].button, world)) continue;
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
