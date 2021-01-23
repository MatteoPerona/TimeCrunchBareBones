using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PeronaScroll : MonoBehaviour
{
    public List<GameObject> objects;
    public Button downBtn;
    private float contentHeight;
    private int nextIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (objects == null)
        {
            objects = new List<GameObject>();
        }
        contentHeight = GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetScroll()
    {
        updateScroll(objects);
    }

    public void scrollDown()
    {
        updateScroll(objects.GetRange(nextIndex, objects.Count-nextIndex-1));
    }

    public void scrollUp()
    {
        updateScroll(objects.GetRange(nextIndex, objects.Count-nextIndex-1));
    }
    
    public void updateScroll(List<GameObject> obs)
    {
        bool firstInactive = false;
        for (int i=1; i<=obs.Count; i++)
        {
            if (objectsHeight(obs.GetRange(0, i)) > contentHeight)
            {
                obs[i].SetActive(false);
                if (!firstInactive)
                {
                    firstInactive=true;
                    nextIndex = objects.IndexOf(obs[i]);
                }
            }
            else
            {
                obs[i].SetActive(true);
            }
        }
    }

    public float objectsHeight(List<GameObject> obs)
    {
        VerticalLayoutGroup layoutVertical = gameObject.GetComponent<VerticalLayoutGroup>();
        float h = layoutVertical.padding.bottom + layoutVertical.padding.top + objects.Count*layoutVertical.spacing;
        h += downBtn.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        foreach (GameObject o in obs)
        {
            h += o.GetComponent<RectTransform>().sizeDelta.y;
        }

        return h;
    }
}
