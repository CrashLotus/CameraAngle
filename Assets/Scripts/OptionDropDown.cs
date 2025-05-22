using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionDropDown : MonoBehaviour
{
    public string m_option;
    TMP_Dropdown m_dropdown;

    // Start is called before the first frame update
    void Start()
    {
        m_dropdown = GetComponent<TMP_Dropdown>();
        int value = PlayerPrefs.GetInt(m_option, 0);
        m_dropdown.value = value;
        m_dropdown.onValueChanged.AddListener(delegate
        {
            OnValueChanged(m_dropdown);
        });
    }

    void OnValueChanged(TMP_Dropdown change)
    {
        PlayerPrefs.SetInt(m_option, m_dropdown.value);
    }
}
