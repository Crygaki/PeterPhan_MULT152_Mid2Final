using UnityEngine;
using System.Collections;

public class PooledParticle : MonoBehaviour
{
    [HideInInspector] public SimplePool pool;
    [HideInInspector] public ParticleSystem ps;

    void OnEnable()
    {
        if (ps != null && pool != null)
            StartCoroutine(FallbackReturn(ps.main.duration));
    }

    void OnParticleSystemStopped()
    {
        if (pool != null)
            pool.Return(gameObject);
    }

    private IEnumerator FallbackReturn(float delay)
    {
        yield return new WaitForSeconds(delay + 0.5f);
        if (gameObject.activeSelf && pool != null)
            pool.Return(gameObject);
    }
}
