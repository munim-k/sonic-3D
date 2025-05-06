using System;
using RagdollEngine;
using UnityEngine;
namespace RagdollEngine
{
    public class LollipopCollectionPlayerBehaviour : PlayerBehaviour
    {
        private int lollipops = 0;
        public Action<int> onLollipopCollection;
        public override void Execute()
        { 
            //Check if any overlapping volumes are lollipops
            foreach(Volume thisVolume in volumes)
            {
                LollipopCollectible lollipop = thisVolume.GetComponent<LollipopCollectible>();
                if (lollipop != null)
                {
                    lollipops++;
                    onLollipopCollection?.Invoke(lollipops);
                    lollipop.Consume();
                }

            }

        }
    }
}
