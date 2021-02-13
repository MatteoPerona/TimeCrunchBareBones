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
    public GameObject scrollContent;
    public Button taskBtn;
    public Button toggle;
    public Button addTaskBtn;
    public Button descriptionToggle;
    public Button doneBtn;
    private float descriptionScaleY;
    public float dToggleTime = 0.25f;
    public float sToggleTime = 0.1f;
    private Vector2 dContainerSize;
    private RectTransform dContainer;
    private bool descriptionOpen;
    private bool updateProjectScroll = false;
    private List<GameObject> currentTasks;
    
    // Start is called before the first frame update
    void Start()
    {
        incompleteTasksActive = true;
        if (project == null)
        {
            project = FindObjectOfType<Logic>().activeProject;
        }
        loadProjectData(project);

        if (currentTasks == null)
        {
            currentTasks = new List<GameObject>();
        }

        updateProjectScroll = FindObjectOfType<Logic>().newProject;

        dContainer = descriptionContainer.GetComponent<RectTransform>();
        dContainerSize = dContainer.sizeDelta;

        descriptionToggle.gameObject.LeanRotateZ(180f, 0f);
        StartCoroutine(scaleHeightCorutine(0.0f, 1750f, 75f, 1f, 0.5f));

        descriptionScaleY = descriptionContainer.gameObject.GetComponent<RectTransform>().localScale.y;
        descriptionToggle.onClick.AddListener(delegate{toggleDescription();});
        description.onSelect.AddListener(delegate{
            if (!descriptionOpen){
                toggleDescription();}
        });

        addTaskBtn.onClick.AddListener(delegate{
            createNewTask();
            GameObject newTaskEditor = Instantiate(taskEditor, addTaskBtn.gameObject.transform.localPosition, Quaternion.identity);
            newTaskEditor.transform.SetParent(transform.parent);
            newTaskEditor.transform.position = Input.mousePosition;
        });

        doneBtn.onClick.AddListener(delegate{doneProtocol();});

        StartCoroutine(loadTasks());
        toggle.onClick.AddListener(delegate{
            if(incompleteTasksActive)
            {
                StartCoroutine(toggleContainers(sToggleTime, scrollContent.transform, true));
                incompleteTasksActive = false;
            }
            else
            {
                StartCoroutine(toggleContainers(sToggleTime, scrollContent.transform));
                incompleteTasksActive = true;
            }
        });

        scrollContent.GetComponent<PeronaScroll>().findObjects();
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

    public void createNewTaskButton()
    {
        GameObject newTaskButton = Instantiate(taskButton, transform.position, Quaternion.identity);
        newTaskButton.transform.SetParent(scrollContent.transform); 
        currentTasks.Add(newTaskButton);
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator loadTasks(bool loadComplete = false)
    {
        List<Task> tasks = project.incompleteTasks;
        if (loadComplete)
        {
            tasks = project.completedTasks;
        }
        yield return StartCoroutine(clearScroll());
        foreach (Task t in tasks)
        {
            activeTask = t;
            createNewTaskButton();
        }
        numTasks.text = project.incompleteTasks.Count.ToString();
        scrollContent.GetComponent<PeronaScroll>().findObjects();
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
        if (updateProjectScroll)
        {
            FindObjectOfType<Logic>().scrollContent.GetComponent<PeronaScroll>().findObjects();
        }
    }

    IEnumerator toggleContainers(float duration, Transform content, bool loadComplete = false)
    {
        CanvasGroup group = content.GetComponent<CanvasGroup>();
        float time = 0.0f;
        while (time < duration)
        {
            float t = time/duration;
            group.alpha = Mathf.Lerp(1,0,t);

            yield return null;
            time += Time.deltaTime;
        }
        group.alpha = 0;
        
        yield return StartCoroutine(loadTasks(loadComplete));
        if (loadComplete)
        {
            addTaskBtn.gameObject.SetActive(false);
        }

        time = 0.0f;
        while (time < duration)
        {
            float t = time/duration;
            group.alpha = Mathf.Lerp(0, 1, t);

            yield return null;
            time += Time.deltaTime;
        }
        group.alpha = 1;
    }
    IEnumerator clearScroll()
    {
        int i = 0;
        if (currentTasks.Count < 1)
        {
            yield break;
        }

        while (i < currentTasks.Count)
        {
            currentTasks[i].GetComponent<TaskButtonLogic>().destroyMe();
            i++;

            yield return null;
        }
        currentTasks.Clear();
        scrollContent.GetComponent<PeronaScroll>().findObjects();
    }
}
