using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DateButtonLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool up;
    Button btn;

    void Start() 
    {
        btn = gameObject.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<DateInputLogic>().clicked = true;
        StartCoroutine(FindObjectOfType<DateInputLogic>().modifyDateWithTime(btn, up));
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        FindObjectOfType<DateInputLogic>().clicked = false;
    }
    
}
