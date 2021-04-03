using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PeronaSorter : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	private Transform scrollContent;
	private PeronaScroll scroll;

	private Vector3 siblingPos;

	private int originIndex;
	private int deltaIndex;

	private Vector3 upperSibling;
	private Vector3 lowerSibling;

	private Vector3 highPos;
	private Vector3 lowPos;

	private float currentYPos;

	private bool upPossible;
	private bool downPossible;
	private bool holdOptsOpen;

	private Coroutine currentCR;
	private List<IEnumerator> routineQue;
	private bool awaiting = false;

	public float waitTime = 1.0f;
	public float animTime = 0.05f;


	void Start()
	{
		if (scrollContent == null)
		{
			scrollContent = transform.parent;
			scroll = scrollContent.GetComponent<PeronaScroll>();
		}

		if (routineQue == null)
		{
			routineQue = new List<IEnumerator>();
		}
	}


	void Update()
	{
		
	}


	public void OnBeginDrag(PointerEventData eventData)
	{
		holdOptsOpen = gameObject.GetComponent<HoldOptions>().holdOptsOpen;
		if (holdOptsOpen)
		{
			calculatePositions();
		}
	}


	void calculatePositions()
	{
		siblingPos = transform.position;

		originIndex = transform.GetSiblingIndex();
		deltaIndex = 0;

		upperSibling = scrollContent.GetChild(originIndex - 1).position;
		lowerSibling = scrollContent.GetChild(originIndex + 1).position;

		upPossible = scroll.upButton.gameObject.activeSelf;
		downPossible = scroll.downButton.gameObject.activeSelf;

		if (!upPossible)
		{
			Debug.Log("up button "+scroll.upButton.transform.position);
		}

		//Find High Pos
		for (int x = 0; x < scrollContent.childCount; x++)
		{
			Transform currentT = scrollContent.GetChild(x);
			bool isDefaultOb = isDefault(currentT);
			if (!isDefaultOb)
			{
				float height = currentT.GetComponent<RectTransform>().sizeDelta.y;
				highPos = new Vector3(currentT.position.x, currentT.position.y + height / 4, currentT.position.z);
				break;
			}
		}

		//Find Low Pos
		for (int x = scrollContent.childCount - 1; x >= 0; x--)
		{
			Transform currentT = scrollContent.GetChild(x);
			bool isDefaultOb = isDefault(currentT);
			if (!isDefaultOb)
			{
				float height = currentT.GetComponent<RectTransform>().sizeDelta.y;
				lowPos = new Vector3(currentT.position.x, currentT.position.y, currentT.position.z);
				Debug.Log(lowPos);
				break;
			}
		}
	}

	private bool isDefault(Transform t)
	{
		foreach (GameObject g in scroll.defaultObs)
		{
			if (t == g.transform)
			{
				return true;
			}
		}
		return false;
	}


	public void OnDrag(PointerEventData data) 
	{
		if (holdOptsOpen)
		{
			currentYPos = data.position.y;

			//sorting current layer
			if (currentYPos < highPos.y && currentYPos > lowPos.y)
			{
				transform.position = new Vector3(siblingPos.x, data.position.y, 0);

				if (currentYPos > upperSibling.y)// Up
				{
					Debug.Log("going up");

					deltaIndex -= 1;
					Transform target = scrollContent.GetChild(originIndex + deltaIndex);
					Vector3 tempSiblingPos = siblingPos;

					setPositionGlobal(deltaIndex, target);
					StartCoroutine(chainingRoutine(setPosition(target, tempSiblingPos, animTime)));
				}
				else if (currentYPos < lowerSibling.y)// Down
				{
					Debug.Log("going down");

					deltaIndex += 1;
					Transform target = scrollContent.GetChild(originIndex + deltaIndex);
					Vector3 tempSiblingPos = siblingPos;

					setPositionGlobal(deltaIndex, target);
					StartCoroutine(chainingRoutine(setPosition(target, tempSiblingPos, animTime)));
				}
			}
			//scroll down
			else if (currentYPos == lowPos.y && downPossible)
			{
				StartCoroutine(scrollAfterSeconds(waitTime));
			}
			//scroll up
			else if (currentYPos == highPos.y && upPossible)
			{
				StartCoroutine(scrollAfterSeconds(waitTime, false));
			}
		}
	}


	public void OnEndDrag(PointerEventData eventData)
	{
		StartCoroutine(chainingRoutine(setPosition(transform, siblingPos, animTime, true)));
	}


	public void setPositionGlobal(int dIndex, Transform a)
	{
		siblingPos = a.position;

		if (dIndex < 0) // Going Up
		{
			int us = dIndex + originIndex - 1;
			int ls = dIndex + originIndex;
			Debug.Log("US: " + us + ", LS: " + ls);

			upperSibling = scrollContent.GetChild(dIndex + originIndex - 1).position;
			lowerSibling = scrollContent.GetChild(dIndex + originIndex).position;
		}
		else if (dIndex > 0) // Going Down
		{
			int ls = dIndex + originIndex + 1;
			int us = dIndex + originIndex;
			Debug.Log("LS: " + ls + ", US: " + us);

			lowerSibling = scrollContent.GetChild(dIndex + originIndex + 1).position;
			upperSibling = scrollContent.GetChild(dIndex + originIndex).position;
		}
	}


	public IEnumerator setPosition(Transform a, Vector3 finalPos, float duration, bool final = false) // Testing this one
	{
		Vector3 posA = a.position;

		float time = 0.0f;
		while (time < duration)
		{
			a.position = Vector3.Lerp(posA, finalPos, (time / duration));

			yield return null;
			time += Time.deltaTime;
		}
		a.position = finalPos;

		if (final)
		{
			transform.SetSiblingIndex(originIndex + deltaIndex);
		}
	}


	public IEnumerator chainingRoutine(IEnumerator cr1)
	{
		routineQue.Add(cr1);
		if (!awaiting)
		{
			awaiting = true;
			currentCR = StartCoroutine(routineQue[0]);
			while (routineQue.Count > 0)
			{
				yield return currentCR;
				routineQue.RemoveAt(0);
				if (routineQue.Count > 0)
				{
					currentCR = StartCoroutine(routineQue[0]);
				}
			}
			awaiting = false;
		}
	}


	public IEnumerator scrollAfterSeconds(float duration, bool down = true)
	{
		bool scrollable = true;

		float time = 0.0f;
		while (time < duration)
		{
			if (down && currentYPos > lowerSibling.y)
			{
				scrollable = false;
				break;
			}
			else if (currentYPos < upperSibling.y)
			{
				scrollable = false;
				break;
			}

			yield return null;
			time += Time.deltaTime;
		}

		if (scrollable)
		{
			if (down)
			{
				scroll.scrollDown();
			}
			else
			{
				scroll.scrollUp();
			}
		}
	}
}
