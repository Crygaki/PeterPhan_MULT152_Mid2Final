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

    public void Init(SimplePool p)
    {
        pool = p;
        t = 0f;
        targetToDestroy = null;
        transform.SetParent(null);
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        gameObject.SetActive(true);

        // Find audio source once
        if (explosionAudio == null)
        {
            explosionAudio = GameObject.Find("AS_StickyBombEx")?.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf || pool == null) return;

        // Move forward until collision
        transform.position += transform.forward * speed * Time.deltaTime;
        t += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Stick to target
        if (collision.rigidbody != null)
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;
        }

        // Track target for destruction
        if (collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("Minion") ||
            collision.gameObject.CompareTag("GrayBall") || collision.gameObject.CompareTag("BlackBall") ||
            collision.gameObject.CompareTag("WoodBarrel") || collision.gameObject.CompareTag("YellowBox") ||
            collision.gameObject.CompareTag("RedBox"))
        {
            targetToDestroy = collision.gameObject;
        }

        // Start countdown to explosion
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        // Spawn explosion effect
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Play audio
        if (explosionAudio != null)
            explosionAudio.Play();

        // Apply explosion force
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(500f, transform.position, 5f);
        }

        // Destroy target if applicable
        if (targetToDestroy != null)
        {
            string type = targetToDestroy.tag;
            WinManager.instance?.RegisterDestruction(type);
            Destroy(targetToDestroy);
        }

        // Reset and return bomb to pool
        transform.SetParent(null);
        var rbSelf = GetComponent<Rigidbody>();
        if (rbSelf != null) rbSelf.isKinematic = false;
        pool.Return(gameObject);
    }
}
