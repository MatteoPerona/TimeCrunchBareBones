using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskButtonLogic : MonoBehaviour
{
	public TMP_Text title;
	public GameObject taskEditor;
	public GameObject reActivateQuestion;
	public bool isComplete;
	private Task task;
	private Transform parentTransform;

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
		parentTransform = FindObjectOfType<AddProjectButtonLogic>().parentPanel.transform;

		if (task == null)
		{
			task = FindObjectOfType<ProjectPanelLogic>().activeTask;
		}
		if (!isComplete)
		{
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				FindObjectOfType<ProjectPanelLogic>().newTask = false;
				FindObjectOfType<ProjectPanelLogic>().activeTask = task;
				GameObject newTaskEditor = Instantiate(taskEditor, transform.position, Quaternion.identity);
				newTaskEditor.transform.SetParent(FindObjectOfType<ProjectPanelLogic>().gameObject.transform.parent);
				newTaskEditor.transform.position = Input.mousePosition;
			});
		}
		else
		{
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				GameObject reActivateQ = Instantiate(reActivateQuestion, parentTransform.position, Quaternion.identity);
				reActivateQ.transform.SetParent(parentTransform);
				reActivateQ.GetComponent<ReActivateTaskLogic>().task = task;
				reActivateQ.GetComponent<ReActivateTaskLogic>().project = FindObjectOfType<ProjectPanelLogic>().project;
			});
		}
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
