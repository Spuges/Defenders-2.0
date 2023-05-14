using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    [DefaultExecutionOrder(-200)]
    public class Inputs : MonoBehaviour
    {
        public static Inputs I { get; private set; }

        public Observable           onEscape = new();

        public Observable<float2>   onMove = new();
        public Observable           onFire = new();

        private void Awake()
        {
            I = this;
        }

        private void Update()
        {
            float2 move_raw = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            onMove?.Invoke(move_raw);

            if(Input.GetKey(KeyCode.Mouse0))
                onFire?.Invoke();

            if (Input.GetKeyDown(KeyCode.Escape))
                onEscape?.Invoke();
        }
    }
}
