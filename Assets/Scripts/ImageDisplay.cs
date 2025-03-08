using System.Collections;
using UnityEngine;

public class ImageDisplay : MonoBehaviour
{
    public GameObject m_screenShot;
    public GameObject m_fullImage;
    public float m_slideTime = 0.25f;

    enum State
    {
        SCREENSHOT,
        FULL
    }
    State m_state = State.SCREENSHOT;
    bool m_isSliding = false;

    private void Awake()
    {
        Swiper swiper = GetComponent<Swiper>();
        swiper.SwipeUpEvent += OnSwipeUp;
        swiper.SwipeDownEvent += OnSwipeDown;
        swiper.SwipeLeftEvent += OnSwipeLeft;
        swiper.SwipeRightEvent += OnSwipeRight;
    }

    private void OnEnable()
    {
        SetState(State.FULL);
    }

    void SetState(State newState)
    {
        if (newState != m_state)
        {
            switch (newState)
            {
                case State.SCREENSHOT:
                    m_screenShot.SetActive(true);
                    m_fullImage.SetActive(false);
                    break;
                case State.FULL:
                    m_screenShot.SetActive(false);
                    m_fullImage.SetActive(true);
                    break;
            }
            m_state = newState;
        }
    }

    void OnSwipeLeft()
    {
        if (m_isSliding)
            return;
        if (m_state == State.SCREENSHOT)
            SetState(State.FULL);
        else
            SetState(State.SCREENSHOT);
        StartCoroutine(Slide(1.0f));
    }

    void OnSwipeRight()
    {
        if (m_isSliding)
            return;
        if (m_state == State.SCREENSHOT)
            SetState(State.FULL);
        else
            SetState(State.SCREENSHOT);
        StartCoroutine(Slide(-1.0f));
    }

    void OnSwipeUp()
    {
        gameObject.SetActive(false);
    }

    void OnSwipeDown()
    {
        gameObject.SetActive(false);
    }

    IEnumerator Slide(float dir)
    {
        m_isSliding = true;
        Vector3 end = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0.0f);
        Vector3 start = end + dir * new Vector3(Screen.width, 0.0f, 0.0f);
        GameObject obj = m_state == State.SCREENSHOT ? m_screenShot : m_fullImage;
        float m_time = m_slideTime;
        while (m_time > 0.0f)
        {
            m_time -= Time.deltaTime;
            float lerp = Mathf.Clamp01(m_time / m_slideTime);
            obj.transform.position = Vector3.Lerp(end, start, lerp);
            yield return null;
        }
        m_isSliding = false;
    }
}
