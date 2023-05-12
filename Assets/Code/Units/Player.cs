using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;

namespace Defender
{
    [RequireComponent(typeof(Spacecraft))]
    [RequireComponent(typeof(ShipAbilities))]
    public class Player : MonoBehaviour
    {
        public static Player I { get; private set; }

        public Spacecraft GetCraft => m_spacecraft;
        private Spacecraft m_spacecraft;
        private ShipAbilities m_abilities;

        // (InputSystem) Doesn't detect my mouse, weird.. Will try to build if it is just an editor bug
        // AHA! It was the simulator. In game view it works just fine :D
        private InputAction m_move;
        private InputAction m_fire;

        private void Awake()
        {
            I = this;
            m_spacecraft = GetComponent<Spacecraft>();
            m_abilities = GetComponent<ShipAbilities>();

            m_move = Inputs.I.input_values.FindAction("Move");
            m_move.Enable();

            m_fire = Inputs.I.input_values.FindAction("Fire");
            m_fire.Enable();
        }

        private void Update()
        {
            m_spacecraft.SetMoveVector(m_move.ReadValue<Vector2>());
            bool fire = 0 < m_fire.ReadValue<float>();

            if(fire)
            {
                m_abilities?.TryAndUse();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeObject == null)
                return;

            UnityEngine.Object selection = UnityEditor.Selection.activeObject;

            if (selection is BaseAbility)
            {
                if(selection is ShootAbility)
                {
                    ShootAbility shoot = selection as ShootAbility;
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(transform.position + shoot.Offset, 0.2f);
                }
            }
        }
#endif
    }
}
