using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackOnButton : MonoBehaviour
{
    public SimplePool pool;         // Assign in Inspector
    public Transform firePoint;     // Assign in Inspector
    public Image cooldownOverlay;   // UI overlay (set to radial fill mode)
    public Image icon;              // Bomb icon (optional dimming)

    private bool shoot;

    public void OnShoot(InputValue value)
    {
        shoot = value.isPressed;
    }

    void Update()
    {
        if (shoot)
        {
            // Use StickyBombProjectile.TrySpawn to enforce cooldown
            bool fired = StickyBombProjectile.TrySpawn(pool, firePoint.position, firePoint.rotation);

            if (!fired)
            {
                Debug.Log("Sticky bomb still on cooldown!");
            }

            shoot = false;
        }

        UpdateCooldownUI();
    }

    void UpdateCooldownUI()
    {
        float elapsed = Time.time - StickyBombProjectile.lastFireTime;
        float ratio = Mathf.Clamp01(elapsed / StickyBombProjectile.cooldownTime);

        // Radial overlay fill (1 = ready, 0 = just fired)
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1f - ratio;
        }

        // Optional: Dim icon when not ready
        if (icon != null)
        {
            icon.color = (ratio >= 1f) ? Color.white : Color.gray;
        }
    }
}
