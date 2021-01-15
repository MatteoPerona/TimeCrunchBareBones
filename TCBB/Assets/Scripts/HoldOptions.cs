using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldOptions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool clicked;
    public bool holdOptsOpen;
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
        while (clicked)
        {
            if (time >= duration)
            {
                FindObjectOfType<ProjectButton>().startHoldOptionsProcess();
                holdOptsOpen = true;
                break;
            }
            yield return null;
            time += Time.deltaTime;
        }
        StartCoroutine(checkDone());
    }

    public IEnumerator checkDone()
    {
        bool correctPress = false;
        float time = 0.0f;
        while (holdOptsOpen)
        {
            if (correctPress)
            {
                bool mainPressed = gameObject.GetComponent<Button>().GetComponent<ButtonAnimator>().pressed;
                bool delPressed = FindObjectOfType<ProjectButton>().delete.GetComponent<ButtonAnimator>().pressed;
                if (!mainPressed && !delPressed)
                {
                    correctPress = false;
                }
            }
            else if (time >= 1)
            {
                if (Input.touchCount > 0)
                {
                    
                    bool mainPressed = gameObject.GetComponent<Button>().GetComponent<ButtonAnimator>().pressed;
                    bool delPressed = FindObjectOfType<ProjectButton>().delete.GetComponent<ButtonAnimator>().pressed;
                    if (mainPressed || delPressed)
                    {
                        correctPress = true;
                    }
                    else
                    {
                        FindObjectOfType<ProjectButton>().endHoldOptionsProcess();
                        holdOptsOpen = false;
                        break;
                    }
                }
            }
            yield return null;
            time += Time.deltaTime;
        }
    }
}
