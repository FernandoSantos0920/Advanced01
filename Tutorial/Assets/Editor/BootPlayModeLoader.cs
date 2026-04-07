#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class BootPlayModeLoader
{
    private const string EditorPrefKey = "BootPlayModeLoader.previousScenePath";
    private const string BootSceneSearchName = "Boot"; // scene asset name to look for

    // State used to drive a small polling state-machine after EnteredPlayMode
    private enum SequenceState { Idle, WaitingForBoot, LoadingPrevious, WaitingForPrevious, UnloadingBoot }

    private static SequenceState state = SequenceState.Idle;
    private static string bootScenePath;
    private static string previousScenePath;

    static BootPlayModeLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        try
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                // Let Unity prompt/save if there are dirty scenes. If the user cancels, stop entering play mode.
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorPrefs.DeleteKey(EditorPrefKey);
                    EditorApplication.isPlaying = false;
                    return;
                }

                var active = EditorSceneManager.GetActiveScene();
                if (!string.IsNullOrEmpty(active.path))
                {
                    EditorPrefs.SetString(EditorPrefKey, active.path);
                }
            }

            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                previousScenePath = EditorPrefs.GetString(EditorPrefKey, string.Empty);
                EditorPrefs.DeleteKey(EditorPrefKey);

                if (string.IsNullOrEmpty(previousScenePath))
                {
                    // Nothing to do, no previous scene recorded
                    return;
                }

                bootScenePath = FindBootScenePath();
                if (string.IsNullOrEmpty(bootScenePath))
                {
                    Debug.LogError("BootPlayModeLoader: Could not find a scene named 'Boot' in the project. Aborting boot sequence.");
                    return;
                }

                // Begin the load sequence. We use a polling approach via EditorApplication.update to be robust across Unity versions.
                state = SequenceState.WaitingForBoot;
                // Load Boot as single (replace whatever is in play) in the player
                var single = new LoadSceneParameters(LoadSceneMode.Single);
                EditorSceneManager.LoadSceneInPlayMode(bootScenePath, single);

                EditorApplication.update += UpdateSequence;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Cleanup();
        }
    }

    private static void UpdateSequence()
    {
        try
        {
            switch (state)
            {
                case SequenceState.WaitingForBoot:
                {
                    var bootScene = SceneManager.GetSceneByPath(bootScenePath);
                    if (bootScene.IsValid() && bootScene.isLoaded)
                    {
                        // Boot loaded, now load the previous scene additively
                        state = SequenceState.LoadingPrevious;
                        var additive = new LoadSceneParameters(LoadSceneMode.Additive);
                        EditorSceneManager.LoadSceneInPlayMode(previousScenePath, additive);
                    }
                    break;
                }

                case SequenceState.LoadingPrevious:
                {
                    state = SequenceState.WaitingForPrevious;
                    break;
                }

                case SequenceState.WaitingForPrevious:
                {
                    var prev = SceneManager.GetSceneByPath(previousScenePath);
                    if (prev.IsValid() && prev.isLoaded)
                    {
                        state = SequenceState.UnloadingBoot;
                        // Attempt to unload Boot by path. If that fails, try by scene name.
                        var unloadOp = SceneManager.UnloadSceneAsync(bootScenePath);
                        if (unloadOp == null)
                        {
                            var bootName = Path.GetFileNameWithoutExtension(bootScenePath);
                            SceneManager.UnloadSceneAsync(bootName);
                        }
                    }
                    break;
                }

                case SequenceState.UnloadingBoot:
                {
                    // Check whether boot is unloaded; if so, finish
                    var boot = SceneManager.GetSceneByPath(bootScenePath);
                    if (!boot.IsValid() || !boot.isLoaded)
                    {
                        Cleanup();
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Cleanup();
        }
    }

    private static string FindBootScenePath()
    {
        // Search for scenes named Boot. Use AssetDatabase to find scene assets without requiring Build Settings.
        try
        {
            var guids = AssetDatabase.FindAssets(BootSceneSearchName + " t:Scene");
            if (guids == null || guids.Length == 0)
                return string.Empty;

            // Prefer an exact match on filename
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (string.Equals(Path.GetFileNameWithoutExtension(path), BootSceneSearchName, StringComparison.OrdinalIgnoreCase))
                    return path;
            }

            // Fallback to first found
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return string.Empty;
        }
    }

    private static void Cleanup()
    {
        state = SequenceState.Idle;
        bootScenePath = null;
        previousScenePath = null;
        EditorApplication.update -= UpdateSequence;
    }
}
#endif

