using UnityEngine;
using UnityEngine.InputSystem;

public class AttackOnButton : MonoBehaviour
{
    public GameObject bulletPrefab; // Assign in Inspector
    public Transform firePoint;     // Assign in Inspector
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
            FireBullet();
            shoot = false; // Reset to avoid continuous firing
        }
    }

    void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("Missing bulletPrefab or firePoint reference.");
        }
    }
}