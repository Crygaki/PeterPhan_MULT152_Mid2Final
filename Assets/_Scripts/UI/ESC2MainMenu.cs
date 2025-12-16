using UnityEngine;

public class ESC2MainMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (UIControl.Instance != null)
            {
                UIControl.Instance.PlayAgain();
            }
            else
            {
                Debug.LogWarning("M pressed but UIControl.Instance is missing.");
            }
        }
    }
}
