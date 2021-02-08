using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PeronaScroll : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public List<GameObject> objects;
    public Button upButton;
    public Button downButton;
    public float threshold = 500.0f;
    public CanvasGroup group;
    public float animTime = .1f;
    public List<GameObject> defaultObs;
    private float contentHeight;
    private int nextIndex;
    private int currentIndex;
    private Vector3 startPos;
    private Vector3 startPosDrag;
    private float dy;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;

        if (objects == null)
        {
            objects = new List<GameObject>();
        }

        if (defaultObs == null)
        {
            defaultObs = new List<GameObject>();
        }

        defaultObs.Add(upButton.gameObject);
        defaultObs.Add(downButton.gameObject);

        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        
        contentHeight = GetComponent<RectTransform>().sizeDelta.y;

        upButton.onClick.AddListener(delegate{
            scrollUp();
            pointersUp();
        });
        downButton.onClick.AddListener(delegate{
            scrollDown();
            pointersUp();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void findObjects()
    {
        objects.Clear();
        for (int i=0; i<transform.childCount; i++)
        {
            bool isDefaultOb = false;
            foreach (GameObject defaultOb in defaultObs)
            {
                if (transform.GetChild(i).gameObject == defaultOb)
                {
                    isDefaultOb = true;
                    break;
                }
            }
            if (!isDefaultOb)
            {
                objects.Add(transform.GetChild(i).gameObject);
            }
        }
        int index = 0;
        foreach (GameObject o in defaultObs)
        {
            o.transform.SetSiblingIndex(index);
            index++;
        }
        foreach (GameObject o in objects)
        {
            Debug.Log(o.GetComponentInChildren<TMP_Text>().text);
        }
        resetScroll();
    }

    public void resetScroll()
    {
        foreach (GameObject g in defaultObs)
        {
            g.SetActive(true);
        }
        upButton.gameObject.SetActive(false);
        if (objects.Count > 0)
        {
            updateScroll(objects);
        }
    }

    public void scrollDown()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in defaultObs)
        {
            g.SetActive(false);
        }
        updateScroll(objects.GetRange(nextIndex, objects.Count-nextIndex));
    }

    public void scrollUp()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(false);
        }        
        int startOffset = 0;
        for (int i=currentIndex-1; i>=0; i--)
        {
            if (objectsHeight(objects.GetRange(i,currentIndex-i)) > contentHeight)
            {
                break;
            }
            else
            {
                startOffset++;
            }
        }
        //Debug.Log("currentIndex-startOffset: "+currentIndex+"-"+startOffset);
        if (currentIndex-startOffset == 0)
        {
            foreach (GameObject g in defaultObs)
            {
                g.SetActive(true);
            }
            upButton.gameObject.SetActive(false);
        }
        nextIndex = currentIndex;
        updateScroll(objects.GetRange(currentIndex-startOffset, startOffset));
    }
    
    public void updateScroll(List<GameObject> obs)
    {
        currentIndex = objects.IndexOf(obs[0]);
        int count = 0;
        for (int i=0; i<obs.Count; i++)
        {
            if (objectsHeight(obs.GetRange(0,i+1)) > contentHeight)
            {
                foreach (GameObject o in obs.GetRange(i, obs.Count-i))
                {
                    o.SetActive(false);
                }
                nextIndex = objects.IndexOf(obs[i]);
                break;
            }
            else
            {
                obs[i].SetActive(true);
                count++;
            }
        }
        checkMoveButtons(obs.GetRange(0, count));
    }

    void checkMoveButtons(List<GameObject> obs)
    {
        if (obs.Contains(objects[0]) && obs.Contains(objects[objects.Count-1]))
        {
            upButton.gameObject.SetActive(false);
            downButton.gameObject.SetActive(false);
        }
        else if (obs.Contains(objects[0]))
        {
            upButton.gameObject.SetActive(false);
            downButton.gameObject.SetActive(true);
        }
        else if (obs.Contains(objects[objects.Count-1]))
        {
            upButton.gameObject.SetActive(true);
            downButton.gameObject.SetActive(false);
        }
        else
        {
            upButton.gameObject.SetActive(true);
            downButton.gameObject.SetActive(true);
        }
        upButton.transform.SetAsFirstSibling();
        downButton.transform.SetAsLastSibling();
    }

    public float objectsHeight(List<GameObject> obs)
    {
        VerticalLayoutGroup layoutVertical = gameObject.GetComponent<VerticalLayoutGroup>();
        float h = layoutVertical.padding.bottom + layoutVertical.padding.top + (obs.Count+1)*layoutVertical.spacing;
        float hI = h;

        //int count=0;
        foreach (GameObject g in defaultObs)
        {
            if (g.activeSelf)
            {
                h += g.GetComponent<RectTransform>().sizeDelta.y;
                //count++;
            }
        }

        //float hD = h-hI;
        
        foreach (GameObject o in obs)
        {
            h += o.GetComponent<RectTransform>().sizeDelta.y;
        }

        //float hF = h-hD;
        //Debug.Log("Count "+obs.Count+","+ " Default "+count+": "+hI+" + "+hD+" + "+hF+" = "+h);
        
        return h;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosDrag = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dy = eventData.position.y-startPosDrag.y;
        
        Vector3 currentPos = new Vector3(startPos.x, startPos.y+50*dy/threshold, startPos.z);
        transform.position = currentPos;

        if (dy>0 && downButton.gameObject.activeSelf)
        {
            downButton.gameObject.GetComponent<CanvasGroup>().alpha = dy/threshold;
        }
        else if (dy<0 && upButton.gameObject.activeSelf)
        {
            upButton.gameObject.GetComponent<CanvasGroup>().alpha = -1*dy/threshold;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        pointersUp();
        if (dy > threshold && downButton.gameObject.activeSelf)
        {
            scrollDown();
        }
        else if (-1*dy > threshold && upButton.gameObject.activeSelf)
        {
            scrollUp();
        }
        downButton.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        upButton.gameObject.GetComponent<CanvasGroup>().alpha = 0;

        StartCoroutine(smoothMove(gameObject, animTime, transform.position, startPos));
    }

    IEnumerator smoothMove(GameObject o, float duration, Vector3 iPos, Vector3 fPos)
    {
        float time = 0.0f;
        while (time < duration)
        {
            o.transform.position = Vector3.Lerp(iPos, fPos, Mathf.Pow(time/duration, 1));

            yield return null;
            time += Time.deltaTime;
        }
        o.transform.position = fPos;
    }

    void pointersUp()
    {
        ButtonAnimator[] animators = FindObjectsOfType<ButtonAnimator>();
        foreach (ButtonAnimator a in animators)
        {
            StartCoroutine(a.pointerActuallyUp());
        }
    }

    IEnumerator fadeOutFadeIn(CanvasGroup group, float duration, UnityEngine.Events.UnityAction call)
    {
        float time = 0.0f;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, 1, duration, 0);
        while(time < duration)
        {
            float currentAlpha = curve.Evaluate(time);
            group.alpha = currentAlpha;

            yield return null;
            time += Time.deltaTime;
        }

        time = 0.0f;
        curve = AnimationCurve.EaseInOut(time, 0, duration, 1);
        while(time < duration)
        {
            float currentAlpha = curve.Evaluate(time);
            group.alpha = currentAlpha;

            yield return null;
            time += Time.deltaTime;
        }
    }
}
