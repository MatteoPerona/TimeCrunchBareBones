using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReActivateTaskLogic : MonoBehaviour
{
	public Task task;
	public Project project;
	public Button cancelBtn;
	public Button doneBtn;
	private CanvasGroup group;
	public float animTime = 0.15f;
	public Button descriptionToggle;
	public float dToggleTime = 0.25f;
	public RectTransform layoutContainer;
	public RectTransform dContainer;
	public TMP_InputField title;
	public TMP_InputField description;
	private bool descriptionOpen;

	void Start()
	{
		group = gameObject.GetComponent<CanvasGroup>();

		StartCoroutine(fadeCanvasGroup(group, animTime, 0, 1, false));

		cancelBtn.onClick.AddListener(delegate {
			StartCoroutine(fadeCanvasGroup(group, animTime, 1, 0));
		});
		doneBtn.onClick.AddListener(delegate {
			project.completedTasks.Remove(task);
			project.incompleteTasks.Add(task);
			FindObjectOfType<ProjectPanelLogic>().toggleTaskScroll();
			FindObjectOfType<ProjectPanelLogic>().resetTaskScroll();
			FindObjectOfType<ToggleLogic>().swapImages();
			StartCoroutine(fadeCanvasGroup(group, animTime, 1, 0));
		});

		descriptionToggle.gameObject.LeanRotateZ(180f, 0f);
		StartCoroutine(scaleHeightCorutine(0.0f, 1750f, 75f, 1f, 0.5f));

		descriptionToggle.onClick.AddListener(delegate { toggleDescription(); });
		description.onSelect.AddListener(delegate {
			if (!descriptionOpen)
			{
				toggleDescription();
			}
		});

		description.text = task.description;
		title.text = task.title;
	}

	void Update()
	{
		
	}

	IEnumerator fadeCanvasGroup(CanvasGroup group, float duration, float startAlpha, float endAlpha, bool destroyObj = true)
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
		if (destroyObj) { desroyMe(); }
	}

	void desroyMe()
	{
		Destroy(gameObject);
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
}
