using UnityEngine;

public class Apple : MonoBehaviour
{
    void Update()
    {
        // If apple falls below a certain Y position, it's missed
        if (transform.position.y < -20f)
        {
            ApplePicker apScript = FindFirstObjectByType<ApplePicker>();
            if (apScript != null)
            {
                apScript.AppleMissed(this.gameObject); // Pass the apple that was missed
            }
            Destroy(gameObject);
        }
    }
}