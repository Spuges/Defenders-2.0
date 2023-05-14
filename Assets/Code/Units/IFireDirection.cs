using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    public interface IShootDir
    {
        public float2 get_direction(float projectile_velocity);
    }
}
