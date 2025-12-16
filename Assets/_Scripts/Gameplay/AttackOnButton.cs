using UnityEngine;
using UnityEngine.InputSystem;

public class AttackOnButton : MonoBehaviour
{
    public SimplePool pool;         // Assign in Inspector
    public Transform firePoint;     // Assign in Inspector
    private bool shoot;

    public void OnShoot(InputValue value)
    {
        shoot = value.isPressed;
    }

    void Update()
    {
        if (shoot)
        {
            var go = pool.Get(firePoint.position, firePoint.rotation);
            if (go != null)
            {
                var bomb = go.GetComponent<StickyBombProjectile>();
                if (bomb != null)
                {
                    bomb.Init(pool); // pooled sticky bomb
                }
            }
            shoot = false;
        }
    }
}
