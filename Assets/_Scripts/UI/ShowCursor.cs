using UnityEngine;

public class ShowCursor : MonoBehaviour
{
    void Start()
    {
        // Make the cursor visible
        Cursor.visible = true;

        // Unlock the cursor so it can move freely
        Cursor.lockState = CursorLockMode.None;
    }
}
