using UnityEngine;

namespace Defender
{
    public interface IActiveAbility 
    {
        public float get_cooldown { get; }
        public float get_cooldown_remaining { get; }

        public bool CanActivate();
        public void Activate();
    }
}
