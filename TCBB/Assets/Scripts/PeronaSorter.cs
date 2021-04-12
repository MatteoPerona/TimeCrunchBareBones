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

	private Vector3 ogPos;
	private int originIndex;

	private Transform upperSibling;
	private Transform lowerSibling;

	private Vector3 highPos;
	private Vector3 lowPos;

	private bool upPossible;
	private bool downPossible;

	private bool dragging;
	private bool ending;
	private bool scrolling;

	private bool upperIsDefault;
	private bool lowerIsDefault;

	public float waitTime = 0.5f;
	public float animTime = 0.05f;


	void Start()
	{
		if (scrollContent == null)
		{
			scrollContent = transform.parent;
			scroll = scrollContent.GetComponent<PeronaScroll>();
		}

		updateState();
		calculateAnchors();
	}


	void Update()
	{
	}


	public void OnBeginDrag(PointerEventData eventData)
	{
		calculateAnchors();
	}


	public void updateState()
	{
		ogPos = transform.position;
		originIndex = transform.GetSiblingIndex();

		for (int x = originIndex - 1; x >= 0; x--)
		{
			upperSibling = scrollContent.GetChild(x);
			if (upperSibling.gameObject.activeSelf)
			{
				break;
			}
		}

		for (int x = originIndex + 1; x <= scrollContent.childCount - 1; x++)
		{
			lowerSibling = scrollContent.GetChild(x);
			if (lowerSibling.gameObject.activeSelf)
			{
				break;
			}
		}

		upperIsDefault = false;
		lowerIsDefault = false;
		foreach (GameObject g in scroll.defaultObs)
		{
			if (upperSibling.gameObject == g)
			{
				upperIsDefault = true;
			}
			else if (lowerSibling.gameObject == g)
			{
				lowerIsDefault = true;
			}
		}
	}


	public void OnDrag(PointerEventData eventData)
	{
		if (!dragging && !ending)
		{
			StartCoroutine(OnDragRoutine(eventData));
		}
	}

	public IEnumerator OnDragRoutine(PointerEventData eventData)
	{
		dragging = true;
		if (eventData.position.y > lowPos.y && eventData.position.y < highPos.y)
		{
			Vector3 deltaPos = new Vector3(ogPos.x, eventData.position.y, ogPos.z);
			transform.position = deltaPos;

			if (deltaPos.y > upperSibling.position.y && !upperIsDefault)
			{
				yield return StartCoroutine(smoothMove(0.01f, ogPos, upperSibling));
				scroll.swapObjects(gameObject, upperSibling.gameObject);
				transform.SetSiblingIndex(upperSibling.GetSiblingIndex());
				LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
				updateState();
			}
			else if (deltaPos.y < lowerSibling.position.y && !lowerIsDefault)
			{
				yield return StartCoroutine(smoothMove(0.01f, ogPos, lowerSibling));
				scroll.swapObjects(gameObject, lowerSibling.gameObject);
				transform.SetSiblingIndex(lowerSibling.GetSiblingIndex());
				LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
				updateState();
			}

			if (!scrolling)
			{
				if (deltaPos.y >= highPos.y - 100 && upPossible)
				{
					StartCoroutine(scrollRoutine());
				}
				else if (deltaPos.y <= lowPos.y + 100 && downPossible)
				{
					StartCoroutine(scrollRoutine(false));
				}
			}
		}

		dragging = false;
	}


	public IEnumerator scrollRoutine(bool up = true)
	{
		scrolling = true;
		bool scrollable = true;

		float time = 0.0f;
		while (time < 0.5f)
		{
			if (up)
			{
				if (transform.position.y < highPos.y - 100 && upPossible)
				{
					scrollable = false;
					break;
				}
			}
			else
			{
				if (transform.position.y > lowPos.y + 100 && downPossible)
				{
					scrollable = false;
					break;
				}
			}
			yield return null;
			time += Time.deltaTime;
		}
		if (up && scrollable)
		{
			scroll.moveObjects(gameObject, -1);
			yield return StartCoroutine(scroll.scrollRoutine(0.2f, false, gameObject));
			calculateAnchors();
			updateState();
		}
		else if (scrollable)
		{
			scroll.moveObjects(gameObject, 1);
			yield return StartCoroutine(scroll.scrollRoutine(0.2f, true, gameObject));
			calculateAnchors();
			updateState();
		}

		StartCoroutine(EndDrag());
		scrolling = false;
	}


	public void OnEndDrag(PointerEventData eventData)
	{
		StartCoroutine(EndDrag());
	}

	public IEnumerator EndDrag()
	{
		ending = true;

		while (dragging)
		{
			yield return null;
		}
		yield return StartCoroutine(smoothMove(animTime, ogPos, transform));
		transform.SetSiblingIndex(originIndex);
		LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
		updateState();

		ending = false;
	}


	public IEnumerator smoothMove(float duration, Vector3 finalPos, Transform b)
	{
		Vector3 bPos = b.position;

		float time = 0.0f;
		while (time < duration)
		{
			b.position = Vector3.Lerp(bPos, finalPos, time / duration);
			yield return null;
			time += Time.deltaTime;
		}
		b.position = finalPos;
	}


	public void calculateAnchors()
	{
		upPossible = scroll.upButton.gameObject.activeSelf;
		downPossible = scroll.downButton.gameObject.activeSelf;

		//Find High Pos
		if (upPossible)
		{
			highPos = scroll.upButton.transform.position;
		}
		else
		{
			for (int x = 0; x < scrollContent.childCount-1; x++)
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
		}

		//Find Low Pos
		if (downPossible)
		{
			lowPos = scroll.downButton.transform.position;
		}
		else
		{
			for (int x = scrollContent.childCount - 1; x >= 0; x--)
			{
				Transform currentT = scrollContent.GetChild(x);
				bool isDefaultOb = isDefault(currentT);
				if (!isDefaultOb && currentT.gameObject.activeSelf)
				{
					float height = currentT.GetComponent<RectTransform>().sizeDelta.y;
					lowPos = new Vector3(currentT.position.x, currentT.position.y - height / 4, currentT.position.z);
					break;
				}
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
}
