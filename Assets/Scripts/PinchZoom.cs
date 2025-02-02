using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public float m_maxZoom = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateCo());        
    }

    IEnumerator UpdateCo()
    {
        while (true)
        {
            // wait until the user is touching with 2 fingers
            while (Input.touchCount < 2)
            {
                yield return null;
            }

            // initial pinch dist
            float dist = (Input.touches[1].position - Input.touches[0].position).magnitude;
            float origScale = transform.localScale.x;
            yield return null;

            // while the user is pinching
            while (Input.touchCount > 1)
            {
                float newDist = (Input.touches[1].position - Input.touches[0].position).magnitude;
                float ratio = origScale * newDist / dist;
                ratio = Mathf.Clamp(ratio, 1.0f, m_maxZoom);
                transform.localScale = new Vector3(ratio, ratio, ratio);
                yield return null;
            }
        }
    }
}
