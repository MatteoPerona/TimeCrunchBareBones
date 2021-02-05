using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PeronaScroll : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public List<GameObject> objects;
    public Button upButton;
    public Button downButton;
    public float threshold = 500;
    private float contentHeight;
    private int nextIndex;
    private int prevIndex;
    private List<GameObject> defaultObs;
    private Vector3 startPos;
    private float dy;

    // Start is called before the first frame update
    void Start()
    {
        if (objects == null)
        {
            objects = new List<GameObject>();
        }

        if (defaultObs == null)
        {
            defaultObs = new List<GameObject>();
        }

        for (int i=0; i<transform.childCount; i++)
        {
            defaultObs.Add(transform.GetChild(i).gameObject);
        }

        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        
        contentHeight = GetComponent<RectTransform>().sizeDelta.y;

        upButton.onClick.AddListener(delegate{
            scrollUp();
        });
        downButton.onClick.AddListener(delegate{
            scrollDown();
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
        resetScroll();
    }

    public void resetScroll()
    {
        updateScroll(objects);
    }

    public void scrollDown()
    {
        Debug.Log(objects.Count+" - "+nextIndex);
        foreach (GameObject g in objects)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in defaultObs)
        {
            g.SetActive(false);
        }
        downButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(true);
        updateScroll(objects.GetRange(nextIndex, objects.Count-nextIndex), false);
    }

    public void scrollUp()
    {
        foreach (GameObject g in objects)
        {
            g.SetActive(false);
        }
        if (prevIndex == 0)
        {
            foreach (GameObject g in defaultObs)
            {
                g.SetActive(true);
            }
        }
        downButton.gameObject.SetActive(true);
        upButton.gameObject.SetActive(false);
        updateScroll(objects.GetRange(prevIndex, objects.Count-prevIndex));
    }
    
    public void updateScroll(List<GameObject> obs, bool updatePrev = true)
    {
        if (updatePrev)
        {
            prevIndex = objects.IndexOf(obs[0]);
        }
        bool firstInactive = false;
        int endi = 0;
        for (int i=0; i<obs.Count; i++)
        {
            if (objectsHeight(obs.GetRange(0,i+1)) > contentHeight)
            {
                obs[i].SetActive(false);
                if (!firstInactive)
                {
                    firstInactive=true;
                    nextIndex = objects.IndexOf(obs[i]);
                    endi=i-1;
                }
            }
            else
            {
                obs[i].SetActive(true);
                endi++;
            }
        }
        if (endi < obs.Count)
        {
            checkMoveButtons(obs.GetRange(0, endi+1));
        }
    }

    void checkMoveButtons(List<GameObject> obs)
    {
        if (obs.Contains(objects[0]) && obs.Contains(objects[objects.Count-1]))
        {
            Debug.Log("Contains All");
            upButton.gameObject.SetActive(false);
            downButton.gameObject.SetActive(false);
        }
        else if (obs.Contains(objects[0]))
        {
            Debug.Log("List is at top");
            upButton.gameObject.SetActive(false);
            downButton.gameObject.SetActive(true);
        }
        else if (obs.Contains(objects[objects.Count-1]))
        {
            Debug.Log("List is at bottom");
            upButton.gameObject.SetActive(true);
            downButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("List is at middle");
            upButton.gameObject.SetActive(true);
            downButton.gameObject.SetActive(true);
        }
        upButton.transform.SetAsFirstSibling();
        downButton.transform.SetAsLastSibling();
    }

    public float objectsHeight(List<GameObject> obs)
    {
        VerticalLayoutGroup layoutVertical = gameObject.GetComponent<VerticalLayoutGroup>();
        float h = layoutVertical.padding.bottom + layoutVertical.padding.top + (transform.childCount-1)*layoutVertical.spacing;
        h += downButton.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        if (downButton.gameObject.activeSelf)
        {
            h += downButton.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        }
        if (upButton.gameObject.activeSelf)
        {
            h += upButton.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        }
        
        foreach (GameObject o in obs)
        {
            h += o.GetComponent<RectTransform>().sizeDelta.y;
        }

        return h;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dy = eventData.position.y-startPos.y;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ButtonAnimator[] animators = FindObjectsOfType<ButtonAnimator>();
        foreach (ButtonAnimator a in animators)
        {
            StartCoroutine(a.pointerActuallyUp());
        }
        if (dy > threshold && downButton.IsActive())
        {
            scrollDown();
        }
        else if (dy < threshold && upButton.IsActive())
        {
            scrollUp();
        }
    }
}
