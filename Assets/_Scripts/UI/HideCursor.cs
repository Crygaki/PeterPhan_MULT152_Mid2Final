using UnityEngine;

public class HideCursor : MonoBehaviour
{
    void Start()
    {
        // Hide the cursor
        Cursor.visible = false;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }
}
