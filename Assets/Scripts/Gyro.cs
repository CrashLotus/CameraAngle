using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gyro : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }
    public Axis m_axis;

    TextMeshProUGUI m_text;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;   
        m_text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 g = Input.gyro.gravity;
        float len = g.magnitude;
        if (len > 0.01f)
        {
            float dir = 0.0f;
            string txt = "";
            switch (m_axis)
            {
                case Axis.X:
                    txt = "Tilt";
                    dir = g.x;
                    break;
                case Axis.Y:
                    dir = g.y;
                    break;
                case Axis.Z:
                    txt = "Elv";
                    dir = g.z;
                    break;
            }
            float ang = Mathf.Rad2Deg * Mathf.Asin(dir / len);
            m_text.enabled = true;
            m_text.text = string.Format("{0}: {1:0.0}°", txt, ang);
        }
        else
        {
            m_text.enabled = false;
        }
    }
}
