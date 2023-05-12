using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public interface IHealth
    {
        public float Health { get; }
        public void Damage(Damage source, float damage);
    }
}
