using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SimplePool pool;
    public GameObject projectilePrefab; // for non-pooled compare
    public Transform firePoint;
    public bool usePool = true;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            if (usePool){
                var go = pool.Get(firePoint.position, firePoint.rotation);
                go.GetComponent<PooledProjectile>().Init(pool);
            } else {
                var go = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                Destroy(go, 2f);
            }
        }
    }
}