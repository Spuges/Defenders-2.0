using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace Defender
{
    public class ShipAbilities : MonoBehaviour
    {
        [SerializeField] int max_capacity = 6;
        [SerializeField] List<BaseAbility> start_abilities = new List<BaseAbility>();

        List<IShipAbility> abilities; 
        private IActiveAbility selected;
        private InputAction m_fire;

        private void Start()
        {
            m_fire = Inputs.I.input_values.FindAction("Fire");
            m_fire.Enable();

            abilities = new List<IShipAbility>(start_abilities.Count);

            start_abilities.ForEach(o => abilities.Add(o.CopyAbility(gameObject)));
            selected = (IActiveAbility)abilities.FirstOrDefault(o => o is IActiveAbility);
        }

        private void Update()
        {
            // Do something fancy
            bool fire = 0 < m_fire.ReadValue<float>();

            if (fire && selected != null && selected.CanActivate())
            {
                selected.Activate();
            }
        }
    }
}
