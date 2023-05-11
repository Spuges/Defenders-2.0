using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

namespace Defender
{
    // This thing can move.
    public class Spacecraft : MonoBehaviour, IObservable<float3>, IVelocity
    {
        [SerializeField] float2 sensitivity = new(20, 5);

        private float2 m_move_vector = float2.zero;
        private float2 m_velocity = float2.zero;

        private Player m_player;

        public float2 velocity => m_velocity;

        // For targeting, enemies, missiles etc.
        private event Action<float3> onCraftMove;

        private void Start()
        {
            m_player = GetComponent<Player>();
        }

        public void SetMoveVector(float2 vector)
        {
            m_move_vector = vector;
        }

        private void FixedUpdate()
        {
            PhysicsMove();
        }

        private void PhysicsMove()
        {
            if (math.lengthsq(m_move_vector) <= 0.001f)
                return;


            float2 pos = ((float3)transform.position).xy;
            float2 projection = m_move_vector * sensitivity * Time.fixedDeltaTime;

            float2 desired_position = pos + projection;

            if(m_player)
            {
                // Clamp player.. I wonder if the buildings should just kill the player
                var bounds = GameRules.I.PlayerHeightBounds;
                desired_position.y = math.clamp(desired_position.y, bounds.x, bounds.y);
            }

            m_velocity = desired_position - pos;
            transform.position = new Vector3(desired_position.x, desired_position.y);

            // Object orientation towards target direction, only x axis
            if (0.001f <= math.abs(m_velocity.x))
                transform.rotation = Quaternion.LookRotation(Vector3.right * m_velocity.x, Vector3.up);

            onCraftMove?.Invoke(transform.position);
        }

        public void Subscribe(Action<float3> callback)
        {
            onCraftMove += callback;
        }

        public void Unsubscribe(Action<float3> callback)
        {
            onCraftMove -= callback;
        }
    }
}

