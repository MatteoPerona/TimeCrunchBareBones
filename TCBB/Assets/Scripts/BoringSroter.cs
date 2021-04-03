using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BoringSroter : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
	public float animTime = 0.08f;

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
		calculatePositions();
	}

	void calculatePositions()
	{
		siblingIndex = transform.GetSiblingIndex();
		siblingPos = transform.position;

		upperSiblingPos = scrollContent.GetChild(siblingIndex - 1).position;
		lowerSiblingPos = scrollContent.GetChild(siblingIndex + 1).position;

		upPossible = false;
		downPossible = false;

		//Find High Pos
		for (int x = 0; x < scrollContent.childCount; x++)
		{
			Transform currentT = scrollContent.GetChild(x);
			bool isDefaultOb = isDefault(currentT);
			if (!isDefaultOb)
			{
				float height = currentT.GetComponent<RectTransform>().sizeDelta.y;
				highPos = new Vector3(currentT.position.x, currentT.position.y + height/4, currentT.position.z);
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
				lowPos = new Vector3(currentT.position.x, currentT.position.y - height/4, currentT.position.z);
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
		currentYPos = data.position.y;
		Debug.Log(currentYPos + " " + transform.position.y + " " + highPos.y + " " + lowPos.y);
		if (!swappingObjects)
		{
			if (currentYPos < highPos.y && currentYPos > lowPos.y)
			{
				transform.position = new Vector3(siblingPos.x, data.position.y, 0);
				if (currentYPos > upperSiblingPos.y)
				{
					siblingIndex -= 1;
					transform.SetSiblingIndex(siblingIndex);
				}
				else if (currentYPos < lowerSiblingPos.y)
				{
					siblingIndex += 1;
					transform.SetSiblingIndex(siblingIndex);
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
