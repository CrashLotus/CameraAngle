using System.Collections;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public float m_maxZoom = 5.0f;
    public bool m_canSlide = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(UpdateCo());
    }

    IEnumerator UpdateCo()
    {
        while (true)
        {
            // wait until the user is touching with 1 fingers
            while (Input.touchCount < 1)
            {
                yield return null;
            }
            Vector2 startTouch = Input.touches[0].position;
            Vector3 startPos = transform.position;

            // wait until the user is touching with 2 fingers
            while (Input.touchCount == 1)
            {
                if (m_canSlide)
                {
                    Vector2 delta = Input.touches[0].position - startTouch;
                    Vector3 pos = startPos;
                    pos.x += delta.x;
                    pos.y += delta.y;
                    transform.position = pos;
                }
                yield return null;
            }

            if (Input.touchCount > 1)
            {
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
}
