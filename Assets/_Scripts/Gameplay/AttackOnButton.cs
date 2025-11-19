using UnityEngine;
using UnityEngine.InputSystem;

public class AttackOnButton : MonoBehaviour
{
    public SimplePool pool;         // Assign in Inpector
    public GameObject bulletPrefab; // Assign in Inspector
    public Transform firePoint;     // Assign in Inspector
    public bool usePool = true;
    private bool shoot;

    // This method is automatically called by PlayerInput (SendMessage mode)
    public void OnShoot(InputValue value)
    {
        shoot = value.isPressed;
    }

    void Update()
    {
        if (shoot)
        {
            if (usePool)
            {
                var go = pool.Get(firePoint.position, firePoint.rotation);
                go.GetComponent<PooledProjectile>().Init(pool);
            }
            else
            {
                FireBullet();
            }
            shoot = false; // Reset to avoid continuous firing
        }
    }

    void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            var go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Destroy(go, 10f);
        }
        else
        {
            Debug.LogWarning("Missing bulletPrefab or firePoint reference.");
        }
    }
}