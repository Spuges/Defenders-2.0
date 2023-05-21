using UnityEngine;
using System;
using Unity.Mathematics;

namespace Assets.Code.Helpers
{
    public static class mathd
    {
        public static float3 CalculateShootVector(float3 target, float3 shooter, float3 target_velocity, float3 projectile_velocity)
        {
            float bulletSpeed = 10f; // set the bullet speed
            float timeToHit = 0f; // initialize the time to hit as 0
                                  // calculate the relative position and velocity of the target
            Vector3 targetRelativePosition = target - shooter;
            Vector3 targetRelativeVelocity = target_velocity - projectile_velocity;

            // calculate the quadratic coefficients
            float a = Vector3.Dot(targetRelativeVelocity, targetRelativeVelocity) - bulletSpeed * bulletSpeed;
            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = Vector3.Dot(targetRelativePosition, targetRelativePosition);

            // solve the quadratic equation
            float disc = b * b - 4f * a * c;
            if (disc >= 0f)
            {
                float t1 = (-b - Mathf.Sqrt(disc)) / (2f * a);
                float t2 = (-b + Mathf.Sqrt(disc)) / (2f * a);
                timeToHit = Mathf.Max(t1, t2); // choose the larger positive root
            }
            // calculate the aim point
            return target + timeToHit * target_velocity;
        }
    }
}
