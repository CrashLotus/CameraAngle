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
        m_isSliding = false;
        m_state = State.FULL;
        m_screenShot.SetActive(true);
        m_fullImage.SetActive(true);
    }

    void OnSwipeLeft()
    {
        if (m_isSliding)
            return;
        GameObject oldImage;
        GameObject newImage;
        if (m_state == State.SCREENSHOT)
        {
            oldImage = m_screenShot;
            newImage = m_fullImage;
            m_state = State.FULL;
        }
        else
        {
            oldImage = m_fullImage;
            newImage = m_screenShot;
            m_state = State.SCREENSHOT;
        }
        StartCoroutine(SlideLeftRight(1.0f, oldImage, newImage));
    }

    void OnSwipeRight()
    {
        if (m_isSliding)
            return;
        GameObject oldImage;
        GameObject newImage;
        if (m_state == State.SCREENSHOT)
        {
            oldImage = m_screenShot;
            newImage = m_fullImage;
            m_state = State.FULL;
        }
        else
        {
            oldImage = m_fullImage;
            newImage = m_screenShot;
            m_state = State.SCREENSHOT;
        }
        StartCoroutine(SlideLeftRight(-1.0f, oldImage, newImage));
    }

    void OnSwipeUp()
    {
        if (m_isSliding)
            return;
        GameObject oldImage;
        GameObject newImage;
        if (m_state == State.SCREENSHOT)
        {
            oldImage = m_screenShot;
            newImage = m_fullImage;
            m_state = State.FULL;
        }
        else
        {
            oldImage = m_fullImage;
            newImage = m_screenShot;
            m_state = State.SCREENSHOT;
        }
        newImage.SetActive(false);
        StartCoroutine(SlideUpDown(-1.0f, oldImage));
    }

    void OnSwipeDown()
    {
        if (m_isSliding)
            return;
        GameObject oldImage;
        GameObject newImage;
        if (m_state == State.SCREENSHOT)
        {
            oldImage = m_screenShot;
            newImage = m_fullImage;
            m_state = State.FULL;
        }
        else
        {
            oldImage = m_fullImage;
            newImage = m_screenShot;
            m_state = State.SCREENSHOT;
        }
        newImage.SetActive(false);
        StartCoroutine(SlideUpDown(1.0f, oldImage));

    }

    IEnumerator SlideLeftRight(float dir, GameObject oldImage, GameObject newImage)
    {
        float width = Display.main.systemWidth;
        float height = Display.main.systemHeight;
        Debug.Log("width = " + width + " height = " + height);
        Debug.Log("Screen.width = " + Screen.width + " Screen.height = " + Screen.height);
        m_isSliding = true;
        oldImage.SetActive(true);
        newImage.SetActive(true);
        Vector3 mid = new Vector3(0.5f * width, 0.5f * height, 0.0f);
        Vector3 from = mid + dir * new Vector3(width, 0.0f, 0.0f);
        Vector3 to = mid - dir * new Vector3(width, 0.0f, 0.0f);
        float m_time = m_slideTime;
        while (m_time > 0.0f)
        {
            m_time -= Time.deltaTime;
            float lerp = Mathf.Clamp01(m_time / m_slideTime);
            oldImage.transform.position = Vector3.Lerp(to, mid, lerp);
            newImage.transform.position = Vector3.Lerp(mid, from, lerp);
            yield return null;
        }
        m_isSliding = false;
    }

    IEnumerator SlideUpDown(float dir, GameObject oldImage)
    {
        m_isSliding = true;
        oldImage.SetActive(true);
        Vector3 mid = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0.0f);
        Vector3 to = mid - dir * new Vector3(0.0f, Screen.height, 0.0f);
        float m_time = m_slideTime;
        while (m_time > 0.0f)
        {
            m_time -= Time.deltaTime;
            float lerp = Mathf.Clamp01(m_time / m_slideTime);
            oldImage.transform.position = Vector3.Lerp(to, mid, lerp);
            yield return null;
        }
        m_isSliding = false;
        gameObject.SetActive(false);
    }
}
