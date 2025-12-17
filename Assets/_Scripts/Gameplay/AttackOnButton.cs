using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class AttackOnButton : MonoBehaviour
{
    [Header("References")]
    public SimplePool pool;
    public Transform firePoint;

    [Header("UI - Left Mouse")]
    public Image cooldownOverlay;
    public Image icon;

    [Header("UI - Right Mouse")]
    public Image ringOverlay;
    public Image ringIcon;
    public TMP_Text ringText;

    private bool shootLeft;
    private bool shootRight;

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

    public void OnShoot(InputValue value)
    {
        shootLeft = value.isPressed;
    }

    public void OnAltShoot(InputValue value)
    {
        shootRight = value.isPressed;
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
                if (!fired)
                {
                    Debug.Log("Not enough RMB cooldown score for ring bomb!");
                }
            }
            else
            {
                Debug.LogWarning("Bomb pool or firePoint not assigned!");
            }
            shootRight = false;
        }

        UpdateCooldownUI();
        UpdateRingUI();
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
        int cooldownScore = ScoreManager.Instance.RmbCooldownScore; // NEW variable in ScoreManager
        int requiredScore = StickyBombProjectile.scoreCooldown;

        float ratio = Mathf.Clamp01((float)cooldownScore / requiredScore);

        if (ringOverlay != null)
            ringOverlay.fillAmount = ratio;

        if (ringIcon != null)
            ringIcon.color = (cooldownScore >= requiredScore) ? Color.white : Color.red;

        if (ringText != null)
            ringText.text = $"RMB: {cooldownScore}/{requiredScore}";
    }

    void HandleScoreChanged(int newScore)
    {
        UpdateRingUI();
    }
}
