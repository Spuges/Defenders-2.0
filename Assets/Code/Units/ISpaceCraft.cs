using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    public interface ISpaceCraft
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }

        public struct Death
        {
            public ISpaceCraft space_craft;
            public Damage source;
        }

        public struct Spawn
        {
            public ISpaceCraft space_craft;
        }

        public struct MoveData
        {
            public float2 velocity;
            public float3 position;

            /// <summary>
            /// Normalised vs acceleration speed, not by normalised vector
            /// </summary>
            public float2 normalised_velocity;
        }
    }

}
