using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class CalibrateButton : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateText();   
    }

    public void UpdateText()
    {
        bool isCalibrated = PlayerPrefs.GetInt("IsCalibrated", 0) != 0;
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            if (isCalibrated)
            {
                text.text = PlayerPrefs.GetString("CalibrationDate", "Unknown");
            }
            else
            {
                text.text = "Not Calibrated";
            }
        }
    }
}
