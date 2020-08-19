#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoSaveEditor : EditorWindow
{
    private static DateTime nextSaveTime;

    [HideInInspector]
    public static bool enableSave = false;

    public static bool enableSaveAfterPlay = false;
    public static bool enableSaveAfterTime = false;
    public static int mins = 5;

    [MenuItem("Editor/AutoSave")]
    public static void AutoSave()
    {
        GetWindow<AutoSaveEditor>("Auto Save Settings");
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);
        enableSave = EditorGUILayout.Toggle("Enable Auto Save", enableSave);
        if (enableSave)
        {
            enableSaveAfterPlay = EditorGUILayout.Toggle("Enable Save After Play", enableSaveAfterPlay);
            enableSaveAfterTime = EditorGUILayout.Toggle("Enable Save After Minutes", enableSaveAfterTime);
            if (enableSaveAfterTime)
            {
                mins = EditorGUILayout.IntField("Minutes", mins);
            }
        }
    }

    // Static constructor that gets called when unity fires up.
    static AutoSaveEditor()
    {
        if (enableSave)
        {
            EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
            {
                if (enableSaveAfterPlay)
                {
                    // If we're about to run the scene...
                    if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying) return;

                    // Save the scene and the assets.
                    Debug.Log("Auto Saving... ");
                    EditorSceneManager.SaveOpenScenes();
                    AssetDatabase.SaveAssets();
                }
            };

            if (enableSaveAfterTime)
            {
                // Also, every five minutes.
                nextSaveTime = DateTime.Now.AddMinutes(mins);
                EditorApplication.update += Update;
            }
        }
    }

    private static void Update()
    {
        if (enableSave)
        {
            if (enableSaveAfterTime)
            {
                if (nextSaveTime > DateTime.Now) return;

                nextSaveTime = nextSaveTime.AddMinutes(mins);

                Debug.Log("Auto Saving... ");
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}

#endif