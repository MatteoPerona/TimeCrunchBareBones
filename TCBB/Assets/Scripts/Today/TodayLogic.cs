using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TodayLogic : MonoBehaviour
{
    public GameObject toDoTask;
    public TMP_Text dateText;
    private System.DateTime date;
    public Button dateForthBtn;
    public Button dateBackBtn;
    public Transform scrollContent;
    public List<GameObject> scrollObjects;
    public float fadeTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        date = System.DateTime.Now.Date;
        dateText.text = date.ToString("d");

        dateForthBtn.onClick.AddListener(delegate{
            StartCoroutine(scrollFader(delegate{dateForth();}));
        });
        dateBackBtn.onClick.AddListener(delegate{
            StartCoroutine(scrollFader(delegate{dateBack();}));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetScroll()
    {
        dateBackBtn.interactable = true;
        System.DateTime oldDate = date;
        date = System.DateTime.Now.Date;
        if (date < oldDate || date > oldDate)
        {
            StartCoroutine(scrollFader(delegate{updateScroll();}));   
        }
    }

    void dateForth()
    {
        if (dateText.text == "Late")
        {
            dateBackBtn.interactable = true;
            updateScroll();
        }
        else
        {
            date = date.AddDays(1);
            dateBackBtn.interactable = true;
            updateScroll();
        }
    }

    void dateBack()
    {
        System.DateTime dateNow = System.DateTime.Now.Date;
        if (date == dateNow)
        {
            dateText.text = "Late";
            updateScrollPastDue();
            dateBackBtn.interactable = false;
        }
        else
        {
            date = date.AddDays(-1);
            updateScroll();
        }
    }
    public IEnumerator scrollFader(UnityEngine.Events.UnityAction call)
    {
        yield return StartCoroutine(fadeCanvasGroup(scrollContent.gameObject.GetComponent<CanvasGroup>(), fadeTime, 1, 0));
        call();
        yield return StartCoroutine(fadeCanvasGroup(scrollContent.gameObject.GetComponent<CanvasGroup>(), fadeTime, 0, 1));
    }

    public void updateScroll()
    {
        dateText.text = date.ToString("d");
        // removes all toDo's with the incorrect date and updates height for others
        if (scrollObjects == null)
        {
            scrollObjects = new List<GameObject>();
        }
        foreach (GameObject toDo in scrollObjects.ToArray())
        {
            if (toDo.GetComponent<ToDoTaskLogic>().task.dateToDo != date)
            {
                removeTodo(toDo);
            }
            else
            {
                toDo.GetComponent<ToDoTaskLogic>().updateHeight();
            }
        }

        // identifies tasks that correspond with date & do not currently exist as a toDo
        List<Task> tasks = new List<Task>();
        List<Project> projects = FindObjectOfType<Logic>().activeProjects;
        foreach (Project p in projects)
        {
            foreach (Task t in p.incompleteTasks)
            {
                if (t.dateToDo == date)
                {
                    tasks.Add(t);
                    foreach (GameObject toDo in scrollObjects)
                    {
                        if (toDo.GetComponent<ToDoTaskLogic>().task == t)
                        {
                            tasks.Remove(t);
                            break;
                        }
                    }
                }
            }
        }

        // creates a toDo for all tasks in tasks
        foreach (Task t in tasks)
        {
            addToDo(t);
        }
    }

    public void updateScrollPastDue()
    {
        // removes all toDo's with the incorrect date and updates height for others
        if (scrollObjects == null)
        {
            scrollObjects = new List<GameObject>();
        }
        foreach (GameObject toDo in scrollObjects.ToArray())
        {
            if (toDo.GetComponent<ToDoTaskLogic>().task.dateToDo >= date)
            {
                removeTodo(toDo);
            }
            else
            {
                toDo.GetComponent<ToDoTaskLogic>().updateHeight();
            }
        }

        // identifies tasks that correspond with date & do not currently exist as a toDo
        List<Task> tasks = new List<Task>();
        List<Project> projects = FindObjectOfType<Logic>().activeProjects;
        foreach (Project p in projects)
        {
            foreach (Task t in p.incompleteTasks)
            {
                if (t.dateToDo < date)
                {
                    tasks.Add(t);
                    foreach (GameObject toDo in scrollObjects)
                    {
                        if (toDo.GetComponent<ToDoTaskLogic>().task == t)
                        {
                            tasks.Remove(t);
                            break;
                        }
                    }
                }
            }
        }

        // creates a toDo for all tasks in tasks
        foreach (Task t in tasks)
        {
            addToDo(t);
        }
    }

    void addToDo(Task t)
    {
        GameObject toDo = Instantiate(toDoTask, transform.position, Quaternion.identity);
        toDo.transform.SetParent(scrollContent);
        toDo.GetComponent<ToDoTaskLogic>().task = t;
        scrollObjects.Add(toDo);
        toDo.GetComponent<ToDoTaskLogic>().updateHeight();
    }
    public void removeTodo(GameObject toDo)
    {
        scrollObjects.Remove(toDo);
        Destroy(toDo);
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
}
