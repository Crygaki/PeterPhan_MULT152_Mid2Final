using UnityEngine;
using System.Collections.Generic;

public class SimplePool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int warmup = 20;

    private readonly Queue<GameObject> q = new();

    // Optional: mark this pool as the explosion pool
    public static SimplePool ExplosionPoolInstance { get; private set; }

    void Awake()
    {
        // If this pool is meant for explosions, mark it globally
        if (prefab != null && prefab.name.Contains("Explosion"))
        {
            if (ExplosionPoolInstance == null)
                ExplosionPoolInstance = this;
        }

        // Warmup pool
        for (int i = 0; i < warmup; i++)
        {
            var go = Instantiate(prefab, transform);
            AttachPooledParticle(go);
            go.SetActive(false);
            q.Enqueue(go);
        }
    }

    public GameObject Get(Vector3 pos, Quaternion rot)
    {
        GameObject go = null;

        // Dequeue until we find a valid object
        while (q.Count > 0 && (go == null || go.Equals(null)))
        {
            go = q.Dequeue();
        }

        if (go == null || go.Equals(null))
        {
            go = Instantiate(prefab, transform);
        }

        if (go == null || go.Equals(null)) return null; // final guard

        AttachPooledParticle(go);

        go.transform.SetParent(null);
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        var rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        return go;
    }

    public void Return(GameObject go)
    {
        if (go == null || go.Equals(null)) return;

        go.SetActive(false);
        go.transform.SetParent(transform);
        q.Enqueue(go);
    }

    private void AttachPooledParticle(GameObject go)
    {
        if (go == null || go.Equals(null)) return;

        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Callback;

            var helper = go.GetComponent<PooledParticle>();
            if (helper == null)
                helper = go.AddComponent<PooledParticle>();

            helper.pool = this;
            helper.ps = ps;
        }
    }
}
