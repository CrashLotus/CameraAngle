using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeAndDate : MonoBehaviour
{
    public TextMeshProUGUI m_date;
    public TextMeshProUGUI m_time;

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("ShowDate", 1) > 0)
        {
            DateTime now = DateTime.Now;
            m_date.gameObject.SetActive(true);
            m_date.text = now.ToShortDateString();
            m_time.gameObject.SetActive(true);
            m_time.text = now.ToShortTimeString();
        }
        else
        {
            m_date.gameObject.SetActive(false);
            m_time.gameObject.SetActive(false);
        }
    }
}
