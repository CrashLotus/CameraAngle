using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class GPS : MonoBehaviour
{
    public TextMeshProUGUI m_lat;
    public TextMeshProUGUI m_lng;
    public TextMeshProUGUI m_alt;

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("ShowGPS", 1) > 0 && Input.location.status == LocationServiceStatus.Running)
        {
            m_lat.gameObject.SetActive(true);
            float lat = Input.location.lastData.latitude;
            if (lat < 0.0f)
                m_lat.text = string.Format("{0:0.0000}° S", -lat);
            else
                m_lat.text = string.Format("{0:0.0000}° N", lat);
            m_lng.gameObject.SetActive(true);
            float lng = Input.location.lastData.longitude;
            if (lng < 0.0f)
                m_lng.text = string.Format("{0:0.0000}° W", -lng);
            else
                m_lng.text = string.Format("{0:0.0000}° E", lng);
            m_alt.gameObject.SetActive(true);
            m_alt.text = string.Format("{0:0} m", Input.location.lastData.altitude);
        }
        else
        {
            m_lat.gameObject.SetActive(false);
            m_lng.gameObject.SetActive(false);
            m_alt.gameObject.SetActive(false);
        }
    }
}
