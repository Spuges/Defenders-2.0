using UnityEngine;

namespace Defender
{
    public interface IShipAbility 
    {
        public Sprite GetIcon { get; }
        public string GetName { get; }
    }
}
