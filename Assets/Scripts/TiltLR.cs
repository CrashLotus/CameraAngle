using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltLR : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log("TiltLR Update");
        Manager manager = Manager.Get();
        float ang = manager.GetTilt();
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
        RectTransform rect = transform as RectTransform;
        ang = manager.GetElevation();
        rect.anchoredPosition = new Vector2(0.0f, ang * 30.0f);
        Debug.Log("TiltLR Done");
    }
}
