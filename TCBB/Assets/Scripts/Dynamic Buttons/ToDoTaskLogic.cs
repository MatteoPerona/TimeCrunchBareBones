using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToDoTaskLogic : MonoBehaviour
{
    public Task task;
    public TMP_Text title;
    public GameObject touchHoldOpts;
    public GameObject regularLayout;
    public Button delete;
    private Button button;
    public GameObject crunchScreen;
    private float animTime = 0.1f;
    private Vector2 initialSizeDelta;
    private float scalingFactor = 0.85f;

    // Start is called before the first frame update
    void Start()
    {
        title.text = task.title;

        updateHeight();

        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate{
            openCrunch();
        });

        initialSizeDelta = GetComponent<RectTransform>().sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        title.text = task.title;
    }

    public void updateHeight()
    {
        //30 min = min height
        //maxTime/30min = 48
        float size = task.timeEstimate*48;
        float h = 150*size;
        if (h < 150) {h = 150;}
        else if (h > 1500) {h = 1500;}
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  h);
    }

    void openCrunch()
    {
        GameObject newCrunch = Instantiate(crunchScreen, Input.mousePosition, Quaternion.identity);
        newCrunch.GetComponent<CrunchLogic>().task = task;
        newCrunch.transform.SetParent(FindObjectOfType<TodayLogic>().transform.parent.parent); 
    }

    public void startHoldOptionsProcess()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        //touchHoldOpts.SetActive(true);
        Vector2 newSizeDelta = new Vector2(initialSizeDelta.x * scalingFactor, initialSizeDelta.y);
        StartCoroutine(Shrink(animTime, GetComponent<RectTransform>(), newSizeDelta));
    }

    public void endHoldOptionsProcess()
    {
        button.onClick.AddListener(delegate {
            openCrunch();
        });

        StartCoroutine(Shrink(animTime, GetComponent<RectTransform>(), initialSizeDelta));
        //touchHoldOpts.SetActive(false);
    }

    IEnumerator fadeCanvasGroup(CanvasGroup group, float duration, float startAlpha, float endAlpha)
    {
        float time = 0.0f;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, startAlpha, duration, endAlpha);
        while (time < duration)
        {
            float currentAlpha = curve.Evaluate(time);
            group.alpha = currentAlpha;

            yield return null;
            time += Time.deltaTime;
        }
        group.alpha = endAlpha;
    }

    public void destroyMe()
	{
        Destroy(gameObject);
	}

    public IEnumerator Shrink(float duration, RectTransform r, Vector2 final)
	{
        Vector2 initial = r.sizeDelta;

        float time = 0.0f;
        while (time < duration)
		{
            r.sizeDelta = Vector2.Lerp(initial, final, time / duration);

            yield return null;
            time += Time.deltaTime;
		}
        r.sizeDelta = final;
	}
}
