using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public abstract class WinCondition : ScriptableObject
    {
        public abstract void Initialise();
        public abstract bool CheckWon();
    }
}
