using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Defender
{
    [RequireComponent(typeof(ShipAbilities))]
    [RequireComponent(typeof(Rigidbody))]
    public class AIInterceptor : AIBase, IVelocity, IShootDir
    {
        [SerializeField] float intercept_dist = 10f;
        [SerializeField] float intercept_vary = 4f;
        [SerializeField] float passive_velocity = 40f; // Not really passive, but.. eh
        [SerializeField] float max_velocity = 75f;
        [SerializeField] PIDV3 accelerate_pid = new PIDV3(0.75f, 0f, 0f);
        [SerializeField] float fire_spread = 6f;

        [SerializeField] private LayerMask avoid_layer;
        [SerializeField] private float avoid_distance = 3f;
        [SerializeField] private Transform orientation_body;
        [SerializeField] private float fire_distance;

        // Difficulty modifiers
        private float m_fire_dist => fire_distance * GameManager.I.GetDifficulty();
        private float m_intrcpt_dist => intercept_dist * GameManager.I.GetDifficulty();
        private float m_intrcpt_vary => intercept_vary * GameManager.I.GetDifficulty();
        private float m_max_vel => max_velocity * GameManager.I.GetDifficulty();
        private float m_pas_vel => passive_velocity * GameManager.I.GetDifficulty();

        public Rigidbody body { get; private set; }

        public PlayerCraft craft { get; private set; }
        public ShipAbilities abilites { get; private set; }

        private float spawn_age;
        private Player player => Player.I;

        public float2 velocity => new float2(body.velocity.x, body.velocity.y);

        public float2 get_target_pos => new float3(follow_target.transform.position).xy;

        public float2 get_projected_target_point
        {
            get
            {
                return get_target_velocity + get_target_pos;
            }
        }

        public float2 get_target_velocity => follow_target.velocity / Time.fixedDeltaTime;

        private float2 bounds;

        private PlayerCraft follow_target;
        private float3? target_point;
        private bool disengage = false;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
            craft = GetComponent<PlayerCraft>();
            abilites = GetComponent<ShipAbilities>();

            bounds = GameRules.I.BoundsY;

            GetComponent<Health>()?.Subscribe((changed) => 
            {
                if (changed.source?.GetComponent<Player>()) // Null safe check
                    follow_target = player.spacecraft;
            });
        }

        void OnPlayerSpawned(Player player)
        {
            StopAllCoroutines();
            StartCoroutine(EngageCheck());
        }

        public void OnEnable()
        {
            GameManager.I.onPlayerSpawned.Subscribe(OnPlayerSpawned);

            spawn_age = Time.timeSinceLevelLoad;
            follow_target = null;

            StartCoroutine(EngageCheck());
        }

        private void OnDisable()
        {
            GameManager.I.onPlayerSpawned.Unsubscribe(OnPlayerSpawned);
        }

        private IEnumerator EngageCheck()
        {
            yield return new WaitForSeconds(2.5f);
            
            while(!GameManager.I.GameOver())
            {
                float vary = Random.Range(-m_intrcpt_vary, m_intrcpt_vary);
                float dist = math.distance(player.transform.position, transform.position);
                float interception_delta = vary + Time.timeSinceLevelLoad - spawn_age + (m_intrcpt_dist - dist);

                if (1f <= GameManager.I.LevelProgress() || 5f < interception_delta)
                {
                    follow_target = player.spacecraft;
                    yield break;
                }

                yield return new WaitForSeconds(.5f);
            }
        }

        private float3 GetRoamingTargetDir()
        {
            if (target_point.HasValue)
            {
                float distance = math.distance(transform.position, target_point.Value);
                // Check if we reached the target
                if (distance < 0.5f || 30f < distance) // Break roaming target if we get teleported to the other side of the world..
                {
                    target_point = NewRoamTarget();
                }   
            }
            else
            {
                target_point = NewRoamTarget();
            }

            Debug.DrawLine(transform.position, target_point.Value);

            return target_point.Value - (float3)transform.position;
        }

        private float3 NewRoamTarget()
        {
            // New target point
            float pos_y = Random.Range(bounds.x + 3f, bounds.y);

            // Give forward direction higher bias

            float x = body.velocity.x;

            float sign;

            if(x <= 0)
                sign = math.sign(Random.Range(-5f, 1f));
            else
                sign = math.sign(Random.Range(-1f, 5f));

            float pos_x = transform.position.x + sign * Random.Range(20, 40);

            return new float3(pos_x, pos_y, 0f);
        }

        private void Update()
        {
            if(follow_target)
            {
                if (!follow_target.gameObject.activeInHierarchy)
                {
                    follow_target = null;
                    return;
                }

                float3 dir = (float3)transform.position - new float3(get_target_pos, 0f);

                if (math.length(dir) < m_fire_dist)
                {
                    abilites.TryAndUse();
                }
            }
        }

        public float2 get_direction(float projectile_velocity)
        {
            float2 t_pos = follow_target.transform.position.f2();

            float2 rand = Random.insideUnitCircle * fire_spread;
            
            return math.normalize(t_pos - transform.position.f2() - rand);
        }

        private float3 FollowDir()
        {

            float3 position = transform.position;
            float3 targ_pos = follow_target.transform.position.f3();

            float3 target_vector = targ_pos - position;
            // Distance vector
            float3 ft = target_vector;
            
            if (math.abs(target_vector.x) > m_fire_dist / 2f && !disengage)
            {
                float dir = math.sign(target_vector.x);
                ft = new float3(targ_pos.x - dir * m_fire_dist / 2.2f, targ_pos.y, 0f);

                ft = ft - position;
                Debug.DrawRay(transform.position, ft, Color.green);
            }
            else
            {
                disengage = true;
                float center = bounds.x + (bounds.y - bounds.x) * .5f;

                float y_delta = targ_pos.y - bounds.x - center;

                float sign = math.sign(y_delta);
                float dir = -math.sign(target_vector.x);

                if (sign < 0f)
                {
                    ft = new float3(targ_pos.x + dir * m_fire_dist * 2f, bounds.y - 2f, 0f);
                }
                else
                {
                    ft = new float3(targ_pos.x + dir * m_fire_dist * 2f, bounds.x + 2f, 0f);
                }

                if (math.abs(target_vector.x) > m_fire_dist * 2f)
                {
                    disengage = false;
                }

                Debug.DrawLine(transform.position, ft, Color.red);
            }

            return ft;
        }

        private void FixedUpdate()
        {
            float3 target_direction = default;

            if (follow_target)
            {
                target_direction = FollowDir();
            }
            else
            {
                target_direction = GetRoamingTargetDir();
            }

            if (CollisionCheck(out float3 new_dir))
            {
                target_direction += new_dir;
            }

            if(0.001f < math.lengthsq(target_direction))
            {
                //Debug.DrawRay(transform.position, target_direction, Color.cyan);
                target_direction = math.normalize(target_direction);
                float vel = disengage ? m_max_vel : m_pas_vel;
                float3 desired_velocity = accelerate_pid.Update(target_direction * vel, body.velocity, Time.fixedDeltaTime);
                body.velocity += Vector3.ClampMagnitude(desired_velocity, vel * Time.fixedDeltaTime);
            }

            if (WorldWrapper.IsOutOfBounds(transform.position, out float3 offset))
            {
                transform.position += (Vector3)offset;
            }

            transform.position = new float3(transform.position.x, math.clamp(transform.position.y, bounds.x, bounds.y), transform.position.z);
        }

        private bool CollisionCheck(out float3 target_direction)
        {
            target_direction = Vector3.zero;
            
            bool collision = false;
            
            float3 position = transform.position;
            
            Collider[] obstacles = Physics.OverlapSphere(position, avoid_distance, avoid_layer.value);

            int collision_count = 0;

            foreach (Collider obstacle in obstacles)
            {
                if (obstacle.gameObject == gameObject)
                    continue;


                float3 closest_point = obstacle.ClosestPoint(position);
                float3 normal = position - closest_point;

                target_direction += normal * avoid_distance;

                collision = true;
                collision_count++;
            }

            target_direction /= collision_count;
            target_direction /= avoid_distance;

            return collision;
        }
    }
}
