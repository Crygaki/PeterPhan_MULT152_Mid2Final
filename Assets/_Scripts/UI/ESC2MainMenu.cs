using UnityEngine;

public class ESC2MainMenu : MonoBehaviour
{
    private UIControl uiControl;

    void Start()
    {
        // Find the UIControl component in the scene
        uiControl = FindFirstObjectByType<UIControl>();

        if (uiControl == null)
        {
            Debug.LogWarning("UIControl not found in scene!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiControl != null)
            {
                uiControl.PlayAgain();
            }
            else
            {
                Debug.LogWarning("ESC pressed but UIControl is missing.");
            }
        }
    }
}