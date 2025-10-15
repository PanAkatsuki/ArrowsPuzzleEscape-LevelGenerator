using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDebugger : MonoBehaviour
{
    [Header("Debugger Setting")]
    [SerializeField]
    private bool isDebuggerOn = true;

    private void Update()
    {
        if (!isDebuggerOn)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
                Debug.Log("Clicked: " + hit.collider.name);
            else
                Debug.Log("No collider hit.");
        }
    }
}
