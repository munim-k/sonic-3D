using RagdollEngine;
using UnityEngine;

namespace RagdollEngine
{

    public class LevelChangePlayerBehaviour : PlayerBehaviour
    {
        public override bool Evaluate()
        {
            foreach(Volume vol in volumes)
            {
                print("Intersecting volume: "+ vol.name);
                LevelChangeTrigger levelChange = vol.GetComponent<LevelChangeTrigger>();
                if (levelChange != null)
                {
                    print("Found level change");
                    levelChange.ChangeLevel();
                    return true; // Return true if a level change volume is found
                }
            }
            return false;
        }

    }

}