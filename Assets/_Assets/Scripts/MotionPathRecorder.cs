using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ScriptableObject to store persistent path data
public class MotionPathData : ScriptableObject
{
    public List<Vector3> positions = new List<Vector3>();
}

[ExecuteInEditMode]
public class MotionPathRecorder : MonoBehaviour
{
    public Transform target;

    [Header("Recording Settings")]
    [Tooltip("Time interval between position recordings")]
    public float recordInterval = 0.1f;
    [SerializeField] private bool isRecording;

    [Header("Path Visualization")]
    public bool showPath = true;
    public Color pathColor = Color.green;
    [Range(0.1f, 2f)] public float pointSize = 0.5f;

    [Header("Data Persistence")]
    public MotionPathData pathData;

    private List<Vector3> runtimePositions = new List<Vector3>();
    private float timer;

    private void Start()
    {
        target = Player.CharacterInstance.playerBehaviourTree.modelTransform;
        if (Application.isPlaying && target != null)
        {
            StartRecording();
        }
    }
    void OnEnable()
    {
#if UNITY_EDITOR
        // Auto-create path data if none exists
        if (pathData == null)
        {
            CreatePathDataAsset();
        }
#endif

        if (Application.isPlaying && target != null)
        {
            StartRecording();
        }
    }
    private void OnApplicationQuit()
    {
        if (isRecording)
        {
            StopRecording();
        }
        // Save path data when the application quits
        SavePathData();
    }
    
    void Update()
    {
        if (!isRecording || target == null) return;

        timer += Time.deltaTime;
        if (timer >= recordInterval)
        {
            runtimePositions.Add(target.position);
            timer = 0;
        }
    }

    public void StartRecording()
    {
        runtimePositions.Clear();
        isRecording = true;
        timer = 0;
        runtimePositions.Add(target.position);
    }

    public void StopRecording()
    {
        isRecording = false;

        // Save to persistent data when stopping
        SavePathData();
    }

    void OnDrawGizmos()
    {
        if (!showPath) return;

        List<Vector3> positionsToDraw = Application.isPlaying ?
            runtimePositions :
            (pathData != null ? pathData.positions : new List<Vector3>());

        if (positionsToDraw.Count < 2) return;

        Gizmos.color = pathColor;

        // Draw path lines
        for (int i = 0; i < positionsToDraw.Count - 1; i++)
        {
            Gizmos.DrawLine(positionsToDraw[i], positionsToDraw[i + 1]);
        }

        // Draw points
        foreach (Vector3 pos in positionsToDraw)
        {
            Gizmos.DrawSphere(pos, pointSize);
        }
    }

    public void ClearPath()
    {
        runtimePositions.Clear();
        if (pathData != null) pathData.positions.Clear();
    }

    void SavePathData()
    {
        if (pathData == null) return;

        pathData.positions.Clear();
        pathData.positions.AddRange(runtimePositions);

#if UNITY_EDITOR
        EditorUtility.SetDirty(pathData);
#endif
    }

#if UNITY_EDITOR
    void CreatePathDataAsset()
    {
        pathData = ScriptableObject.CreateInstance<MotionPathData>();
        string path = "Assets/MotionPath/MotionPathData.asset";
        AssetDatabase.CreateAsset(pathData, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Created path data asset at: " + path);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MotionPathRecorder))]
public class MotionPathRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MotionPathRecorder recorder = (MotionPathRecorder)target;

        GUILayout.Space(10);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Start Recording"))
            {
                if (recorder.target != null)
                {
                    recorder.StartRecording();
                }
                else
                {
                    Debug.LogWarning("Assign a target before recording!");
                }
            }

            if (GUILayout.Button("Stop Recording"))
            {
                recorder.StopRecording();
            }
        }

        if (GUILayout.Button("Clear Path"))
        {
            recorder.ClearPath();
        }

        GUILayout.Space(5);
        EditorGUILayout.HelpBox("Path will automatically record during play mode when target is assigned.", MessageType.Info);
        EditorGUILayout.HelpBox("Path data is saved in MotionPathData.asset for editor visualization.", MessageType.Info);
    }
}
#endif