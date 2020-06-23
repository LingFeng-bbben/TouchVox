using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Toggle ToggleTop;
    public Toggle ToggleFull;
    public InputField ipInput;
    public Slider VolSlider;
    public Button ButtonClose;


    // Start is called before the first frame update
    void Start()
    {
        ButtonClose.onClick.AddListener(OnPanelClose);
#if !(UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        ToggleFull.enabled = false;
        ToggleTop.enabled = false;
#endif
        OnPanelOpen();
    }

    void OnPanelOpen()
    {
        if (!PlayerPrefs.HasKey("IP")) return;
        ToggleTop.isOn = PlayerPrefs.GetInt("AlwaysTop") > 0 ? true : false;
        ToggleFull.isOn = PlayerPrefs.GetInt("FullScreen") > 0 ? true : false;
        ipInput.text = PlayerPrefs.GetString("IP");
        VolSlider.value = PlayerPrefs.GetFloat("VolSens");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        TopAlways.instance.AssignTopmostWindow(ToggleTop.isOn);
        Screen.fullScreen = ToggleFull.isOn;
#endif

    }

    void OnPanelClose()
    {
        PlayerPrefs.SetInt("AlwaysTop", ToggleTop.isOn ? 1 : 0);
        PlayerPrefs.SetInt("FullScreen", ToggleFull.isOn ? 1 : 0);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        TopAlways.instance.AssignTopmostWindow(ToggleTop.isOn);
        Screen.fullScreen = ToggleFull.isOn;
#endif

        PlayerPrefs.SetString("IP", ipInput.text);
        PlayerPrefs.SetFloat("VolSens", VolSlider.value);
        PlayerPrefs.Save();
        SocketControl.instance.Connect(ipInput.text);
        gameObject.SetActive(false);
    }
}
