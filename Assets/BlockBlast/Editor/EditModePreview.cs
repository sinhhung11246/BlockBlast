using System.Reflection;
using BlockBlast.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BlockBlast.EditorTools
{
    /// <summary>
    /// Builds the game in the open scene while editing, so the Scene and Game views show it without pressing Play.
    /// The preview is never saved and is removed before Play, where GameController builds the real game as usual.
    /// Menu: BlockBlast > Edit Preview.
    /// </summary>
    [InitializeOnLoad]
    public static class EditModePreview
    {
        public enum View { Off, Home, Play, Settings, DailyTasks }

        const string Menu = "BlockBlast/Edit Preview/";
        const string PrefKey = "BlockBlast.EditPreview";
        const string PreviewName = "BlockBlast (Edit Preview)";
        const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        static GameObject preview, createdCamera;
        static float builtAspect;
        static double nextResizeCheck;

        static View Current
        {
            get => (View)EditorPrefs.GetInt(PrefKey, (int)View.Home);
            set => EditorPrefs.SetInt(PrefKey, (int)value);
        }

        static EditModePreview()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode) Remove();
                else if (state == PlayModeStateChange.EnteredEditMode) Rebuild();
            };
            AssemblyReloadEvents.beforeAssemblyReload += Remove;
            EditorSceneManager.sceneOpened += (_, _) => Rebuild();
            EditorSceneManager.newSceneCreated += (_, _, _) => Rebuild();
            EditorApplication.update += RefitOnResize;
            EditorApplication.delayCall += Rebuild;
        }

        [MenuItem(Menu + "Home", priority = 1)] static void ShowHome() => Show(View.Home);
        [MenuItem(Menu + "Play Screen", priority = 2)] static void ShowPlay() => Show(View.Play);
        [MenuItem(Menu + "Settings Popup", priority = 3)] static void ShowSettings() => Show(View.Settings);
        [MenuItem(Menu + "Daily Tasks Popup", priority = 4)] static void ShowTasks() => Show(View.DailyTasks);
        [MenuItem(Menu + "Off", priority = 20)] static void ShowOff() => Show(View.Off);

        [MenuItem(Menu + "Home", true)] static bool CanShow() => !EditorApplication.isPlayingOrWillChangePlaymode;
        [MenuItem(Menu + "Play Screen", true)] static bool CanShowPlay() => CanShow();
        [MenuItem(Menu + "Settings Popup", true)] static bool CanShowSettings() => CanShow();
        [MenuItem(Menu + "Daily Tasks Popup", true)] static bool CanShowTasks() => CanShow();

        /// <summary>Switches the preview to <paramref name="view"/> and rebuilds it.</summary>
        public static void Show(View view)
        {
            Current = view;
            Rebuild();
        }

        static void Rebuild()
        {
            Remove();
            if (Current == View.Off || EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (Object.FindAnyObjectByType<GameController>() != null) return; // the scene has its own GameController

            bool hadCamera = Camera.main != null;
            preview = new GameObject(PreviewName);
            var game = preview.AddComponent<GameController>();
            Invoke(game, "Awake"); // edit mode does not call Awake on its own
            switch (Current)
            {
                case View.Play: Invoke(game, "Restart"); break;
                case View.Settings: Invoke(game, "Restart"); Invoke(game, "OpenSettings"); break;
                case View.DailyTasks: Invoke(game, "OpenTasks"); break;
            }

            if (!hadCamera && Camera.main != null) createdCamera = Camera.main.gameObject; // made by Awake; keep it out of the scene file
            foreach (Transform t in preview.GetComponentsInChildren<Transform>(true)) t.gameObject.hideFlags = HideFlags.DontSave;
            if (createdCamera != null) createdCamera.hideFlags = HideFlags.DontSave;
            builtAspect = Camera.main != null ? Camera.main.aspect : 0f;
            SceneView.RepaintAll();
        }

        static void Remove()
        {
            if (preview == null) preview = GameObject.Find(PreviewName);
            if (preview != null) Object.DestroyImmediate(preview);
            if (createdCamera != null) Object.DestroyImmediate(createdCamera);
            preview = createdCamera = null;
        }

        /// <summary>Backgrounds are fitted when built, so rebuild once the Game view changes shape.</summary>
        static void RefitOnResize()
        {
            if (EditorApplication.timeSinceStartup < nextResizeCheck) return;
            nextResizeCheck = EditorApplication.timeSinceStartup + 0.5;
            if (preview == null || EditorApplication.isPlayingOrWillChangePlaymode || Camera.main == null) return;
            if (!Mathf.Approximately(Camera.main.aspect, builtAspect)) Rebuild();
        }

        static void Invoke(GameController game, string method) =>
            typeof(GameController).GetMethod(method, Private).Invoke(game, null);
    }
}
