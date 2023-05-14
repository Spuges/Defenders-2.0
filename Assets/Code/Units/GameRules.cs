using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using TMPro;

namespace Defender
{
    [DefaultExecutionOrder(-100)]
    public class GameRules : MonoBehaviour
    {
        public static GameRules I { get; private set; }

        public float2 BoundsY => new float2(min_height_for_player, max_height_for_player);

        [SerializeField] private float max_height_for_player = 27f;
        [SerializeField] private float min_height_for_player = 6.5f;

        private void Awake()
        {
            I = this;

        }

        private void OnDrawGizmosSelected()
        {
            Camera cam = Camera.main;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.left * math.INFINITY, Vector3.right * math.INFINITY);
        }
    }
}
