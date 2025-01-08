using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltLR : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        float ang = Mathf.Rad2Deg * Mathf.Asin(Input.gyro.gravity.x / Input.gyro.gravity.magnitude);
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, ang);
    }
}
