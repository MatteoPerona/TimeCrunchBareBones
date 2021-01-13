using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelOpener : MonoBehaviour
{
    public GameObject container;
    public float openTime = 0.25f;
    public float fadeTime = 0.25f;
    private RectTransform panelRect;
    // Start is called before the first frame update
    void Start()
    {
        panelRect = gameObject.GetComponent<RectTransform>();
        container.GetComponent<CanvasGroup>().alpha = 0f;
        StartCoroutine(scaleMoveFade());
    }

    IEnumerator scaleMoveFade()
    {
        StartCoroutine(scaleInPanel(openTime));
        yield return StartCoroutine(movePanel(openTime, transform.localPosition, new Vector3(0f, 0f, 0f)));
        yield return StartCoroutine(fadeCanvasGroup(container.GetComponent<CanvasGroup>(), fadeTime, 0f, 1f));
    }

    IEnumerator scaleInPanel(float duration)
    {
        float startHeight = 0.0f;
        float endHeight = panelRect.sizeDelta.y;
        float startWidth = 0.0f;
        float endWidth = panelRect.sizeDelta.x;

        float time = 0.0f;
        AnimationCurve heightCurve = AnimationCurve.EaseInOut(time, startHeight, duration, endHeight);
        AnimationCurve widthCurve = AnimationCurve.EaseInOut(time, startWidth, duration, endHeight);
        while (time < duration)
        {
            float currentHeight = heightCurve.Evaluate(time);
            float currentWidth = widthCurve.Evaluate(time);
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);
            panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);

            yield return null;
            time += Time.deltaTime;
        }
        panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endHeight);
        panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, endWidth);
    }

    IEnumerator fadeCanvasGroup(CanvasGroup group, float duration, float startAlpha, float endAlpha)
    {
        float time = 0.0f;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, startAlpha, duration, endAlpha);
        while(time < duration)
        {
            float currentAlpha = curve.Evaluate(time);
            group.alpha = currentAlpha;

            yield return null;
            time += Time.deltaTime;
        }
        checkNewPanel();
    }

    IEnumerator movePanel(float duration, Vector3 startPos, Vector3 endPos)
    {
        float time = 0.0f;
        while(time < duration)
        {
            float t = time/duration;
            panelRect.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
            time += Time.deltaTime;
        }
        panelRect.localPosition = endPos;
    }

    void checkNewPanel()
    {
        if (FindObjectOfType<Logic>().newProject)
        {
            FindObjectOfType<Logic>().createProjectBtn();
        }
        
        if (FindObjectOfType<ProjectPanelLogic>() != null)
        {
            if (FindObjectOfType<ProjectPanelLogic>().newTask)
            {
                FindObjectOfType<ProjectPanelLogic>().createNewTaskButton();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation
        
        rectTransform.pivot = pivot;                            // change the pivot
        rectTransform.localPosition -= deltaPosition;           // reverse the position change
    }
}
