//----------------------------------------------------------------------------------------
//	Copyright © 2024 Matt Whiting, All Rights Reserved.
//  For educational purposes only.
//  Please do not distribute or republish in electronic or print form without permission.
//  Thanks - whitingm@usc.edu
//----------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    public GameObject m_yesButton;
    public GameObject m_noButton;
    public GameObject m_okButton;

    public delegate void OnDialogButton();
    OnDialogButton m_yes;
    OnDialogButton m_no;

    public static void ShowDialog(string text, OnDialogButton onYes, OnDialogButton onNo=null)
    {
        GameObject obj = Resources.Load<GameObject>("DialogBox");
        if (obj == null)
        {
            Debug.LogError("Unable to load Dialog Box");
            return;
        }
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Can't find the Canvas");
            return;
        }
        GameObject box = Instantiate(obj, canvas.transform);
        DialogBox dialogBox = box.GetComponent<DialogBox>();
        if (dialogBox == null)
        {
            Debug.LogError("Can't find DialogBox component");
            return;
        }
        dialogBox._ShowDialog(text, onYes, onNo);
    }

    void _ShowDialog(string text, OnDialogButton onYes, OnDialogButton onNo)
    {
        gameObject.SetActive(true);
        m_text.text = text;
        m_yes = onYes;
        m_no = onNo;
        // if no is specified, you get a yes and a no
        m_yesButton.SetActive(m_no != null);
        m_noButton.SetActive(m_no != null);
        // if no not specified, you just get ok
        m_okButton.SetActive(m_no == null);
    }

    public void OnDialogYes()
    {
        Debug.Log("OnDialogYes()");
        if (null != m_yes)
            m_yes();
        Destroy(gameObject);
    }

    public void OnDialogNo()
    {
        Debug.Log("OnDialogNo()");
        if (null != m_no)
            m_no();
        Destroy(gameObject);
    }
}
