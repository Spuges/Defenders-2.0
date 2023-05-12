using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    public class FollowCamera : MonoBehaviour
    {
        public static FollowCamera I { get; private set; }

        [SerializeField] private Material skybox_material;

        [SerializeField, Range(0f, 1f)] private float max_follow_offset = .8f;
        private float left_bound_velocity_offset; // Lets make this aspect ratio friendly. needs to be recalculated when resolution changes
        [SerializeField] private PIDV3 follow_pid = new PIDV3(0.05f, 0f, 0f);

        private float3 camera_offset;
        private float3 target_last_pos;
        private float2 velocity_offset;

        private void Awake()
        {
            I = this;

            camera_offset = transform.position;
            SetCameraOffset();
        }

        private void Start()
        {
            target_last_pos = Player.I.transform.position;
            SetSkyboxRotation();
            Player.I.GetCraft.Subscribe(SetFollowTarget);
        }

        void SetFollowTarget(Spacecraft.MoveData data)
        {
            velocity_offset = data.normalised_velocity.x * new float2(1, 0) * left_bound_velocity_offset;
            target_last_pos = data.position + new float3(velocity_offset, 0);
            SetSkyboxRotation();
        }

        private void FixedUpdate()
        {
            if (!Mathf.Approximately(transform.position.x, target_last_pos.x))
                SetSkyboxRotation();
        }

        private void SetSkyboxRotation()
        {
            float3 target_point = new float3(target_last_pos.x, 0f, 0f) + camera_offset;
            transform.position += (Vector3)follow_pid.Update(target_point, transform.position, Time.fixedDeltaTime);
            float rotation = (transform.position.x / (WorldGen.WrapDistance() / 360f) % 360f);
            skybox_material.SetFloat("_Rotation", rotation);
        }

        // Should be called every time 
        void SetCameraOffset()
        {
            Camera main_cam = Camera.main;

            Plane plane = new Plane(-Vector3.forward, Vector3.zero);

            Ray ray = new Ray(transform.position, transform.forward);
            Ray left_point_ray = main_cam.ViewportPointToRay(new Vector3(0, .5f));

            if (plane.Raycast(ray, out float forward_distance))
            {
                camera_offset = ray.GetPoint(forward_distance) - ray.direction * forward_distance;

                if (plane.Raycast(left_point_ray, out float left_distance))
                {
                    Debug.DrawLine(left_point_ray.GetPoint(left_distance), ray.GetPoint(forward_distance), Color.red, 1000f);
                    left_bound_velocity_offset = math.distance(left_point_ray.GetPoint(left_distance), ray.GetPoint(forward_distance)) * max_follow_offset;
                }
            }
            else
            {
                camera_offset = Vector3.zero;
            }
        }
    }
}
