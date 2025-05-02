using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BehaviourGraph
{

#if UNITY_EDITOR

    // Custom editor for the BehaviourTree component in the Unity Inspector
    [CustomEditor(typeof(BehaviourTree))]
    public class BehaviourTreeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draws the default inspector UI for the BehaviourTree component
            DrawDefaultInspector();

            // Adds a button to the inspector to open the Behavior Graph editor
            if (GUILayout.Button("Edit Behaviour Tree"))
                BehaviourTreeLoader.LoadGraph(target as BehaviourTree).Load();
        }
    }

    // Ensures that the BehaviourTreeLoader component is always attached to the same GameObject as BehaviourTree
    [RequireComponent(typeof(BehaviourTreeLoader))]

#endif

    public class BehaviourTree : MonoBehaviour
    {

#if UNITY_EDITOR

        // Stores the graph data for nodes in the behavior tree (used in the editor)
        [HideInInspector, SerializeReference] public List<NodeGraphData> nodeGraphData;

        // Stores the graph data for edges (connections) between nodes in the behavior tree (used in the editor)
        [HideInInspector] public List<EdgeGraphData> edgeGraphData;

#endif

        // Stores the root node for the update behavior tree
        [HideInInspector, SerializeReference] public InputNodeTreeData update;

        // Stores the root node for the late update behavior tree
        [HideInInspector, SerializeReference] public InputNodeTreeData lateUpdate;

        // Stores the root node for the fixed update behavior tree
        [HideInInspector, SerializeReference] public InputNodeTreeData fixedUpdate;

        // Stores the root node for the late fixed update behavior tree
        [HideInInspector, SerializeReference] public InputNodeTreeData lateFixedUpdate;

        // Array of all behaviors used in the tree
        [HideInInspector] public Behaviour[] behaviours;

        // Events triggered during the behavior tree lifecycle
        public event EventHandler Set;   // Triggered when the tree is "set"
        public event EventHandler Reset; // Triggered when the tree is "reset"

#if UNITY_EDITOR

        // Provides access to the BehaviourTreeLoader component
        public BehaviourTreeLoader behaviourTreeLoader
        {
            get
            {
                return GetComponent<BehaviourTreeLoader>();
            }
        }

#endif

        // Called every frame to execute the update behavior tree
        public virtual void Update()
        {
            update.Run(this);
        }

        // Called every frame after Update to execute the late update behavior tree
        public virtual void LateUpdate()
        {
            lateUpdate.Run(this);
        }

        // Called at a fixed time interval to execute the fixed update behavior tree
        public virtual void FixedUpdate()
        {
            // Trigger the Reset event
            Reset?.Invoke(this, EventArgs.Empty);

            // Run the fixed update behavior tree
            fixedUpdate.Run(this);

            // Schedule the late fixed update behavior tree
            StartCoroutine(ScheduleLateFixedUpdate());
        }

        // Executes the late fixed update behavior tree
        public virtual void LateFixedUpdate()
        {
            lateFixedUpdate.Run(this);

            // Trigger the Set event
            Set?.Invoke(this, EventArgs.Empty);
        }

        // Coroutine to schedule the late fixed update behavior tree after the next FixedUpdate
        IEnumerator ScheduleLateFixedUpdate()
        {
            yield return new WaitForFixedUpdate();

            LateFixedUpdate();
        }

        // Retrieves the index of a behavior in the behaviors array, adding it if it doesn't exist
        public int GetBehaviourIndex(Behaviour behaviour)
        {
            // Check if the behavior already exists in the array
            for (int i = 0; i < behaviours.Length; i++)
                if (behaviours[i] == behaviour) return i;

            // If not, add the behavior to the array and return its new index
            behaviours = behaviours.Append(behaviour).ToArray();

            return behaviours.Length - 1;
        }
    }

    // Represents a single behavior in the behavior tree
    public class Behaviour : MonoBehaviour
    {
        // Reference to the parent BehaviorTree
        [HideInInspector] public BehaviourTree behaviourTree;

        // Indicates whether the behavior is currently active
        [HideInInspector] public bool active;

        // Indicates whether the behavior was active in the previous frame
        [HideInInspector] public bool wasActive;

        // Called when the behavior is enabled
        public virtual void OnEnable()
        {
            // Get the parent BehaviorTree
            behaviourTree = GetComponentInParent<BehaviourTree>();

            // Subscribe to the Set and Reset events
            behaviourTree.Set += OnSet;
            behaviourTree.Reset += OnReset;
        }

        // Called when the behavior is disabled
        public virtual void OnDisable()
        {
            // Unsubscribe from the Set and Reset events
            behaviourTree.Set -= OnSet;
            behaviourTree.Reset -= OnReset;
        }

        // Evaluates whether the behavior should be active (default implementation always returns false)
        public virtual bool Evaluate()
        {
            return false;
        }

        // Executes the behavior (default implementation does nothing)
        public virtual void Execute() { }

        // Called when the Set event is triggered
        public virtual void OnSet(object sender, EventArgs e)
        {
            wasActive = active;
        }

        // Called when the Reset event is triggered
        public virtual void OnReset(object sender, EventArgs e)
        {
            active = false;
        }
    }



#if UNITY_EDITOR

#region Graph Data

[Serializable]
    public class EdgeGraphData
    {
        public string input;

        public string output;
    }

    [Serializable]
    public class NodeGraphData
    {
        public Vector2 position;

        public string GUID;

        public virtual BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            BehaviourNode behaviourNode = new BehaviourNode()
            {
                GUID = GUID
            };

            behaviourNode.Create();

            behaviourNode.SetPosition(new Rect(position, Vector3.zero));

            return behaviourNode;
        }
    }

    [Serializable]
    public class InputNodeGraphData : NodeGraphData
    {
        public string title;

        public string outputPortGUID;

        public override BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            InputBehaviourNode inputBehaviourNode = new InputBehaviourNode()
            {
                title = title,

                GUID = GUID
            };

            inputBehaviourNode.Create();

            inputBehaviourNode.SetPosition(new Rect(position, Vector3.zero));

            inputBehaviourNode.outputPort.GUID = outputPortGUID;

            return inputBehaviourNode;
        }
    }

    [Serializable]
    public class CheckNodeGraphData : NodeGraphData
    {
        public int checkIndex;

        public string inputPortGUID;

        public string passPortGUID;

        public string failPortGUID;

        public override BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            CheckBehaviourNode checkBehaviourNode = new CheckBehaviourNode()
            {
                GUID = GUID
            };

            checkBehaviourNode.Create();

            checkBehaviourNode.SetPosition(new Rect(position, Vector3.zero));

            checkBehaviourNode.checkObjectField.value = behaviourTree.behaviours[checkIndex];

            checkBehaviourNode.inputPort.GUID = inputPortGUID;

            checkBehaviourNode.passPort.GUID = passPortGUID;

            checkBehaviourNode.failPort.GUID = failPortGUID;

            return checkBehaviourNode;
        }
    }

    [Serializable]
    public class ActionNodeGraphData : NodeGraphData
    {
        public int actionIndex;

        public string inputPortGUID;

        public string outputPortGUID;

        public override BehaviourNode CreateNode(BehaviourTree behaviourTree)
        {
            ActionBehaviourNode actionBehaviourNode = new ActionBehaviourNode()
            {
                GUID = GUID
            };

            actionBehaviourNode.Create();

            actionBehaviourNode.SetPosition(new Rect(position, Vector3.zero));

            actionBehaviourNode.actionObjectField.value = behaviourTree.behaviours[actionIndex];

            actionBehaviourNode.inputPort.GUID = inputPortGUID;

            actionBehaviourNode.outputPort.GUID = outputPortGUID;

            return actionBehaviourNode;
        }
    }

    #endregion

#endif

    #region Tree Data

    [Serializable]
    public class NodeTreeData
    {
        public virtual void Reset(BehaviourTree behaviourTree) { }

        public virtual void Run(BehaviourTree behaviourTree) { }

        public virtual void Set(BehaviourTree behaviourTree) { }
    }

    [Serializable]
    public class InputNodeTreeData : NodeTreeData
    {
        [SerializeReference] public NodeTreeData pass;

        public override void Reset(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Reset(behaviourTree);
        }

        public override void Run(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Run(behaviourTree);
        }

        public override void Set(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Set(behaviourTree);
        }
    }

    [Serializable]
    public class CheckNodeTreeData : NodeTreeData
    {
        [SerializeReference] public NodeTreeData pass;

        [SerializeReference] public NodeTreeData fail;

        public int checkIndex;

        public override void Reset(BehaviourTree behaviourTree)
        {
            Behaviour thisCheck = behaviourTree.behaviours[checkIndex];

            if (thisCheck)
                thisCheck.active = false;

            if (pass != null)
                pass.Reset(behaviourTree);

            if (fail != null)
                fail.Reset(behaviourTree);
        }

        public override void Run(BehaviourTree behaviourTree)
        {
            Behaviour thisCheck = behaviourTree.behaviours[checkIndex];

            if (thisCheck)
            {
                thisCheck.active = thisCheck.Evaluate();

                if (thisCheck.active)
                {
                    if (pass != null)
                        pass.Run(behaviourTree);
                }
                else if (fail != null)
                    fail.Run(behaviourTree);
            }
        }

        public override void Set(BehaviourTree behaviourTree)
        {
            Behaviour thisCheck = behaviourTree.behaviours[checkIndex];

            if (thisCheck)
                thisCheck.wasActive = thisCheck.active;

            if (pass != null)
                pass.Set(behaviourTree);

            if (fail != null)
                fail.Set(behaviourTree);
        }
    }

    [Serializable]
    public class ActionNodeTreeData : NodeTreeData
    {
        [SerializeReference] public NodeTreeData pass;

        public int actionIndex;

        public override void Reset(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Reset(behaviourTree);
        }

        public override void Run(BehaviourTree behaviourTree)
        {
            Behaviour thisAction = behaviourTree.behaviours[actionIndex];

            if (thisAction != null)
                thisAction.Execute();

            if (pass != null)
                pass.Run(behaviourTree);
        }

        public override void Set(BehaviourTree behaviourTree)
        {
            if (pass != null)
                pass.Set(behaviourTree);
        }
    }

    #endregion
}
