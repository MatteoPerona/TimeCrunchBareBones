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
	private int siblingIndex;
	private Vector3 siblingPos;
	private Vector3 upperSiblingPos;
	private Vector3 lowerSiblingPos;
	private Vector3 highPos;
	private Vector3 lowPos;
	private float currentYPos;
	private bool upPossible;
	private bool downPossible;
	private bool swappingObjects;
	public float waitTime = 1.0f;
	public float animTime = 0.25f;

	void Start()
	{
		if (scrollContent == null)
		{
			scrollContent = transform.parent;
			scroll = scrollContent.GetComponent<PeronaScroll>();
		}
	}

	void Update()
	{
		
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		calculatePositions(true);
	}

	void calculatePositions(bool init = false)
	{
		if (init)
		{
			siblingIndex = transform.GetSiblingIndex();
			siblingPos = transform.position;
		}

		upPossible = false;
		downPossible = false;

		upperSiblingPos = scrollContent.GetChild(siblingIndex - 1).position;
		lowerSiblingPos = scrollContent.GetChild(siblingIndex + 1).position;

		if (scroll.upButton.gameObject.activeSelf)
		{
			highPos = scroll.upButton.transform.position;
			upPossible = true;
		}
		else
		{
			highPos = scroll.defaultObs[scroll.defaultObs.Count - 3].transform.position;
		}
		if (scroll.downButton.gameObject.activeSelf)
		{
			lowPos = scroll.downButton.transform.position;
			downPossible = true;
		}
		else
		{
			Transform lastChild = scrollContent.GetChild(scrollContent.childCount-1);
			lowPos = lastChild.position;
			float halfHeight = lastChild.GetComponent<RectTransform>().sizeDelta.y/2;
			lowPos = new Vector3(0f, lowPos.y - halfHeight, 0f);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		currentYPos = data.position.y;
		Debug.Log(currentYPos+" "+transform.position.y+" "+highPos.y+" "+lowPos.y);
		if (!swappingObjects)
		{
			if (currentYPos < highPos.y && currentYPos > lowPos.y)
			{
				transform.position = new Vector3(siblingPos.x, data.position.y, 0);
				if (currentYPos > upperSiblingPos.y)
				{
					StartCoroutine(setPosition(siblingIndex - 1, scrollContent.GetChild(siblingIndex - 1), siblingPos, animTime));
				}
				else if (currentYPos < lowerSiblingPos.y)
				{
					StartCoroutine(setPosition(siblingIndex + 1, scrollContent.GetChild(siblingIndex + 1), siblingPos, animTime));
				}
			}
			else if (currentYPos < lowPos.y && downPossible)
			{
				StartCoroutine(scrollAfterSeconds(waitTime));
			}
			else if (currentYPos > highPos.y && upPossible)
			{
				StartCoroutine(scrollAfterSeconds(waitTime, false));
			}
		}
		else
		{
			transform.position = new Vector3(siblingPos.x, data.position.y, 0);
		}
	}

	public IEnumerator scrollAfterSeconds(float duration, bool down = true)
	{
		bool scrollable = true;

		float time = 0.0f;
		while (time < duration)
		{
			if (down && currentYPos > lowerSiblingPos.y)
			{
				scrollable = false;
				break;
			}
			else if (currentYPos < upperSiblingPos.y)
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

	public void OnEndDrag(PointerEventData eventData)
	{
		StartCoroutine(setPosition(siblingIndex, transform, siblingPos, animTime, true));
	}

	public IEnumerator setPosition(int newIndex, Transform a, Vector3 finalPos, float duration, bool final = false)
	{
		swappingObjects = true;
		Vector3 posA = a.position;

		float time = 0.0f;
		while (time < duration)
		{
			Vector3.Lerp(a.position, finalPos, (time / duration));
			LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
			
			yield return null;
			time += Time.deltaTime;
		}
		a.position = finalPos;
		if (!final)
		{
			siblingIndex = newIndex;
			siblingPos = posA;
			calculatePositions();
		}
		else
		{
			transform.SetSiblingIndex(siblingIndex);
		}

		swappingObjects = false;
	}
}
