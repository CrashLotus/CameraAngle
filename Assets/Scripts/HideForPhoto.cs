using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideForPhoto : MonoBehaviour
{
    public Manager m_manager;

    private void Start()
    {
        m_manager.AddHideForPhoto(gameObject);        
    }
}
