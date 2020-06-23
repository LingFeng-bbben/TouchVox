using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SlideControl : MonoBehaviour
{
    // Start is called before the first frame update
    Text txt;
    void Start()
    {
        Input.multiTouchEnabled = true;
        txt = GameObject.Find("Debug").GetComponent<Text>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        AnalogRequest();
    }

    // Update is called once per frame
    Vector4 leftArea = new Vector4(0, Screen.height*(2f/3f), Screen.width*0.4f, Screen.height);
    Vector4 rightArea = new Vector4(Screen.width * 0.6f, Screen.height * (2f / 3f), Screen.width, Screen.height);
    float rightDelta = 0;
    float leftDelta = 0;
    bool haveRight = false;
    bool haveLeft = false;


    void Update()
    {
        txt.text = "";
        haveRight = false;
        haveLeft = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            txt.text+="Finger id "+ Input.touches[i].fingerId +" pos:"+ Input.touches[i].position.x+","+ Input.touches[i].position.y+"\n";
            if (isInRect(Input.touches[i].position, rightArea))
            {
                txt.text += "RightDelta " + Input.touches[i].deltaPosition.x+"\n";
                rightDelta = Input.touches[i].deltaPosition.x;
                haveRight = true;
            }
            if (isInRect(Input.touches[i].position, leftArea))
            {
                txt.text += "LeftDelta " + Input.touches[i].deltaPosition.x + "\n";
                leftDelta = Input.touches[i].deltaPosition.x;
                haveLeft = true;
            }
        }
        if (!haveLeft) leftDelta = 0f;
        if (!haveRight) rightDelta = 0f;
    }

    float left = 0f;
    float right = 0f;
    async void AnalogRequest()
    {
        while (true)
        {
            float sens = PlayerPrefs.GetFloat("VolSens");
            if (rightDelta != 0 || leftDelta != 0)
            {
                if (rightDelta > 10f) rightDelta = 10f;
                if (rightDelta < -10f) rightDelta = -10f;
                right += rightDelta *0.005f*sens;
                while (right > 1f)
                    right -= 1f;
                while (right < 0f)
                    right += 1f;
                if (leftDelta > 10f) leftDelta = 10f;
                if (leftDelta < -10f) leftDelta = -10f;
                left += leftDelta * 0.005f * sens;
                while (left > 1f)
                    left -= 1f;
                while (left < 0f)
                    left += 1f;
                object[] parax = new object[] { "VOL-R", right };
                object[] paray = new object[] { "VOL-L", left };
                SocketControl.instance.SendRequest(new Request("analogs", "write", new object[] { parax,paray }));
            }
            await Task.Delay(20);
        }
    }

    bool isInRect(Vector2 pos,Vector4 rect)
    {
        if (pos.x >= rect.x && pos.x <= rect.z && pos.y >= rect.y && pos.y <= rect.w)
            return true;
        else
            return false;
    }
}
