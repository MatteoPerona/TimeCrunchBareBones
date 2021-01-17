using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float animTime = .06f;
    public float scalingFactor = .8f;
    private Vector3 startSize;
    private Vector3 scalingVector;
    public bool pressed;

    void Start() 
    {
        startSize = gameObject.GetComponent<RectTransform>().localScale;
        scalingVector = new Vector3(scalingFactor, scalingFactor, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        gameObject.LeanScale(Vector3.Scale(startSize, scalingVector), animTime);
        StartCoroutine(pointerActuallyUp());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    IEnumerator pointerActuallyUp()
    {
        while (Input.touchCount > 0 || Input.anyKey)
        {
            yield return null;
        }
        pressed = false;
        gameObject.LeanScale(startSize, animTime);
    }
}
