using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public class RotateByVelocity : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigid;
        [SerializeField] private float rotation_speed = 15f;
         
        private void LateUpdate()
        {
            if(rigid.velocity.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rigid.velocity, Vector3.up), rotation_speed * Time.deltaTime);
        }
    }
}
