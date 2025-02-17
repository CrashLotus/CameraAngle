using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageDisplay : MonoBehaviour
{
    public TextMeshProUGUI m_date;

    RawImage m_rawImage;
    Texture2D m_texDisplay;

    private void OnEnable()
    {
        m_rawImage = GetComponent<RawImage>();

        if (null != m_texDisplay)
        {
            Destroy(m_texDisplay);
            m_texDisplay = null;
        }

        // get the most recent file
        SaveData save = SaveData.Get();
        List<string> dates = save.GetDates();
        if (null != dates && dates.Count > 0)
        {
            string date = dates[dates.Count - 1];
            List<string> files = save.GetFiles(date);
            if (null != files && files.Count > 0)
            {
                string file = files[files.Count - 1];
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

    private void OnDisable()
    {
        Destroy(m_texDisplay);
        m_texDisplay = null;
    }
}
