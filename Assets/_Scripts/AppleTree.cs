using UnityEngine;

public class AppleTree : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject applePrefab;       // Normal apple
    public GameObject goldApplePrefab;   // Gold apple
    public GameObject poisonApplePrefab; // Poison apple

    public float speed = 1f;
    public float leftAndRightEdge = 10f;
    public float changeDirChance = 0.001f;
    public float appleDropDelay = 1f;

    void Start()
    {
        InvokeRepeating("DropApple", 2f, appleDropDelay);
    }

    void DropApple()
    {
        GameObject prefabToDrop = ChooseApplePrefab();

        if (prefabToDrop != null)
        {
            Instantiate(prefabToDrop, transform.position, Quaternion.identity);

            // --- Play Apple Drop SFX ---
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayAppleDropSfx();
            }
        }
        else
        {
            Debug.LogError("No apple prefab chosen!");
        }
    }

    GameObject ChooseApplePrefab()
    {
        // Random value between 0 and 1
        float rand = Random.value;

        if (rand < 0.7f)
        {
            // 70% chance
            return applePrefab;
        }
        else if (rand < 0.8f)
        {
            // Next 10% (0.7–0.8)
            return goldApplePrefab;
        }
        else
        {
            // Remaining 20% (0.8–1.0)
            return poisonApplePrefab;
        }
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;

        if (pos.x < -leftAndRightEdge)
        {
            speed = Mathf.Abs(speed);
        }
        else if (pos.x > leftAndRightEdge)
        {
            speed = -Mathf.Abs(speed);
        }
    }

    void FixedUpdate()
    {
        if (Random.value < changeDirChance)
        {
            speed *= -1;
        }
    }
}
