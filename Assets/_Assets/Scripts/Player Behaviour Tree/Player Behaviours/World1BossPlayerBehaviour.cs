using UnityEngine;
using UnityEngine.InputSystem;

namespace RagdollEngine
{
    public class World1BossPlayerBehaviour : PlayerBehaviour
    {
        
       
        public override void Execute()
        {
            foreach (var volume in volumes)
            {
                if (volume is World1BossPillarTrigger pillar)
                {
                    pillar.Activate();
                }
            }
        }
    }
}
