using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Defender
{
    [DefaultExecutionOrder(-200)]
    public class Inputs : MonoBehaviour
    {
        public static Inputs I { get; private set; }

        private void Awake()
        {
            I = this;
        }
    }
}
