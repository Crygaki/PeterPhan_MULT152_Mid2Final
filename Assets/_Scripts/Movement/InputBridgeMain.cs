using UnityEngine;
using UnityEngine.InputSystem;

public class InputBridgeMain : MonoBehaviour
{
    public bool firePressed { get; private set; }

    public bool reloadPressed { get; private set; }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started) firePressed = true;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started) reloadPressed = true;
    }


    void LateUpdate()
    {
        // auto-clear so it's edge-triggered
        firePressed = false;
        reloadPressed = false;
    }
}