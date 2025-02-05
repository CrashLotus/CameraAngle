using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class CalibrateButton : MonoBehaviour
{
    public TextMeshProUGUI m_dateText;

    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateText();   
    }

    public void UpdateText()
    {
        bool isCalibrated = PlayerPrefs.GetInt("IsCalibrated", 0) != 0;
        if (m_dateText != null)
        {
            if (isCalibrated)
            {
                float calAng = PlayerPrefs.GetFloat("ElvCalibrate", 0.0f);
                m_dateText.text = string.Format("{0} {1:0.00}°", PlayerPrefs.GetString("CalibrationDate", "Unknown"), calAng);
            }
            else
            {
                m_dateText.text = "Not Calibrated";
            }
        }
    }
}
