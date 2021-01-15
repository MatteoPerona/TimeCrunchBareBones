using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AdderLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        FindObjectOfType<CrunchLogic>().adderClicked = true;
        StartCoroutine(FindObjectOfType<CrunchLogic>().addTime());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        FindObjectOfType<CrunchLogic>().adderClicked = false;
    }
}
