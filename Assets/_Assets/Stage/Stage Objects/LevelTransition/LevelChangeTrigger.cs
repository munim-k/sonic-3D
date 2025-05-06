#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelChangeTrigger))]
public class LevelChangeVolumeEditor : Editor
{
    private string[] sceneNames; // Array to store scene names
    private int selectedSceneIndex; // Index of the selected scene

    private void OnEnable()
    {
        // Retrieve all scenes in the build settings
        var scenes = EditorBuildSettings.scenes;
        sceneNames = new string[scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path); // Get scene name
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelChangeTrigger levelChangeVolume = (LevelChangeTrigger)target;

        // Dropdown for selecting a scene
        selectedSceneIndex = EditorGUILayout.Popup("Select Level", selectedSceneIndex, sceneNames);

        // Button to set the selected scene
        if (GUILayout.Button("Set Level"))
        {
            levelChangeVolume.SetLevel(sceneNames[selectedSceneIndex]);
        }
    }
}

public class LevelChangeTrigger : Trigger
{
    [SerializeField] private string levelName; // Selected level name

    public void SetLevel(string level)
    {
        levelName = level;
    }

    public void ChangeLevel()
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("Level name is not set!");
        }
    }
}

#endif
