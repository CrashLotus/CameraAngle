using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gyro : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,  // not supported
        Z
    }
    public Axis m_axis;

    TextMeshProUGUI m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 g = Input.gyro.gravity;
        float len = g.magnitude;
        if (len > 0.01f)
        {
            float ang = 0.0f;
            switch (m_axis)
            {
                case Axis.X:
                    ang = Manager.Get().GetTilt();
                    break;
                case Axis.Z:
                    ang = Manager.Get().GetElevation();
                    break;
            }
            m_text.enabled = true;
            m_text.text = string.Format("{0:0.0}°", ang);
        }
        else
        {
            m_text.enabled = false;
        }
    }
}
