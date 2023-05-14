using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    public class WorldChunk : MonoBehaviour
    {
        private Data settings;
        public float2 Bounds() => new float2(transform.position.x - settings.half_size, transform.position.x + settings.half_size);

        public struct Data
        {
            public float size;
            public float half_size;
        }

        public void Initialise(Data settings)
        {
            this.settings = settings;

            WorldGen.I.Subscribe(OnPropGenerated);
        }

        public void AddProp(GameObject gob)
        {
            gob.transform.SetParent(transform, true);
        }


        private bool IsInside(float x_position)
        {
            return x_position < Bounds().y && Bounds().x <= x_position;
        }

        private void OnPropGenerated(WorldGen.PropData prop)
        {
            //Debug.Log($"[{gameObject.name}] Chunk pos: {transform.position} ## Prop pos: {prop.obj.transform.position} ## {IsInside(prop.obj.transform.position.x)}");

            if(IsInside(prop.obj.transform.position.x))
            {
                //Debug.Log($"Prop generated @ {prop.obj.transform.position} and captured by chunk: {name}");
                prop.obj.transform.SetParent(transform, true);
            }
        }

        private void Update()
        {
            if (!Player.I || !Player.I.gameObject.activeInHierarchy)
                return;

            if(WorldWrapper.IsOutOfBounds(transform.position, out float3 offset))
            {
                transform.position += (Vector3)offset;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one * settings.size);
        }
    }
}
