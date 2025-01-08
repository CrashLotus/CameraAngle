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
        float dir = 0.0f;
        switch (m_axis)
        {
            case Axis.X:
                dir = Input.gyro.gravity.x;
                break;
            case Axis.Y:
                dir = Input.gyro.gravity.y;
                break;
            case Axis.Z:
                dir = Input.gyro.gravity.z;
                break;
        }
        float ang = Mathf.Rad2Deg * Mathf.Asin(dir / Input.gyro.gravity.magnitude);
        m_text.text = string.Format("{0:0.0}", ang);
    }
}
