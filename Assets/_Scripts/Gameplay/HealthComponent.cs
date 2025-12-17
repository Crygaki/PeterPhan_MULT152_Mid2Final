using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int startHealth = 100;
    [SerializeField] private HealthConfig config; // drag an SO asset here

    [Header("UI (optional)")]
    [SerializeField] private UITK_HUD hud;
    [SerializeField] private RestartUI restartUI; // Assign in scene or auto-find

    [Header("Gameplay")]
    [SerializeField] private bool drainHealthOverTime = true;
    [SerializeField] private float drainInterval = 1f; // seconds per tick
    [SerializeField] private int drainAmount = 1;      // base drain per tick

    [Header("Collision Damage")]
    [SerializeField] private int collisionDamage = 1;   // base damage
    [SerializeField] private float collisionCooldown = 1f; // seconds between hits
    private float lastCollisionTime;

    public int Current { get; private set; }
    public bool IsDead => Current <= 0;

    // Events (publisher)
    public event Action<int, int> OnHealthChanged; // current, max
    public event Action<int> OnDamaged;           // amount
    public event Action<int> OnHealed;            // amount
    public event Action OnDied;

    private float drainTimer;
    private int lastScore = 0;

    private void Awake()
    {
        int max = config != null ? config.maxHealth : maxHealth;
        int start = config != null ? config.startHealth : startHealth;

        maxHealth = max;
        Current = Mathf.Clamp(start, 0, maxHealth);

        if (!restartUI) restartUI = RestartUI.Instance;
        if (!restartUI) restartUI = UnityEngine.Object.FindAnyObjectByType<RestartUI>(FindObjectsInactive.Include);

        RaiseChanged();
    }

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.AddListener(HandleScoreChanged);
            lastScore = ScoreManager.Instance.CurrentScore; // initialize baseline
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.RemoveListener(HandleScoreChanged);
        }
    }

    private void Update()
    {
        if (!drainHealthOverTime || IsDead) return;

        drainTimer += Time.deltaTime;
        if (drainTimer >= drainInterval)
        {
            drainTimer = 0f;
            Damage(GetDrainAmount());
        }
    }

    // --- Event handler: replenish health when score changes ---
    private void HandleScoreChanged(int newScore)
    {
        int delta = newScore - lastScore;
        if (delta > 0)
        {
            int healAmount = delta;

            if (ScoreManager.Instance != null)
            {
                switch (ScoreManager.Instance.currentMode)
                {
                    case DifficultyMode.Hard:
                        healAmount = delta * 2;
                        break;
                    case DifficultyMode.Extreme:
                        healAmount = delta * 4;
                        break;
                        // Easy stays 1:1
                }
            }

            Heal(healAmount);
        }
        lastScore = newScore;
    }

    // --- Collision damage with cooldown and difficulty scaling ---
    private void OnCollisionEnter(Collision collision) => TryApplyCollisionDamage(collision.gameObject);
    private void OnTriggerEnter(Collider other) => TryApplyCollisionDamage(other.gameObject);

    private void TryApplyCollisionDamage(GameObject obj)
    {
        if (IsDead) return;

        if (obj.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (Time.time - lastCollisionTime >= collisionCooldown)
            {
                int totalDamage = GetCollisionDamage(obj);
                Damage(totalDamage);
                lastCollisionTime = Time.time;
            }
        }
    }

    private int GetDrainAmount()
    {
        int amount = drainAmount;

        if (ScoreManager.Instance != null)
        {
            switch (ScoreManager.Instance.currentMode)
            {
                case DifficultyMode.Easy: amount += 1; break;
                case DifficultyMode.Hard: amount += 2; break;
                case DifficultyMode.Extreme: amount += 4; break;
            }
        }

        return amount;
    }

    private int GetCollisionDamage(GameObject obj)
    {
        int totalDamage = collisionDamage;

        if (obj.CompareTag("Boss")) totalDamage += 3;
        else if (obj.CompareTag("MinionCapsule") || obj.CompareTag("MinionCylinder")) totalDamage += 2;
        else if (obj.CompareTag("MinionCube") || obj.CompareTag("MinionSphere")) totalDamage += 1;

        if (ScoreManager.Instance != null)
        {
            switch (ScoreManager.Instance.currentMode)
            {
                case DifficultyMode.Hard: totalDamage *= 2; break;
                case DifficultyMode.Extreme: totalDamage *= 3; break;
            }
        }

        return totalDamage;
    }

    public void Damage(int amount)
    {
        if (amount <= 0 || IsDead) return;
        Current = Mathf.Max(0, Current - amount);
        OnDamaged?.Invoke(amount);
        RaiseChanged();

        if (IsDead) Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || Current <= 0) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        OnHealed?.Invoke(amount);
        RaiseChanged();
    }

    private void Die()
    {
        OnDied?.Invoke();
        GameManager.instance?.EndGame();
        SceneManager.LoadScene("GameOverScene");
    }

    private void RaiseChanged()
    {
        OnHealthChanged?.Invoke(Current, maxHealth);
        if (hud != null) hud.SetHP(Current, maxHealth);
    }
}
