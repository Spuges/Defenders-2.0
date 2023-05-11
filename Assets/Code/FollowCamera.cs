using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defender
{
    public class FollowCamera : MonoBehaviour
    {
        public static FollowCamera I { get; private set; }

        [SerializeField] private Material skybox_material;


        private Vector3 camera_offset;
        private Vector3 target_last_pos;

        private void Awake()
        {
            I = this;

            camera_offset = transform.position;
            SetCameraOffset();
        }

        private void Start()
        {
            transform.position = new Vector3(Player.I.transform.position.x, 0f, 0f) + camera_offset;
            float rotation = (transform.position.x / (WorldGen.I.GetWrapDistance / 360f) % 360f);
            skybox_material.SetFloat("_Rotation", rotation);
        }

        private void LateUpdate()
        {
            FollowPlayer();
        }

        void FollowPlayer()
        {
            if(Player.I == null || Player.I.GetCraft == null)
            {
                return;
            }

            if (Mathf.Approximately(Player.I.transform.position.x, target_last_pos.x))
                return;

            transform.position = new Vector3(Player.I.transform.position.x, 0f, 0f) + camera_offset;
            float rotation = (transform.position.x / (WorldGen.I.GetWrapDistance / 360f) % 360f);
            skybox_material.SetFloat("_Rotation", rotation);
            target_last_pos = Player.I.transform.position;
        }

        void SetCameraOffset()
        {
            Plane plane = new Plane(-Vector3.forward, Vector3.zero);

            Ray ray = new Ray(transform.position, transform.forward);

            if (plane.Raycast(ray, out float value))
            {
                Debug.Log("Derp");
                camera_offset = ray.GetPoint(value) - ray.direction * value;
            }
            else
            {
                camera_offset = Vector3.zero;
            }
        }
    }
}
