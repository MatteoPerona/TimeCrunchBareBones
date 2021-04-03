using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProjectButton : MonoBehaviour
{
    public Project project;
    public GameObject projectPanel;
    private Transform parentPanel;
    public TMP_Text title;
    public Image progress;
    public TMP_Text taskCount;
    public GameObject touchHoldOpts;
    public GameObject regularLayout;
    public Button delete;
    public GameObject deleteQuestionPanel;

    // Start is called before the first frame update
    void Start()
    {
        delete.onClick.AddListener(delegate{openDeleteQuestion();});
        touchHoldOpts.SetActive(false);
        if (project == null)
        {
            project = FindObjectOfType<Logic>().activeProject;
        }
        
        title.text = project.title;
        progress.fillAmount = project.progress();
        taskCount.text = project.incompleteTasks.Count.ToString();

        parentPanel = FindObjectOfType<Canvas>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        title.text = project.title;
        progress.fillAmount = project.progress();
        taskCount.text = project.incompleteTasks.Count.ToString();
    }

    
    void OnEnable()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            FindObjectOfType<Logic>().activeProject = project;
            FindObjectOfType<Logic>().newProject = false;
            GameObject newEditor = Instantiate(projectPanel, transform.position, Quaternion.identity);
            newEditor.transform.SetParent(parentPanel.transform);
            newEditor.transform.position = Input.mousePosition;
        });
        regularLayout.GetComponent<CanvasGroup>().alpha = 1;
        touchHoldOpts.GetComponent<CanvasGroup>().alpha = 0;
        touchHoldOpts.SetActive(false);
    }

    public void startHoldOptionsProcess()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        touchHoldOpts.SetActive(true);
        StartCoroutine(fadeCanvasGroup(regularLayout.GetComponent<CanvasGroup>(), 0.25f, 1, 0));
        StartCoroutine(fadeCanvasGroup(touchHoldOpts.GetComponent<CanvasGroup>(), 0.25f, 0, 1));        
    }

    public void endHoldOptionsProcess()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            FindObjectOfType<Logic>().activeProject = project;
            FindObjectOfType<Logic>().newProject = false;
            GameObject newEditor = Instantiate(projectPanel, transform.position, Quaternion.identity);
            newEditor.transform.SetParent(parentPanel.transform);
            newEditor.transform.position = Input.mousePosition;
        });
        StartCoroutine(fadeCanvasGroup(regularLayout.GetComponent<CanvasGroup>(), 0.25f, 0, 1));
        StartCoroutine(fadeCanvasGroup(touchHoldOpts.GetComponent<CanvasGroup>(), 0.25f, 1, 0));
        touchHoldOpts.SetActive(false);
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

    void openDeleteQuestion()
    {
        GameObject newDeleteQ = Instantiate(deleteQuestionPanel, transform.position, transform.rotation);
        newDeleteQ.transform.SetParent(parentPanel);
        newDeleteQ.transform.localPosition = new Vector3(0,0,0);
        StartCoroutine(fadeCanvasGroup(newDeleteQ.GetComponent<CanvasGroup>(), 0.25f, 0, 1));
        newDeleteQ.GetComponent<DeleteQPanelLogic>().project = project;
        newDeleteQ.GetComponent<DeleteQPanelLogic>().setTargetButton(gameObject);
    }

    public void destroyMe()
    {
        transform.SetParent(transform.parent.parent);
        Destroy(gameObject);
        FindObjectOfType<Logic>().scrollContent.GetComponent<PeronaScroll>().findObjects();
    }
}
