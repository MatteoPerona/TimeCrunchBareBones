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
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		loadSave();

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
	public void createProjectBtn(Project p)
	{
		GameObject newProjBtn = Instantiate(projectButton, transform.position, Quaternion.identity);
		newProjBtn.GetComponent<ProjectButton>().project = p;
		newProjBtn.transform.SetParent(scrollContent.transform);
		newProjBtn.transform.SetSiblingIndex(0);
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnApplicationQuit() 
	{
		SaveData.saveToJson(gameObject.GetComponent<Logic>());
	}

	void loadSave()
	{
		UserData data = SaveData.loadFromJson();
		activeProjects = data.activeProjects;
		finishedProjects = data.finishedProjects;
		
		foreach (Project p in activeProjects)
		{
			activeProject = p;
			createProjectBtn(p);
		}
		scrollContent.GetComponent<PeronaScroll>().findObjects();
	}
}
