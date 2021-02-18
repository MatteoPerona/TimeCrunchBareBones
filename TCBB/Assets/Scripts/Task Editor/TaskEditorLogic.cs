using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskEditorLogic : MonoBehaviour
{
    public Task task;
    public GameObject container;
    public Button doneButton;
    public Button dateUpBtn;
    public Button dateDownBtn;
    public Button descriptionToggle;
    public float dToggleTime = 0.25f;
    public RectTransform layoutContainer;
    public RectTransform dContainer;
    public TMP_InputField title;
    public TMP_InputField description;
    public TMP_InputField timeEstimate;
    public TMP_InputField dateInput;
    public Slider timeEstimateSlider;
    public System.DateTime date;
    private bool descriptionOpen;

    // Start is called before the first frame update
    void Start()
    {
        if (task == null)
        {
            task = FindObjectOfType<ProjectPanelLogic>().activeTask;
            if (task.dateToDo == new System.DateTime()){task.dateToDo = System.DateTime.Now.Date.AddDays(1);}
        }
        title.text = task.title;
        description.text = task.description;
        timeEstimate.text = FindObjectOfType<TimeInput>().timeFromRawValue(task.timeEstimate);
        date = task.dateToDo;
        dateInput.text = date.ToString("d");

        doneButton.onClick.AddListener(delegate{save();});

        descriptionToggle.gameObject.LeanRotateZ(180f, 0f);
        StartCoroutine(scaleHeightCorutine(0.0f, 1750f, 75f, 1f, 0.5f));

        descriptionToggle.onClick.AddListener(delegate{toggleDescription();});
        description.onSelect.AddListener(delegate{
            if (!descriptionOpen){
                toggleDescription();}
        });
    }

    void updateTimeEstimate()
    {
        
    }

    void save()
    {
        task.title = title.text;
        task.description = description.text;
        task.dateToDo = date;
        task.timeEstimate = FindObjectOfType<TimeInput>().rawValueFromTime(timeEstimate.text);
        FindObjectOfType<ProjectPanelLogic>().scrollContent.GetComponent<PeronaScroll>().findObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void toggleDescription()
    {
        if (descriptionOpen)
        {
            descriptionToggle.gameObject.LeanRotateZ(180f, 0f);
            StartCoroutine(scaleHeightCorutine(dToggleTime, 1750f, 75f, 1f, 0.5f));
            descriptionOpen = false;
        }
        else
        {
            descriptionToggle.gameObject.LeanRotateZ(0f, 0f);
            StartCoroutine(scaleHeightCorutine(dToggleTime, 75f, 1750f, 0.5f, 1f));
            descriptionOpen = true;
        }
    }

    IEnumerator scaleHeightCorutine(float duration, float startHeight, float endHeight, float startAlpha, float endAlpha)
    {
        float time = 0.0f;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, startHeight, duration, endHeight);
        AnimationCurve alphaCurve = AnimationCurve.EaseInOut(time, startAlpha, duration, endAlpha);
        while (time < duration)
        {
            float currentHeight = curve.Evaluate(time);
            dContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContainer);

            float currentAlpha = alphaCurve.Evaluate(time);
            dContainer.GetComponent<CanvasGroup>().alpha = currentAlpha;

            yield return null;
            time += Time.deltaTime;
        }
        dContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endHeight);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContainer);

        dContainer.GetComponent<CanvasGroup>().alpha = endAlpha;
    }
}
