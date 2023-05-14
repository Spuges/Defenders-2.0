using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Defender
{
    public class ShipAbilities : MonoBehaviour
    {
        [SerializeField] int max_capacity = 6;
        [SerializeField] List<BaseAbility> start_abilities = new List<BaseAbility>();

        List<IShipAbility> abilities; 
        private IActiveAbility selected;

        private void OnEnable()
        {
            abilities = new List<IShipAbility>(start_abilities.Count);
            start_abilities.ForEach(o => abilities.Add(o.CopyAbility(gameObject)));
            selected = (IActiveAbility)abilities.FirstOrDefault(o => o is IActiveAbility);
        }

        public void TryAndUse()
        {
            if (!gameObject.activeSelf)
                return;

            if(selected != null && selected.CanActivate())
            {
                selected.Activate();
            }
        }
    }
}
