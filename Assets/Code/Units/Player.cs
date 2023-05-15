using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

namespace Defender
{
    [RequireComponent(typeof(PlayerCraft))]
    [RequireComponent(typeof(ShipAbilities))]
    [RequireComponent(typeof(Health))]
    public class Player : MonoBehaviour
    {
        public static Observable OnDeath = new Observable();

        public static Player I { get; private set; }

        public PlayerCraft spacecraft { get; private set; }
        public ShipAbilities abilities { get; private set; }

        public Health hp { get; private set; }

        public int current_lives { get; private set; }
        [SerializeField] private int max_lives = 3;

        // (InputSystem) Doesn't detect my mouse, weird.. Will try to build if it is just an editor bug
        // AHA! It was the simulator. In game view it works just fine :D
        // Removed InputSystem as it bugged out completely... I've come to a conclusion that it is a big
        // pile of garboogle.
        private void Awake()
        {
            I = this;
            spacecraft = GetComponent<PlayerCraft>();
            abilities = GetComponent<ShipAbilities>();
            hp = GetComponent<Health>();

            Inputs.I.onMove.Subscribe(spacecraft.SetMoveVector);
            Inputs.I.onFire.Subscribe(abilities.TryAndUse);
            Inputs.I.onMove.Invoke(transform.forward.f2());
        }

        public void DeductLife() => current_lives--;
        public void RestoreLives() => current_lives = max_lives;


        private void OnDisable()
        {
            OnDeath?.Invoke();
            GameManager.I.PlayerDeath();
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
                    Gizmos.DrawWireSphere(transform.TransformPoint(shoot.Offset), 0.2f);
                }
            }
        }
#endif
    }
}
