using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Swiper : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public delegate void SwipeEvent();
    public SwipeEvent SwipeUpEvent;
    public SwipeEvent SwipeDownEvent;
    public SwipeEvent SwipeLeftEvent;
    public SwipeEvent SwipeRightEvent;

    public float m_swipeDist = 10.0f;

    Vector2 m_dragStart = Vector2.zero;
    bool m_isDragging = false;

    bool CheckDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - m_dragStart;
        if (delta.magnitude > m_swipeDist)
        {
            SwipeText("Swipe");
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                {
                    SwipeRightEvent?.Invoke();
                }
                else
                {
                    SwipeLeftEvent?.Invoke();
                }
            }
            else
            {
                if (delta.y > 0)
                {
                    SwipeUpEvent?.Invoke();
                }
                else
                {
                    SwipeDownEvent?.Invoke();
                }
            }
            m_isDragging = false;   // complete the drag
            return true;
        }
        return false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_isDragging)
        {
            SwipeText("Drag");
            CheckDrag(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDragging = true;
        m_dragStart = eventData.position;
        SwipeText("Begin Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragDist = (eventData.position - m_dragStart).magnitude;
        SwipeText("End Drag " + dragDist.ToString());
        if (m_isDragging)
        {
            CheckDrag(eventData);
        }
        m_isDragging = false;
    }

    void SwipeText(string text)
    {
        //Debug.Log(text);
    }
}
