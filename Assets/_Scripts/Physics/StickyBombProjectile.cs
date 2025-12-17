using UnityEngine;

public class StickyBombProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;
    public float explosionDelay = 2f;

    [Header("Ring Bomb Settings")]
    public float ringRadius = 2f;
    public float ringLaunchSpeed = 15f;

    [Header("Cooldown Settings")]
    public static float timeCooldown = 1f;
    public static int scoreCooldown = 10;

    private SimplePool pool; // bomb pool
    private AudioSource explosionAudio;
    private GameObject targetToDestroy;
    private float t;

    public static float lastFireTime = -Mathf.Infinity;
    private bool spawnRingOnExplode = false;

    public static bool TrySpawn(SimplePool p, Vector3 position, Quaternion rotation, bool spawnRing = false)
    {
        if (!spawnRing)
        {
            if (Time.time < lastFireTime + timeCooldown)
                return false;
        }
        else
        {
            if (ScoreManager.Instance == null || ScoreManager.Instance.CurrentScore < scoreCooldown)
                return false;
        }

        GameObject bomb = p.Get(position, rotation);
        if (bomb == null || bomb.Equals(null)) return false;

        StickyBombProjectile proj = bomb.GetComponent<StickyBombProjectile>();
        proj.Init(p, spawnRing);

        if (!spawnRing)
            lastFireTime = Time.time;
        else
            ScoreManager.Instance.UseScore(scoreCooldown);

        return true;
    }

    public void Init(SimplePool p, bool spawnRing = false)
    {
        pool = p;
        t = 0f;
        targetToDestroy = null;
        spawnRingOnExplode = spawnRing;

        transform.SetParent(null);

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(true);

        if (explosionAudio == null)
            explosionAudio = GameObject.Find("AS_StickyBombEx")?.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!gameObject.activeSelf || pool == null) return;
        transform.position += transform.forward * speed * Time.deltaTime;
        t += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;
        }

        if (collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("MinionCapsule") ||
            collision.gameObject.CompareTag("MinionCube") || collision.gameObject.CompareTag("MinionCylinder") ||
            collision.gameObject.CompareTag("MinionSphere"))
        {
            targetToDestroy = collision.gameObject;
        }

        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        // Spawn explosion effect from pool
        if (SimplePool.ExplosionPoolInstance != null)
        {
            GameObject effect = SimplePool.ExplosionPoolInstance.Get(transform.position, Quaternion.identity);

            if (effect != null && !effect.Equals(null))
            {
                var ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ps.Clear();
                    ps.Play();
                }
            }
        }

        if (explosionAudio != null)
            explosionAudio.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(500f, transform.position, 5f);
        }

        if (targetToDestroy != null)
        {
            string type = targetToDestroy.tag;
            GameManager.instance?.ObjectDestroyed(type);

            // Only destroy enemies, never pooled projectiles/effects
            if (!targetToDestroy.TryGetComponent<StickyBombProjectile>(out _))
            {
                Destroy(targetToDestroy);
            }
            else
            {
                var bombPool = targetToDestroy.GetComponent<StickyBombProjectile>()?.pool;
                if (bombPool != null)
                    bombPool.Return(targetToDestroy);
            }
        }

        if (spawnRingOnExplode)
            SpawnRingBombs();

        var rbSelf = GetComponent<Rigidbody>();
        if (rbSelf != null)
        {
            rbSelf.isKinematic = false;
            rbSelf.linearVelocity = Vector3.zero;
            rbSelf.angularVelocity = Vector3.zero;
        }

        pool.Return(gameObject);
    }

    void SpawnRingBombs()
    {
        if (pool == null) return;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 spawnPos = transform.position + dir * ringRadius;

            GameObject bomb = pool.Get(spawnPos, Quaternion.LookRotation(dir));
            if (bomb == null || bomb.Equals(null)) continue;

            StickyBombProjectile proj = bomb.GetComponent<StickyBombProjectile>();
            proj.Init(pool, false);

            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * ringLaunchSpeed; // Unity 6+ API
        }
    }
}
