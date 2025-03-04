using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageDisplay : MonoBehaviour
{
    public RawImage m_rawImage;
    public TextMeshProUGUI m_date;
    public float m_slideTime = 0.25f;

    Texture2D m_texDisplay;
    int m_dateIndex = 0;
    int m_fileIndex = 0;
    List<string> m_dates;
    List<string> m_files;

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
        if (null != m_texDisplay)
        {
            Destroy(m_texDisplay);
            m_texDisplay = null;
        }

        m_dateIndex = -1;
        m_fileIndex = -1;
        LoadImage();
    }

    void LoadImage()
    { 
        // get the most recent file
        SaveData save = SaveData.Get();
        m_dates = save.GetDates();
        if (null != m_dates && m_dates.Count > 0)
        {
            if (m_dateIndex < 0 || m_dateIndex >= m_dates.Count)
            {
                m_dateIndex = m_dates.Count - 1;
                m_fileIndex = -1;
            }
            string date = m_dates[m_dateIndex];
            m_files = save.GetFiles(date);
            if (null != m_files && m_files.Count > 0)
            {
                if (m_fileIndex < 0 || m_fileIndex >= m_files.Count)
                {
                    m_fileIndex = m_files.Count - 1;
                }
                string file = m_files[m_fileIndex];
                m_texDisplay = NativeGallery.LoadImageAtPath(file);
                if (null == m_texDisplay)
                {
                    Debug.LogError("Unable to load file " + file);
                    return;
                }
                m_rawImage.texture = m_texDisplay;
                if (null != m_date)
                {
                    m_date.text = Path.GetFileName(file);
                }
            }
        }
    }

    void OnDisable()
    {
        Destroy(m_texDisplay);
        m_texDisplay = null;
    }

    void OnSwipeLeft()
    {
        if (SwipeLeftRight(-1))
            StartCoroutine(SlideScreen(new Vector3(-1.0f, 0.0f, 0.0f)));
    }

    void OnSwipeRight()
    {
        if (SwipeLeftRight(1))
            StartCoroutine(SlideScreen(new Vector3(1.0f, 0.0f, 0.0f)));
    }

    void OnSwipeUp()
    {
        if (SwipeUpDown(-1))
            StartCoroutine(SlideScreen(new Vector3(0.0f, 1.0f, 0.0f)));
    }

    void OnSwipeDown()
    {
        if (SwipeUpDown(1))
            StartCoroutine(SlideScreen(new Vector3(0.0f, -1.0f, 0.0f)));
    }

    bool SwipeLeftRight(int dir)
    {
        if (null == m_files || m_files.Count < 1)
        {
            return false;
        }
        int oldIndex = m_fileIndex;
        m_fileIndex += dir;
        m_fileIndex = Mathf.Clamp(m_fileIndex, 0, m_files.Count - 1);
        if (oldIndex != m_fileIndex)
        {
            return true;
        }
        return false;
    }

    bool SwipeUpDown(int dir)
    {
        if (null == m_dates || m_dates.Count < 1)
        {
            return false;
        }
        int oldIndex = m_dateIndex;
        m_dateIndex += dir;
        m_dateIndex = Mathf.Clamp(m_dateIndex, 0, m_dates.Count - 1);
        if (oldIndex != m_dateIndex)
        {
            m_fileIndex = -1;
            return true;
        }
        return false;
    }

    IEnumerator SlideScreen(Vector3 dir)
    {
        Vector3 startPos = new Vector3(
            0.5f * Screen.width,
            0.5f * Screen.height,
            0.0f
            );
        Vector3 endPos = startPos;
        endPos += Vector3.Scale(dir, new Vector3(Screen.width, Screen.height, 0.0f));

        float timer = 0.0f;
        while (timer < m_slideTime)
        {
            float lerp = timer / m_slideTime;
            m_rawImage.transform.position = Vector3.Lerp(startPos, endPos, lerp);

            timer += Time.deltaTime;
            yield return null;
        }

        LoadImage();
        endPos = startPos;
        startPos -= Vector3.Scale(dir, new Vector3(Screen.width, Screen.height, 0.0f));
        
        timer = 0.0f;
        while (timer < m_slideTime)
        {
            float lerp = timer / m_slideTime;
            m_rawImage.transform.position = Vector3.Lerp(startPos, endPos, lerp);

            timer += Time.deltaTime;
            yield return null;
        }

        m_rawImage.transform.position = endPos;
    }
}
