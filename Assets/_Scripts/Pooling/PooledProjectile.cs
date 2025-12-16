using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float life = 2f;

    private SimplePool pool;
    private float t;

    public void Init(SimplePool p)
    {
        pool = p;
        t = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!gameObject.activeSelf || pool == null) return;

        transform.position += transform.forward * speed * Time.deltaTime;
        t += Time.deltaTime;

        if (t >= life)
        {
            pool.Return(gameObject); // recycle bullet
        }
    }

    void OnCollisionEnter(Collision _)
    {
        pool.Return(gameObject); // recycle bullet on hit
    }
}
