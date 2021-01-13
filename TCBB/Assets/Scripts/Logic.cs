using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    public Button addProjectBtn;
    public Project activeProject;
    public List<Project> activeProjects = new List<Project>();
    public List<Project> finishedProjects = new List<Project>();
    public GameObject scrollContent;
    public GameObject projectButton;
    public bool newProject;

    // Start is called before the first frame update
    void Start()
    {
        addProjectBtn.onClick.AddListener(delegate{
            newProject = true;
            addProject();
        });
    }

    void addProject()
    {
        activeProject = new Project();
        activeProjects.Add(activeProject);
    }
    public void createProjectBtn()
    {
        GameObject newProjBtn = Instantiate(projectButton, transform.position, Quaternion.identity);
        newProjBtn.transform.SetParent(scrollContent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
