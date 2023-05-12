using System;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    public class WorldWrapper : MonoBehaviour
    {
        public static WorldWrapper I 
        { 
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<WorldWrapper>();

                    if (instance == null)
                    {
                        instance = new GameObject(nameof(WorldWrapper)).AddComponent<WorldWrapper>();
                    }
                }

                return instance;
            }
        }
        private static WorldWrapper instance;

        public void Initialise(float total_width, byte divisions)
        {
            Debug.Assert(1 <= divisions);

            float chunk_size = total_width / divisions;
            Debug.Log($"Chunk size: {chunk_size}");

            WorldGen.I.Subscribe(OnPropGenerated);
        }

        private void OnPropGenerated(WorldGen.PropData prop)
        {
            Debug.Log($"Prop generated @ {prop.obj.transform.position}");
        }
    }
}
