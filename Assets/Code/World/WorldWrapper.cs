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

        public float chunk_width { get; private set; }

        public void Initialise(float total_width, byte divisions)
        {
            Debug.Assert(1 <= divisions);

            float chunk_width = total_width / divisions;
            float half = chunk_width / 2f;
            Debug.Log($"Chunk size: {chunk_width}");

            for(int d = 0; d < divisions; d++)
            {
                WorldChunk chunk = new GameObject($"Chunk [{d}]").AddComponent<WorldChunk>();

                chunk.transform.SetParent(transform, false);
                chunk.transform.position = new Vector3(d * chunk_width - WorldGen.offset.x + half, 0f, 0f);

                // Could make the chunk data static, but in order to keep them reusable I'll leave as is.
                chunk.Initialise(new WorldChunk.Data()
                {
                    size = chunk_width,
                    half_size = half,
                });
            }
        }
    }
}
