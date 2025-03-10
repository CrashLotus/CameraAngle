using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public float m_flashTime = 0.25f;

    static ScreenFlash s_theFlash;

    Image m_image;
    Color m_color;
    bool m_isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        m_color = m_image.color;
        m_image.enabled = false;
        s_theFlash = this;
    }

    public static ScreenFlash Get()
    {
        return s_theFlash;
    }

    public void DoFlash()
    {
        if (m_isFlashing)
            return;
        m_isFlashing = true;
        StartCoroutine(FlashCo());
    }

    IEnumerator FlashCo()
    {
        m_image.enabled = true;
        Color color = m_color;
        float countDown = m_flashTime;
        while (countDown > 0.0f)
        {
            float alpha = countDown / m_flashTime;
            color.a = alpha;
            m_image.color = color;
            countDown -= Time.deltaTime;
            yield return null;
        }
        color.a = 0.0f;
        m_image.color = color;
        m_image.enabled = false;
        m_isFlashing = false;
    }
}
