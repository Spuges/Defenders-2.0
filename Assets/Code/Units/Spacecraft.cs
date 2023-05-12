using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

namespace Defender
{
    // This thing can move.
    public class Spacecraft : MonoBehaviour, IObservable<Spacecraft.MoveData>, IVelocity, IHealth
    {
        [SerializeField] float2 sensitivity = new(20, 5);

        private float2 m_move_vector = float2.zero;
        private float2 m_last_vector = float2.zero;

        private float2 m_velocity = float2.zero;

        private Player m_player;

        [SerializeField] private PIDV2 acceleration_pid = new PIDV2(0.035f, 0f, 0f);

        [Header("Stats")]
        [SerializeField] private float base_max_health = 10f;
        private float m_health;

        float2 IVelocity.velocity => m_velocity;

        float IHealth.Health => m_health;

        // For targeting, enemies, missiles etc.
        private event Action<MoveData> onCraftMove;

        public struct MoveData
        {
            public float2 velocity;
            public float3 position;

            /// <summary>
            /// Normalised vs acceleration speed, not by normalised vector
            /// </summary>
            public float2 normalised_velocity;
        }

        public struct Death
        {
            public Spacecraft space_craft;
        }

        public struct Spawn
        {
            public Spacecraft space_craft;
        }

        private void OnEnable()
        {
            Start();
        }

        private void Awake()
        {
            m_player = GetComponent<Player>();
        }

        private void Start() // Reset and initialise here.
        {
            m_health = base_max_health;
        }

        public void SetMoveVector(float2 vector)
        {
            m_move_vector = vector;
        }

        private void FixedUpdate()
        {
            m_last_vector += acceleration_pid.Update(m_move_vector, m_last_vector, Time.fixedDeltaTime);
            PhysicsMove();
        }

        private void PhysicsMove()
        {
            if (math.lengthsq(m_last_vector) <= 0.001f)
                return;

            float2 pos = ((float3)transform.position).xy;
            float2 projection = m_last_vector * sensitivity * Time.fixedDeltaTime;

            float2 desired_position = pos + projection;

            if(m_player)
            {
                // Clamp player.. I wonder if the buildings should just kill the player
                var bounds = GameRules.I.PlayerHeightBounds;
                desired_position.y = math.clamp(desired_position.y, bounds.x, bounds.y);
            }

            m_velocity = (desired_position - pos) / Time.fixedDeltaTime;
            transform.position = new Vector3(desired_position.x, desired_position.y);

            // Object orientation towards target direction, only x axis
            if (0.001f <= math.abs(m_velocity.x))
                transform.rotation = Quaternion.LookRotation(Vector3.right * m_velocity.x, Vector3.up);

            onCraftMove?.Invoke(new MoveData()
            {
                position = transform.position,
                velocity = m_velocity,
                normalised_velocity = m_velocity / this.sensitivity,
            });
        }

        public void Subscribe(Action<MoveData> callback)
        {
            onCraftMove += callback;
        }

        public void Unsubscribe(Action<MoveData> callback)
        {
            onCraftMove -= callback;
        }

        public void Damage(Damage source, float damage)
        {
            m_health -= damage;

            if(m_health <= 0f)
            {
                GameManager.Death(new Death() { space_craft = this });
                gameObject.SetActive(false); // Something something on death.
            }
        }
    }
}

