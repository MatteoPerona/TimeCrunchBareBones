using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionLogic : MonoBehaviour
{
    public Button cancelBtn;
    public Button doneBtn;

    // Start is called before the first frame update
    void Start()
    {
        cancelBtn.onClick.AddListener(delegate{
            StartCoroutine(FindObjectOfType<CrunchLogic>().crunch(0.25f, true, gameObject.GetComponent<RectTransform>()));
        });

        doneBtn.onClick.AddListener(delegate{
            Task t = FindObjectOfType<CrunchLogic>().task;
            foreach (GameObject toDo in FindObjectOfType<TodayLogic>().scrollObjects)
            {
                if (toDo.GetComponent<ToDoTaskLogic>().task == t)
                {
                    FindObjectOfType<TodayLogic>().removeTodo(toDo);
                    break;
                }
            }
            foreach (Project p in FindObjectOfType<Logic>().activeProjects)
            {
                foreach (Task task in p.incompleteTasks)
                {
                    if (task == t)
                    {
                        p.completedTasks.Add(t);
                        p.incompleteTasks.Remove(t);
                        break;
                    }
                }
            }
            FindObjectOfType<CrunchLogic>().destroyMe();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
