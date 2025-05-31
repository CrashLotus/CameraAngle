using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowImage : MonoBehaviour
{
    public GameObject m_optionsPanel;

    public void OnEnable()
    {
        Button button = GetComponent<Button>();
        if (button)
        { 
            Manager manager = Manager.Get();
            if (manager && manager.HasImage())
            {
                button.interactable = true;
                return;
            }
            button.interactable = false;
        }
    }

    public void OnClick()
    {
        Manager manager = Manager.Get();
        if (manager)
        {
            if (manager.ShowImage())
            {
                m_optionsPanel.SetActive(false);
            }    
        }
    }
}
