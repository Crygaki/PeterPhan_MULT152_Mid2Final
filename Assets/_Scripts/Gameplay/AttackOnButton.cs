using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class AttackOnButton : MonoBehaviour
{
    [Header("References")]
    public SimplePool pool;          // Bomb pool (assign in Inspector)
    public Transform firePoint;      // Where bombs spawn

    [Header("UI - Left Mouse")]
    public Image cooldownOverlay;    // For LMB time cooldown
    public Image icon;               // Bomb icon (optional dimming)

    [Header("UI - Right Mouse")]
    public Image ringOverlay;        // Overlay fill for RMB score cooldown
    public Image ringIcon;           // Separate icon for RMB
    public TMP_Text ringText;        // Text showing score requirement

    private bool shootLeft;
    private bool shootRight;

    // Track last time RMB was fired so we can reset overlay
    private bool ringJustFired = false;

    // --- Input System Callbacks (Send Messages mode) ---
    public void OnShoot(InputValue value)   // Left mouse
    {
        shootLeft = value.isPressed;
    }

    public void OnAltShoot(InputValue value) // Right mouse
    {
        shootRight = value.isPressed;
    }

    void OnEnable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged.AddListener(HandleScoreChanged);
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged.RemoveListener(HandleScoreChanged);
    }

    void Update()
    {
        if (shootLeft)
        {
            if (pool != null && firePoint != null)
            {
                bool fired = StickyBombProjectile.TrySpawn(pool, firePoint.position, firePoint.rotation, false);
                if (!fired) Debug.Log("Sticky bomb still on cooldown!");
            }
            else
            {
                Debug.LogWarning("Bomb pool or firePoint not assigned!");
            }
            shootLeft = false;
        }

        if (shootRight)
        {
            if (pool != null && firePoint != null)
            {
                bool fired = StickyBombProjectile.TrySpawn(pool, firePoint.position, firePoint.rotation, true);
                if (fired)
                {
                    // Reset overlay when ring bomb is fired
                    ringJustFired = true;
                }
                else
                {
                    Debug.Log("Not enough score for ring bomb!");
                }
            }
            else
            {
                Debug.LogWarning("Bomb pool or firePoint not assigned!");
            }
            shootRight = false;
        }

        UpdateCooldownUI(); // LMB time cooldown
        UpdateRingUI();     // RMB score cooldown
    }

    void UpdateCooldownUI()
    {
        float elapsed = Time.time - StickyBombProjectile.lastFireTime;
        float ratio = Mathf.Clamp01(elapsed / StickyBombProjectile.timeCooldown);

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f - ratio;

        if (icon != null)
            icon.color = (ratio >= 1f) ? Color.white : Color.gray;
    }

    void UpdateRingUI()
    {
        if (ScoreManager.Instance == null) return;

        int currentScore = ScoreManager.Instance.CurrentScore;
        int requiredScore = StickyBombProjectile.scoreCooldown;

        float ratio = Mathf.Clamp01((float)currentScore / requiredScore);

        if (ringOverlay != null)
        {
            if (ringJustFired)
            {
                // Drain overlay immediately when fired
                ringOverlay.fillAmount = 0f;
                ringJustFired = false;
            }
            else
            {
                // Fill overlay based on score progress
                ringOverlay.fillAmount = ratio;
            }
        }

        if (ringIcon != null)
            ringIcon.color = (currentScore >= requiredScore) ? Color.white : Color.red;

        if (ringText != null)
            ringText.text = $"Score: {currentScore}/{requiredScore}";
    }

    void HandleScoreChanged(int newScore)
    {
        UpdateRingUI();
    }
}
