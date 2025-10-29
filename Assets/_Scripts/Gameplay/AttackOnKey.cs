using UnityEngine;
using UnityEngine.InputSystem;

public class AttackOnKey : MonoBehaviour
{
    [SerializeField] private HealthComponent target;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private Key key = Key.F;

    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        //The following is for assigning a keyboard key in the Inspector
        if (Keyboard.current[key].wasPressedThisFrame)
        {
            target?.Damage(damageAmount);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}