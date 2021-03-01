using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewToggleLogic : MonoBehaviour
{
	public Button projectsBtn;
	public Button todayBtn;
	public float animTime;
	public GameObject projectsPanel;
	public GameObject todayPanel;

	// Start is called before the first frame update
	void Start()
	{
		projectsBtn.onClick.AddListener(delegate{
			StartCoroutine(swapButtons(projectsBtn.GetComponent<RectTransform>(), todayBtn.GetComponent<RectTransform>(), animTime));
			StartCoroutine(changePanels(projectsPanel.GetComponent<RectTransform>(), todayPanel.GetComponent<RectTransform>(), animTime));
		});
		todayBtn.onClick.AddListener(delegate{
			StartCoroutine(swapButtons(projectsBtn.GetComponent<RectTransform>(), todayBtn.GetComponent<RectTransform>(), animTime));
			StartCoroutine(changePanels(projectsPanel.GetComponent<RectTransform>(), todayPanel.GetComponent<RectTransform>(), animTime));
		});
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	IEnumerator swapButtons(RectTransform rect1, RectTransform rect2, float duration)
	{
		RectTransform r1 = rect1;
		RectTransform r2 = rect2;
		if (rect1.sizeDelta.x < rect2.sizeDelta.x)
		{
			r1 = rect2;
			r2 = rect1;
		}

		Vector2 size1 = r1.sizeDelta;
		Vector2 size2 = r2.sizeDelta;

		Vector3 pos1 = r1.localPosition;
		Vector3 pos2 = r2.localPosition;
		Vector3 pos3 = pos2;
		pos3.x = pos3.x*-1;

		float time = 0.0f;
		while (time < duration)
		{
			r1.sizeDelta = Vector2.Lerp(size1, size2, time/duration);
			r2.sizeDelta = Vector2.Lerp(size2, size1, time/duration);

			r1.localPosition = Vector2.Lerp(pos1, pos3, time/duration);
			r2.localPosition = Vector2.Lerp(pos2, pos1, time/duration);

			yield return null;
			time += Time.deltaTime;
		}
		r1.sizeDelta = size2;
		r2.sizeDelta = size1;

		r1.localPosition = pos3;
		r2.localPosition = pos1;
	}

	IEnumerator changePanels(RectTransform rect1, RectTransform rect2, float duration)
	{
		RectTransform r1 = rect1;
		RectTransform r2 = rect2;
		
		bool updateTodayScroll = false;
		if (rect1.localPosition.x < 0)
		{
			FindObjectOfType<PeronaScroll>().resetScroll();
			r1 = rect2;
			r2 = rect1;
			updateTodayScroll = true;
		}

		Vector3 pos1 = r1.localPosition;
		Vector3 pos2 = r2.localPosition;
		Vector3 pos3 = pos2;
		pos3.x = pos3.x*-1;

		float time = 0.0f;
		while (time < duration)
		{
			r1.localPosition = Vector2.Lerp(pos1, pos3, time/duration);
			r2.localPosition = Vector2.Lerp(pos2, pos1, time/duration);

			yield return null;
			time += Time.deltaTime;
		}
		r1.localPosition = pos3;
		r2.localPosition = pos1;

		if (updateTodayScroll)
		{
			FindObjectOfType<TodayLogic>().updateScroll(true);
			//FindObjectOfType<TodayLogic>().resetScroll();
		}
	}
}
