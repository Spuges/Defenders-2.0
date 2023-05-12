using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;

namespace Defender
{
    [RequireComponent(typeof(Spacecraft))]
    public class Player : MonoBehaviour
    {
        public static Player I { get; private set; }

        public Spacecraft GetCraft => m_spacecraft;
        private Spacecraft m_spacecraft;

        // (InputSystem) Doesn't detect my mouse, weird.. Will try to build if it is just an editor bug
        // AHA! It was the simulator. In game view it works just fine :D
        private InputAction m_move;

        private void Awake()
        {
            I = this;
            m_spacecraft = GetComponent<Spacecraft>();

            m_move = Inputs.I.input_values.FindAction("Move");
            m_move.Enable();
        }

        private void Update()
        {
            m_spacecraft.SetMoveVector(m_move.ReadValue<Vector2>());
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
