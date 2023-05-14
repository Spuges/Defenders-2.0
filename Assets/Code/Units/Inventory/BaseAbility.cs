using UnityEngine;

namespace Defender
{
    // I wish unity could serialize objects based on interface reference.
    public abstract class BaseAbility : ScriptableObject, IShipAbility
    {
        [SerializeField] public Sprite GetIcon => default;

        [SerializeField] public string GetName => default;

        protected GameObject owner { get; private set; }

        public IShipAbility CopyAbility(GameObject owner)
        {
            // Should look from pool first... TODO
            var instance = Instantiate(this);
            instance.owner = owner;
            instance.Initialise();
            return instance;
        }

        protected abstract void Initialise();
    }
}