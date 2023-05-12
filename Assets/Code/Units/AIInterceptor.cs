using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    [RequireComponent(typeof(Spacecraft))]
    [RequireComponent(typeof(ShipAbilities))]
    public class AIInterceptor : AIBase
    {
        public enum State
        {
            Leader,
            Wingman
        }

        private Spacecraft craft;
        private ShipAbilities abilites;

        private Player target => Player.I;

        private void Start()
        {
            craft = GetComponent<Spacecraft>();
            abilites = GetComponent<ShipAbilities>();
        }

        private void FixedUpdate()
        {
            
        }
    }
}
