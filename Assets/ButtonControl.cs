using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonControl : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData p)
    {
        object[] para = { gameObject.name, 1.0f };
        SocketControl.instance.SendRequest(new Request("buttons", "write", new object[]{ para }));
    }


    public void OnPointerUp(PointerEventData p)
    {
        object[] para = { gameObject.name, 0f };
        SocketControl.instance.SendRequest(new Request("buttons", "write", new object[] { para }));
    }

}
