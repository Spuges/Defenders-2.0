using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    // This thing can move.
    public class Spacecraft : MonoBehaviour
    {
        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float accelerationSpeed = 100f;

        private Vector2 m_move_vector = Vector2.zero;
        private Vector2 m_velocity = Vector2.zero;

        public void SetMoveVector(Vector2 vector)
        {
            // Do something fancy
            m_move_vector = vector;
        }

        private void FixedUpdate()
        {
            PhysicsMove();
        }

        private void PhysicsMove()
        {
            if (Mathf.Approximately(m_velocity.sqrMagnitude, 0f))
                return;

            m_velocity = m_move_vector;
        }
    }
}

