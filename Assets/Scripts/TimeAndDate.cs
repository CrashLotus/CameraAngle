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
        DateTime now = DateTime.Now;
        m_date.text = now.ToShortDateString();
        m_time.text = now.ToShortTimeString();
    }
}
