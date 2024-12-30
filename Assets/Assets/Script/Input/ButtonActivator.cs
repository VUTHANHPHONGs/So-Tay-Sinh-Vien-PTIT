using UnityEngine;
using UnityEngine.UI;

public class ButtonActivator : MonoBehaviour
{
    public Button myButton; // G�n button t? Inspector
    public Button myButton1; // G�n button t? Inspector
    public Button myButton2; // G�n button t? Inspector
    public Button myButton3; // G�n button t? Inspector

    public bool IsUIShow = false;

    public KeyCode keyToPress = KeyCode.Space; // Ph�m b?n mu?n s? d?ng ?? k�ch ho?t button

    void Update()
    {
        if (!IsUIShow)
        {
            // Ki?m tra xem ph�m c� ???c nh?n hay kh�ng
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // G?i h�m OnClick c?a button khi ph�m ???c nh?n
                myButton.onClick.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // G?i h�m OnClick c?a button khi ph�m ???c nh?n
                myButton3.onClick.Invoke();
            }
        }
       
        if (Input.GetKeyDown(KeyCode.O))
        {
            // G?i h�m OnClick c?a button khi ph�m ???c nh?n
            myButton1.onClick.Invoke();

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // G?i h�m OnClick c?a button khi ph�m ???c nh?n
            myButton2.onClick.Invoke();
        }
       
    }

    public void ISShow()
    {
        IsUIShow = true;
    }
    public void ISShow2()
    {
        IsUIShow = false;
    }
}
