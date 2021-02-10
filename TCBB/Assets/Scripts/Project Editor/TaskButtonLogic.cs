using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskButtonLogic : MonoBehaviour
{
    public TMP_Text title;
    public GameObject taskEditor;
    private Task task;

    void Awake()
    {
        if (task == null)
        {
            task = FindObjectOfType<ProjectPanelLogic>().activeTask;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (task == null)
        {
            task = FindObjectOfType<ProjectPanelLogic>().activeTask;
        }

        gameObject.GetComponent<Button>().onClick.AddListener(delegate{
            FindObjectOfType<ProjectPanelLogic>().newTask = false;
            FindObjectOfType<ProjectPanelLogic>().activeTask = task;
            GameObject newTaskEditor = Instantiate(taskEditor, transform.position, Quaternion.identity);
            newTaskEditor.transform.SetParent(FindObjectOfType<ProjectPanelLogic>().gameObject.transform.parent);
            newTaskEditor.transform.position = Input.mousePosition;
        });
    }

    // Update is called once per frame
    void Update()
    {
        title.text = task.title;
    }

    public void destroyMe()
    {
        Destroy(gameObject);
    }
}
