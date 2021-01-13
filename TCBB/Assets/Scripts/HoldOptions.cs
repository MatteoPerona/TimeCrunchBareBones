using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldOptions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool clicked;
    public float holdTime = 1.5f;
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
        clicked = true;
        StartCoroutine(holdCoroutine(holdTime));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
    }

    public IEnumerator holdCoroutine(float duration)
    {
        float time = 0.0f;
        while (time < duration && clicked)
        {
            yield return null;
            time += Time.deltaTime;
        }

    }
}
