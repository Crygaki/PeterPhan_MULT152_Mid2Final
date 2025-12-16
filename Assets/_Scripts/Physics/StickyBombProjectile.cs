using UnityEngine;

public class StickyBombProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float explosionDelay = 2f;
    public GameObject explosionEffect;
    private SimplePool pool;
    private AudioSource explosionAudio;
    private GameObject targetToDestroy;
    private float t;

    // --- Cooldown fields ---
    public static float cooldownTime = 1f; // 1 second between bombs
    public static float lastFireTime = -Mathf.Infinity;

    /// <summary>
    /// Call this instead of directly spawning a bomb.
    /// Returns true if bomb was successfully initialized, false if still on cooldown.
    /// </summary>
    public static bool TrySpawn(SimplePool p, Vector3 position, Quaternion rotation)
    {
        // Enforce cooldown
        if (Time.time < lastFireTime + cooldownTime)
        {
            return false;
        }

        GameObject bomb = p.Get(position, rotation);
        StickyBombProjectile proj = bomb.GetComponent<StickyBombProjectile>();
        proj.Init(p);

        lastFireTime = Time.time;
        return true;
    }

    public void Init(SimplePool p)
    {
        pool = p;
        t = 0f;
        targetToDestroy = null;
        transform.SetParent(null);
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        gameObject.SetActive(true);

        if (explosionAudio == null)
        {
            explosionAudio = GameObject.Find("AS_StickyBombEx")?.GetComponent<AudioSource>();
        }
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
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

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
            Destroy(targetToDestroy);
        }

        transform.SetParent(null);
        var rbSelf = GetComponent<Rigidbody>();
        if (rbSelf != null) rbSelf.isKinematic = false;
        pool.Return(gameObject);
    }
}
