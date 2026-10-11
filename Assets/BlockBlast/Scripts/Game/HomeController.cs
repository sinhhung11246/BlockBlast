using BlockBlast.View;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>Home scene: mode buttons start the Game scene; the gear opens Settings and the notes icon Daily Tasks.</summary>
    public sealed class HomeController : ScreenController
    {
        [SerializeField] HomeView homeView;
        [SerializeField] DailyTasksView tasksView;

        DailyTasks tasks;

        protected override void Awake()
        {
            base.Awake();
            tasks = new DailyTasks(config);
            homeView.SetCoins(CoinStore.Coins);
        }

        protected override void Tick(Vector2 pointer)
        {
            if (!PointerInput.Pressed) return;
            if (tasksView.IsOpen) TapTasks(pointer);
            else if (homeView.HitsPlay(pointer)) Load(GameScene);
            else if (homeView.HitsSettings(pointer)) OpenSettings();
            else if (homeView.HitsTasks(pointer)) OpenTasks();
        }

        protected override void Replay() => Load(GameScene);

        protected override void GoHome() { } // already home; the popup has closed

        void OpenTasks()
        {
            int count = Mathf.Min(tasks.Count, tasksView.RowCount);
            for (int i = 0; i < count; i++)
                tasksView.SetRow(i, new TaskRowView.State
                {
                    text = tasks[i].Description,
                    reward = tasks[i].reward,
                    progress = tasks.Progress(i),
                    target = tasks[i].target,
                    claimed = tasks.Claimed(i),
                });
            tasksView.HideRowsFrom(count);
            tasksView.Show(true);
        }

        void TapTasks(Vector2 pointer)
        {
            switch (tasksView.Tap(pointer, out int row))
            {
                case DailyTasksView.Action.Close:
                    tasksView.Show(false);
                    return;
                case DailyTasksView.Action.Claim:
                    if (tasks.Claim(row) == 0) return;
                    homeView.SetCoins(CoinStore.Coins);
                    OpenTasks(); // refresh rows
                    return;
            }
        }
    }
}
