using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    [RequireComponent(typeof(Spacecraft))]
    [RequireComponent(typeof(ShipAbilities))]
    [RequireComponent(typeof(Rigidbody))]
    public class AIInterceptor : AIBase
    {
        [RuntimeInitializeOnLoadMethod()]
        private static void InitLayer() => AILayer = LayerMask.NameToLayer("ai");

        private static LayerMask AILayer;

        public enum State
        {
            Leader,
            Wingman,
            Roaming,
        }

        [SerializeField] float group_distance = 15f;
        [SerializeField] float intercept_distance = 10f;
        [SerializeField] float follow_distance = 3f;
        [SerializeField] float passive_velocity = 40f;
        [SerializeField] float max_velocity = 75f;
        [SerializeField] PIDV3Clamped accelerate_pid = new PIDV3Clamped(0.75f, 0f, 0f);

        [SerializeField, Min(0.01f)] float state_update_interval = 0.1f;

        [SerializeField] private LayerMask avoid_layer;
        [SerializeField] private float avoid_distance = 3f;

        private Rigidbody body;

        private Spacecraft craft;
        private ShipAbilities abilites;

        private State m_state = State.Roaming;
        private bool is_engaging = false;
        private Player enemy => Player.I;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
            craft = GetComponent<Spacecraft>();
            abilites = GetComponent<ShipAbilities>();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            m_state = State.Roaming;

            StartCoroutine(AIRoutine());
        }

        public void OnJoin(AIInterceptor interceptor)
        {

        }

        private IEnumerator AIRoutine()
        {
            while(true)
            {
                switch (m_state)
                {
                    case State.Leader: Lead(); break;
                    case State.Wingman: Follow(); break;
                    case State.Roaming: Roam(); break;
                }

                yield return new WaitForSeconds(state_update_interval);
            }
        }

        private void Lead()
        {
        }

        private void Follow()
        {

        }

        private void Roam()
        {
            var bounds = GameRules.I.PlayerHeightBounds;
            var player = Player.I;
        }

        private void FixedUpdate()
        {
            float3 position = transform.position;
            float3 target_direction = (float3)enemy.transform.position - position;
 
            Collider[] obstacles = Physics.OverlapSphere(position, avoid_distance, avoid_layer.value);

            float closest_distance = float.MaxValue;
            foreach(Collider obstacle in obstacles)
            {
                if (obstacle.gameObject == gameObject)
                    continue;

                float dist = math.distancesq(obstacle.ClosestPoint(position), position);
                if (dist < closest_distance) // Avoid the closest one.
                {
                    float3 closest_point = obstacle.ClosestPoint(position);
                    float3 normal = math.normalize(position - closest_point);

                    Debug.DrawRay(closest_point, normal * avoid_distance, Color.red);
                    target_direction = normal * avoid_distance;
                    Debug.DrawRay(position, target_direction, Color.green);
                    closest_distance = dist;
                }
            }

            if(0.001f < math.lengthsq(target_direction))
            {
                float3 desired_velocity = accelerate_pid.Update(target_direction, body.velocity, Time.fixedDeltaTime, passive_velocity);
                body.velocity += (Vector3)desired_velocity;
            }

            if (WorldWrapper.IsOutOfBounds(transform.position, out float3 offset))
            {
                transform.position += (Vector3)offset;
            }
        }
    }
}
