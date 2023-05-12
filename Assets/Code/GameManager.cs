using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Defender
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        public Observable<Spacecraft.Death> onDeathEvent = new();
        public Observable<Spacecraft.Spawn> onSpawnEvent = new();

        public static void Death(Spacecraft.Death death)
        {
            I.onDeathEvent.Invoke(death);
        }

        private void Awake()
        {
            I = this;
        }

        public void OnReset()
        {
            onDeathEvent = null;
        }
    }
}
