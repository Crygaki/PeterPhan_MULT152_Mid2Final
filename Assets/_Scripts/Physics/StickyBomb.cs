using UnityEngine;

public class StickyBomb : MonoBehaviour
{
    public float explosionDelay = 2f;
    public GameObject explosionEffect;
    private GameObject targetToDestroy;
    private AudioSource explosionAudio;

    void Start()
    {
        //Find and get the AudioSource component
        explosionAudio = GameObject.Find("AS_StickyBombEx").GetComponent<AudioSource>();

        if(explosionAudio == null) //Null check
        {
            Debug.LogError("AudioSource is null.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Stick to the object
        if (collision.rigidbody != null)
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;
        }

        // If the target is an enemy or pickup, store the object to destroy later
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

        // Play the explosion audio
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

        // Destroy the target object after explosion effect
        if (targetToDestroy != null)
        {
            string type = targetToDestroy.tag;
            WinManager.instance?.RegisterDestruction(type);
            Destroy(targetToDestroy);
        }

        // Destroy the bomb itself
        Destroy(gameObject);
    }
}
