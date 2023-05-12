using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    [DefaultExecutionOrder(-100)]
    public class AIManager : MonoBehaviour
    {
        public static AIManager I { get; private set; }

        private void Awake()
        {
            I = this;
        }


    }
}
