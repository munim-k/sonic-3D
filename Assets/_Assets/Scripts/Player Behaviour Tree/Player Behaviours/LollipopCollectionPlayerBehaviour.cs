using System;
using RagdollEngine;
using UnityEngine;
namespace RagdollEngine
{
    public class LollipopCollectionPlayerBehaviour : PlayerBehaviour
    {
        private int lollipops = 0;
        public Action<int> onLollipopChange;


        public void Awake()
        {
            //Get lollipops amount from sharedPrefs
            lollipops = PlayerPrefs.GetInt("Lollipops", 0);
            onLollipopChange?.Invoke(lollipops);
        }
        public override void Execute()
        { 
            //Check if any overlapping volumes are lollipops
            foreach(Volume thisVolume in volumes)
            {
                LollipopCollectible lollipop = thisVolume.GetComponent<LollipopCollectible>();
                if (lollipop != null)
                {
                    lollipops++;
                    onLollipopChange?.Invoke(lollipops);
                    lollipop.Consume();
                }

            }

        }
        public int GetLollipops()
        {
            return lollipops;
        }
        public void SaveLollipops()
        {
            //Save lollipops amount to sharedPrefs
            PlayerPrefs.SetInt("Lollipops", lollipops);
            PlayerPrefs.Save();
        }
    }
}
