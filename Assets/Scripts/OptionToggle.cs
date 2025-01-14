using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionToggle : MonoBehaviour
{
    public string m_option;

    Toggle m_toggle;

    // Start is called before the first frame update
    void Start()
    {
        m_toggle = GetComponent<Toggle>();
        m_toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(m_option, 1) > 0);
        m_toggle.onValueChanged.AddListener(delegate {
            OnValueChanged(m_toggle);
            });
    }

    void OnValueChanged(Toggle change)
    {
        PlayerPrefs.SetInt(m_option, m_toggle.isOn ? 1 : 0);
    }
}
