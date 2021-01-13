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

    void Start() 
    {
        startSize = gameObject.GetComponent<RectTransform>().localScale;
        scalingVector = new Vector3(scalingFactor, scalingFactor, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.LeanScale(Vector3.Scale(startSize, scalingVector), animTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.LeanScale(startSize, animTime);
    }
}
