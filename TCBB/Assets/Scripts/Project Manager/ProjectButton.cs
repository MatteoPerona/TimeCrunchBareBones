using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProjectButton : MonoBehaviour
{
    private Project project;
    public GameObject projectPanel;
    private Transform parentPanel;
    public TMP_Text title;
    public Image progress;
    public TMP_Text taskCount;

    // Start is called before the first frame update
    void Start()
    {
        if (project == null)
        {
            project = FindObjectOfType<Logic>().activeProject;
        }
        
        title.text = project.title;
        progress.fillAmount = project.progress();
        taskCount.text = project.incompleteTasks.Count.ToString();

        parentPanel = FindObjectOfType<AddProjectButtonLogic>().parentPanel.transform;
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            FindObjectOfType<Logic>().activeProject = project;
            FindObjectOfType<Logic>().newProject = false;
            GameObject newEditor = Instantiate(projectPanel, transform.position, Quaternion.identity);
            newEditor.transform.SetParent(parentPanel.transform);
            newEditor.transform.position = Input.mousePosition;
        });
    }

    // Update is called once per frame
    void Update()
    {
        title.text = project.title;
        progress.fillAmount = project.progress();
        taskCount.text = project.incompleteTasks.Count.ToString();
    }
}
