using UnityEngine;

public class MouseManager : MonoBehaviour
{
    void Awake()
    {
        // ?n con tr? chu?t khi b?t ??u game
        Cursor.visible = false;
        // Kh�a con tr? chu?t ? gi?a m�n h�nh
        Cursor.lockState = CursorLockMode.Locked;
    }

    // H�m ?? hi?n th? l?i con tr? chu?t khi c?n
    public void ShowCursor()
    {
        // Hi?n th? con tr? chu?t
        Cursor.visible = true;
        // M? kh�a con tr? chu?t
        Cursor.lockState = CursorLockMode.None;
    }

    // H�m ?? ?n l?i con tr? chu?t
    public void HideCursor()
    {
        // ?n con tr? chu?t
        Cursor.visible = false;
        // Kh�a con tr? chu?t
        Cursor.lockState = CursorLockMode.Locked;
    }
}
