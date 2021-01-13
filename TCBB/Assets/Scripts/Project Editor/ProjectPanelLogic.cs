using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProjectPanelLogic : MonoBehaviour
{
    public Project project;
    public Task activeTask;
    public bool newTask;
    public bool incompleteTasksActive;
    public GameObject taskEditor;
    public GameObject taskButton;
    public TMP_InputField title;
    public TMP_InputField description;
    public TMP_Text numTasks;
    public GameObject descriptionContainer;
    public RectTransform layoutContainer;
    public RectTransform viewportRect;
    public GameObject scrollContentIncomplete;
    public GameObject scrollContenComplete;
    public Button taskBtn;
    public Button toggle;
    public Button addTaskBtn;
    public Button descriptionToggle;
    public Button doneBtn;
    private float descriptionScaleY;
    public float dToggleTime = 0.25f;
    private Vector2 dContainerSize;
    private RectTransform dContainer;
    private bool descriptionOpen;
    
    // Start is called before the first frame update
    void Start()
    {
        incompleteTasksActive = true;
        if (project == null)
        {
            project = FindObjectOfType<Logic>().activeProject;
        }
        loadProjectData(project);

        dContainer = descriptionContainer.GetComponent<RectTransform>();
        dContainerSize = dContainer.sizeDelta;

        descriptionToggle.gameObject.LeanRotateZ(180f, 0f);
        StartCoroutine(scaleHeightCorutine(0.0f, 1750f, 75f, 1f, 0.5f));

        descriptionScaleY = descriptionContainer.gameObject.GetComponent<RectTransform>().localScale.y;
        descriptionToggle.onClick.AddListener(delegate{toggleDescription();});

        addTaskBtn.onClick.AddListener(delegate{
            createNewTask();
            GameObject newTaskEditor = Instantiate(taskEditor, addTaskBtn.gameObject.transform.localPosition, Quaternion.identity);
            newTaskEditor.transform.SetParent(gameObject.transform.parent);
            newTaskEditor.transform.position = Input.mousePosition;
        });

        doneBtn.onClick.AddListener(delegate{doneProtocol();});

        loadTasks();
        toggle.onClick.AddListener(delegate{
            if(incompleteTasksActive)
            {
                StartCoroutine(toggleContainers(dToggleTime, scrollContentIncomplete.transform, scrollContenComplete.transform));
                incompleteTasksActive = false;
            }
            else
            {
                StartCoroutine(toggleContainers(dToggleTime, scrollContenComplete.transform, scrollContentIncomplete.transform));
                incompleteTasksActive = true;
            }
        });
    }

    void loadProjectData(Project p)
    {
        title.text = project.title;
        description.text = project.description;
    }

    void createNewTask()
    {
        FindObjectOfType<Logic>().newProject = false;
        newTask = true;
        Task t = new Task();
        project.incompleteTasks.Add(t);
        activeTask = t; 
        numTasks.text = project.incompleteTasks.Count.ToString();
    }

    public void createNewTaskButton(bool incomplete = true)
    {
        GameObject newTaskButton = Instantiate(taskButton, transform.position, Quaternion.identity);
        if (incomplete)
        {
            newTaskButton.transform.SetParent(scrollContentIncomplete.transform); 
        }
        else
        {
            newTaskButton.transform.SetParent(scrollContenComplete.transform); 
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void loadTasks()
    {
        foreach (Task t in project.completedTasks)
        {
            activeTask = t;
            createNewTaskButton(false);
            numTasks.text = project.completedTasks.Count.ToString();
        }

        foreach (Task t in project.incompleteTasks)
        {
            activeTask = t;
            createNewTaskButton();
            numTasks.text = project.incompleteTasks.Count.ToString();
        }
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

    void doneProtocol()
    {
        project.title = title.text;
        project.description = description.text;
    }

    IEnumerator toggleContainers(float duration, Transform t1, Transform t2)
    {
        Vector3 p1 = t1.localPosition;
        Vector3 p2 = t2.localPosition;
        Vector3 fp1 = p1;
        fp1.x -= p2.x-p1.x+p1.x;

        float time = 0.0f;
        while (time < duration)
        {
            float t = time/duration;
            t2.localPosition = Vector3.Lerp(p2, p1, t);
            t1.localPosition = Vector3.Lerp(p1, fp1, t);

            yield return null;
            time += Time.deltaTime;
        }
        t2.localPosition = p1;
        t1.localPosition = fp1;

    }
}
