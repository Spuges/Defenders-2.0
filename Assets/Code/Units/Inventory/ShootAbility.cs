using UnityEngine;
using Unity.Mathematics;

namespace Defender
{
    [CreateAssetMenu(fileName = "New ShootAbility", menuName = "Game/Abilities/Shoot Ability")]
    public class ShootAbility : BaseAbility, IActiveAbility
    {
        // Quality of life
        public Vector3 Offset => spawn_offset; // Draw a wire sphere on the spawn position of the player obect in the scene.

        [SerializeField] float start_velocity = 50f;
        [SerializeField] float cooldown = default;
        [SerializeField] float damage = default;
        [SerializeField] Damage projectile = default;
        [SerializeField] float3 spawn_offset;
        [SerializeField] float life_time = 10f; // Die and go to pool.
        [SerializeField] bool inherit_velocity = true;

        private Collider m_collider;
        private IVelocity m_velocity;
        private IShootDir m_target;
        private Transform m_transform;

        private float next_fire = 0f;

        bool IActiveAbility.CanActivate() => CanFire();
        void IActiveAbility.Activate() => Shoot();

        public float get_cooldown => cooldown;

        public float get_cooldown_remaining => next_fire - Time.timeSinceLevelLoad;


        /// <summary>
        /// Happens after the ability is copied
        /// </summary>
        protected override void Initialise()
        {
            m_transform = owner.GetComponent<Transform>();  // To get the rotation
            m_target = owner.GetComponent<IShootDir>();
            m_velocity = owner.GetComponent<IVelocity>();   // Add velocity for projectiles depending on the flight speed
            m_collider = owner.GetComponent<Collider>();    // Ignore collision

            //Debug.Log($"Initialise {nameof(ShootAbility)} for {owner.name} - velocity component: {m_velocity == null}");
        }

        public bool CanFire()
        {
            return next_fire <= Time.timeSinceLevelLoad;
        }

        public void Shoot()
        {
            if (!CanFire())
                return;

            next_fire = Time.timeSinceLevelLoad + cooldown;

            float3 offset = spawn_offset;

            GameObject new_projectile = default;

            if (projectile.Copy(out Damage dmg))
            {
                dmg.owner = owner;
                dmg.amount = damage; // Override the old damage
                new_projectile = dmg.gameObject;

                if (m_collider != null)
                    dmg.SetIgnoreCollision(m_collider, new_projectile.GetComponent<Collider>());
            }
            else
            {
                Debug.LogError($"Ro-ou, no projectile prefab");
            }

            // Initial position
            new_projectile.transform.position = m_transform.TransformPoint(offset);

            float2 dir = m_target.get_direction(start_velocity);

            float2 projectile_velocity = dir * start_velocity;

            Rigidbody rigid = new_projectile.GetComponent<Rigidbody>();
            rigid.velocity = new float3(projectile_velocity, 0f);

            // Add inherited velocity
            if (m_velocity != null && inherit_velocity)
            {
                rigid.velocity += new Vector3(m_velocity.velocity.x, 0, 0);
            }

            // Ignore origin - collision

            Lifetime life = new_projectile.GetComponent<Lifetime>() ?? new_projectile.AddComponent<Lifetime>();
            life.Setup(life_time);
        }
    }
}