using UnityEngine;
using StarterAssets;

public class MYUIManager : MonoBehaviour
{
    public GameObject instructionPanel; // Holds instruction text + Play button
    public GameObject gameplayRoot;     // Root object to activate gameplay
    public StarterAssets.StarterAssetsInputs starterInputs; // Drag your Player here in Inspector

    void Start()
    {
        // Disable movement input at start
        starterInputs.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        Debug.Log("StartGame button clicked!");

        instructionPanel.SetActive(false);
        gameplayRoot.SetActive(true);

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable movement input when game begins
        starterInputs.enabled = true;
    }
}