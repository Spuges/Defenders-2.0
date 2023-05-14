using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

namespace Defender
{
    // Uhh.. More like a player controller rather than a ship, they both act differently..
    public class PlayerCraft : MonoBehaviour, IVelocity, ISpaceCraft, IShootDir
    { 
        [SerializeField] float2 sensitivity = new(20, 5);

        private float2 m_move_vector = float2.zero;
        private float2 m_last_vector = float2.zero;

        private float2 m_velocity = float2.zero;

        private Player m_player;

        [SerializeField] private PIDV2 acceleration_pid = new PIDV2(0.035f, 0f, 0f);

        public float2 velocity => m_velocity;

        public GameObject GameObject => gameObject;

        public Transform Transform => transform;

        private void Awake()
        {
            m_player = GetComponent<Player>();
        }

        public void SetMoveVector(float2 vector)
        {
            m_move_vector = vector * sensitivity;
        }

        private void FixedUpdate()
        {
            m_last_vector += acceleration_pid.Update(m_move_vector, m_last_vector, Time.fixedDeltaTime);

            if (math.lengthsq(m_last_vector) <= 0.001f)
                return;

            float2 pos = transform.position.f2();
            float2 projection = m_last_vector * Time.fixedDeltaTime;

            float2 desired_position = pos + projection;

            if(m_player)
            {
                // Clamp player.. I wonder if the buildings should just kill the player
                var bounds = GameRules.I.BoundsY;
                desired_position.y = math.clamp(desired_position.y, bounds.x, bounds.y);
            }

            m_velocity = (desired_position - pos) / Time.fixedDeltaTime;
            transform.position = new Vector3(desired_position.x, desired_position.y);

            // Object orientation towards target direction, only x axis
            if (0.001f <= math.abs(m_velocity.x))
                transform.rotation = Quaternion.LookRotation(Vector3.right * m_velocity.x, Vector3.up);
        }

        public float2 get_direction(float projectile_velocity)
        {
            return ((float3)transform.forward).xy;
        }
    }
}

