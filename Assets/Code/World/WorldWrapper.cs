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

        public Data m_data { get; private set; }

        public struct Data
        {
            public float total_width;
            public byte divisions;
            public float chunk_plane_depth;
            public GameObject chunk_plane_prefab;
        }

        public void Initialise(Data data)
        {
            m_data = data;
            Debug.Assert(1 <= m_data.divisions);

            float chunk_width = m_data.total_width / m_data.divisions;
            float half = chunk_width / 2f;
            Debug.Log($"Chunk size: {chunk_width}");

            for(int d = 0; d < m_data.divisions; d++)
            {
                WorldChunk chunk = new GameObject($"Chunk [{d}]").AddComponent<WorldChunk>();

                chunk.transform.SetParent(transform, false);
                chunk.transform.position = new Vector3(d * chunk_width - WorldGen.offset.x + half, 0f, 0f);

                // Create plane, so the skybox doesn't leak inbetween buildings.
                if(data.chunk_plane_prefab.Copy(out GameObject copy))
                {
                    copy.transform.SetParent(chunk.transform, false);
                    copy.transform.localPosition = float3.zero;
                    // Using an unity plane, its 10x10
                    copy.transform.localScale = new float3(chunk_width * .1f, 1f, copy.transform.localScale.z);
                }

                // Could make the chunk data static, but in order to keep them reusable I'll leave as is.
                chunk.Initialise(new WorldChunk.Data()
                {
                    size = chunk_width,
                    half_size = half,
                });
            }
        }

        public static bool IsOutOfBounds(float3 point, out float3 offset)
        {
            offset = float3.zero;

            float distance = point.x - Player.I.transform.position.x;

            if (WorldGen.offset.x < math.abs(distance))
            {
                // Wrap
                offset.x = WorldGen.WrapDistance() * -math.sign(distance);
                return true;
            }

            return false;
        }
    }
}
