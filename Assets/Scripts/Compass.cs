using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Compass : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    public RectTransform m_content;
    public float m_rate = 1.0f;

    float m_curAng = 0.0f;
    bool m_isVisible = true;

    void Start()
    {
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        SetVisible(PlayerPrefs.GetInt("ShowCompass", 1) > 0);
        if (m_isVisible)
        {
            m_text.text = string.Format("{0:0.0}°", Input.compass.trueHeading);
            float ang = Input.compass.trueHeading;
            if (ang > 180.0f)
                ang -= 360.0f;
            float diff = ang - m_curAng;
            if (diff > 180.0f)
                diff -= 360.0f;
            if (diff < -180.0f)
                diff += 360.0f;
            m_curAng += diff * m_rate * Time.deltaTime;
            if (m_curAng > 180.0f)
                m_curAng -= 360.0f;
            if (m_curAng < -180.0f)
                m_curAng += 360.0f;
            float scrollX = -2.0f * m_curAng;
            m_content.anchoredPosition = new Vector2(scrollX, 0);
        }
    }

    void SetVisible(bool isVisible)
    {
        if (isVisible == m_isVisible)
            return;
        foreach (Transform child in transform)
        { 
            child.gameObject.SetActive(isVisible);
        }
        m_isVisible = isVisible;
    }
}
