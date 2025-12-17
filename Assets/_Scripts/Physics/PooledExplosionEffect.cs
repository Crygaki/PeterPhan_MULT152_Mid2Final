using UnityEngine;
using System.Collections;
using System.Linq;

public class PooledExplosionEffect : MonoBehaviour
{
    [SerializeField] private bool playOnEnable = true;

    private SimplePool pool;
    private ParticleSystem[] systems;

    // Call this once when spawning from the pool
    public void Init(SimplePool owningPool)
    {
        pool = owningPool;

        if (systems == null || systems.Length == 0)
            systems = GetComponentsInChildren<ParticleSystem>(includeInactive: true);

        // Reset all particle systems for reuse
        foreach (var ps in systems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Clear(true);
            ps.time = 0f;
        }

        if (playOnEnable)
        {
            foreach (var ps in systems)
                ps.Play(true);
        }

        // Compute a safe return time and schedule return
        float returnDelay = ComputeMaxLifetime(systems);
        StopAllCoroutines();
        StartCoroutine(ReturnAfter(returnDelay));
    }

    private float ComputeMaxLifetime(ParticleSystem[] particleSystems)
    {
        if (particleSystems == null || particleSystems.Length == 0)
            return 2f; // fallback

        float max = 0f;

        foreach (var ps in particleSystems)
        {
            var main = ps.main;

            // Estimate lifetime = system duration + max start lifetime
            float duration = main.duration;
            float startLife = 0f;

            var sl = main.startLifetime;
            switch (sl.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    startLife = sl.constant;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    startLife = sl.constantMax;
                    break;
                case ParticleSystemCurveMode.Curve:
                    startLife = sl.curve.keys.Length > 0 ? sl.curve.keys.Max(k => k.value) : 0f;
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    // take the higher of the two curves’ peaks
                    float maxA = sl.curveMax.keys.Length > 0 ? sl.curveMax.keys.Max(k => k.value) : 0f;
                    float maxB = sl.curve.keys.Length > 0 ? sl.curve.keys.Max(k => k.value) : 0f;
                    startLife = Mathf.Max(maxA, maxB);
                    break;
            }

            // If looping, give a reasonable cap so it returns
            if (main.loop)
            {
                // One full loop cycle plus start lifetime
                max = Mathf.Max(max, duration + startLife);
            }
            else
            {
                max = Mathf.Max(max, duration + startLife);
            }
        }

        // Safety margin so late sub-emitters finish
        return Mathf.Max(0.05f, max + 0.1f);
    }

    private IEnumerator ReturnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Stop everything before returning
        if (systems != null)
        {
            foreach (var ps in systems)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (pool != null)
            pool.Return(gameObject);
        else
            gameObject.SetActive(false); // fallback
    }
}
