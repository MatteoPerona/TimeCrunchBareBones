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
	public GameObject deleteQuestionPanel;
	public GameObject touchHoldOpts;
	public GameObject regularLayout;
	public Button delete;
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
	
	public void startHoldOptionsProcess()
	{
	}

	public void endHoldOptionsProcess()
	{
	}

	IEnumerator fadeCanvasGroup(CanvasGroup group, float duration, float startAlpha, float endAlpha)
	{
		float time = 0.0f;
		AnimationCurve curve = AnimationCurve.EaseInOut(time, startAlpha, duration, endAlpha);
		while (time < duration)
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
		GameObject newDeleteQ = Instantiate(deleteQuestionPanel, parentTransform.position, transform.rotation);
		newDeleteQ.transform.SetParent(parentTransform);
		newDeleteQ.transform.localPosition = new Vector3(0, 0, 0);
		StartCoroutine(fadeCanvasGroup(newDeleteQ.GetComponent<CanvasGroup>(), 0.25f, 0, 1));
		newDeleteQ.GetComponent<DeleteQPanelLogic>().task = task;
		newDeleteQ.GetComponent<DeleteQPanelLogic>().setProjectButton(gameObject);
	}

	public void destroyMe()
	{
		Destroy(gameObject);
	}
}
