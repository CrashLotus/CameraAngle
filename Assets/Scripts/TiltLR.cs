using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltLR : MonoBehaviour
{
    public float m_tiltRatio = 1.0f;
    public float m_elvRatio = 30.0f;

    // Update is called once per frame
    void Update()
    {
        Manager manager = Manager.Get();
        float ang = manager.GetTilt();
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang * m_tiltRatio);
        RectTransform rect = transform as RectTransform;
        ang = manager.GetElevation();
        rect.anchoredPosition = new Vector2(0.0f, ang * m_elvRatio);
    }
}
