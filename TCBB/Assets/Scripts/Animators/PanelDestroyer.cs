using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelDestroyer : MonoBehaviour
{
    public Button doneBtn;
    public GameObject container;
    public float closeTime = 0.25f;
    public float fadeTime = 0.05f;
    private RectTransform panelRect;
    // Start is called before the first frame update
    void Start()
    {
        panelRect = gameObject.GetComponent<RectTransform>();

        doneBtn.onClick.AddListener(delegate{
            destructionProtocol();
        });
    }

    public void destructionProtocol()
    {
        StartCoroutine(fadeCanvasGroup(container.GetComponent<CanvasGroup>(), fadeTime, 1f, 0f));
        StartCoroutine(scaleOutPanel(closeTime));
    }

    IEnumerator scaleOutPanel(float duration)
    {
        SetPivot(panelRect, new Vector2(0.5f, 0f));
        float startHeight = panelRect.sizeDelta.y;
        float endHeight = 0.0f;
        float startWidth = panelRect.sizeDelta.y;
        float endWidth = 0.0f;

        float time = 0.0f;
        AnimationCurve heightCurve = AnimationCurve.EaseInOut(time, startHeight, duration, endHeight);
        AnimationCurve widthCurve = AnimationCurve.EaseInOut(time, startWidth, duration, endWidth);

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
        panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endWidth);
        SetPivot(panelRect, new Vector2(0.5f, 0.5f));
        destroyMe();
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
        group.alpha = endAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void destroyMe()
    {
        Destroy(gameObject);
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
