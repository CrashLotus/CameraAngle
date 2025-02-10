using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraLevel : MonoBehaviour
{
    public Color m_selectColor = Color.white;
    public Color m_unSelectColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);

    Image m_image;
    bool m_isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        Select(false);
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isSelected)
        {
            Manager manager = Manager.Get();
            float tilt = manager.GetTilt();
            if (Mathf.Abs(tilt) < 0.05f)
            {
                float elv = manager.GetElevation();
                if (Mathf.Abs(elv) < 0.05f)
                {
                    manager.OnCameraClicked();
                    Select(false);
                }
            }
        }        
    }

    public void OnClick()
    {
        Select(!m_isSelected);
    }

    void Select(bool isSelected)
    {
        m_isSelected = isSelected;
        m_image.color = m_isSelected ? m_selectColor : m_unSelectColor;
    }


}
