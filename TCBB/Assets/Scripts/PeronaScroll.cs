using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PeronaScroll : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public List<GameObject> objects;
	public Button upButton;
	public Button downButton;
	public float threshold = 500.0f;
	private float animTime = .1f;
	private float scrollTime = .2f;
	public List<GameObject> defaultObs;
	private CanvasGroup group;
	private float contentHeight;
	private int nextIndex;
	private int currentIndex;
	private Vector3 startPos;
	private Vector3 startPosDrag;
	private float dy;
	private bool startPosSet;
	private bool hasCached = false;

	// Start is called before the first frame update
	void Start()
	{
		startPosSet = false;

		if (objects == null)
		{
			objects = new List<GameObject>();
		}

		if (defaultObs == null)
		{
			defaultObs = new List<GameObject>();
		}

		if (group == null)
		{
			group = gameObject.GetComponent<CanvasGroup>();
		}

		defaultObs.Add(upButton.gameObject);
		defaultObs.Add(downButton.gameObject);

		upButton.gameObject.SetActive(false);
		downButton.gameObject.SetActive(false);
		
		contentHeight = GetComponent<RectTransform>().sizeDelta.y;

		upButton.onClick.AddListener(delegate{
			scrollUp();
			pointersUp();
		});
		downButton.onClick.AddListener(delegate{
			scrollDown();
			pointersUp();
		});

		startPos = transform.position;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void findObjects()
	{
		objects.Clear();
		for (int i=0; i<transform.childCount; i++)
		{
			bool isDefaultOb = false;
			foreach (GameObject defaultOb in defaultObs)
			{
				if (transform.GetChild(i).gameObject == defaultOb)
				{
					isDefaultOb = true;
					break;
				}
			}
			if (!isDefaultOb)
			{
				objects.Add(transform.GetChild(i).gameObject);
			}
		}
		int index = 0;
		foreach (GameObject o in defaultObs)
		{
			o.transform.SetSiblingIndex(index);
			index++;
		}

		if (hasCached)
			loadObjectsFromCache();
		else if (objects.Count > 0)
			Debug.Log(objects.Count);
			foreach (GameObject g in objects)
				if (g == defaultObs[0])
					Debug.Log("bad");
			cacheObjectsOrder();
			hasCached = true;

		resetScroll();
	}

	public void resetScroll()
	{
		foreach (GameObject g in defaultObs)
		{
			g.SetActive(true);
		}
		upButton.gameObject.SetActive(false);
		downButton.gameObject.SetActive(false);
		if (objects.Count > 0)
		{
			updateScroll(objects);
		}
	}

	public void scrollDown(GameObject protectedObject = null)
	{
		foreach (GameObject g in objects)
		{
			if (g != protectedObject)
				g.SetActive(false);
		}
		foreach (GameObject g in defaultObs)
		{
			g.SetActive(false);
		}
		updateScroll(objects.GetRange(nextIndex, objects.Count-nextIndex));
	}

	public void scrollUp(GameObject protectedObject = null)
	{
		foreach (GameObject g in objects)
		{
			if (g != protectedObject)
				g.SetActive(false);
		}
		
		int startOffset = 0;
		for (int i=currentIndex-1; i>=0; i--)
		{
			if (objectsHeight(objects.GetRange(i,currentIndex-i)) > contentHeight)
			{
				break;
			}
			else
			{
				startOffset++;
			}
		}

		if (currentIndex-startOffset == 0)
		{
			foreach (GameObject g in defaultObs)
			{
				g.SetActive(true);
			}
			upButton.gameObject.SetActive(false);
		}
		nextIndex = currentIndex;
		updateScroll(objects.GetRange(currentIndex-startOffset, startOffset));
	}
	
	public void updateScroll(List<GameObject> obs)
	{
		currentIndex = objects.IndexOf(obs[0]);
		int count = 0;
		for (int i=0; i<obs.Count; i++)
		{
			if (objectsHeight(obs.GetRange(0,i+1)) > contentHeight)
			{
				foreach (GameObject o in obs.GetRange(i, obs.Count-i))
				{
					o.SetActive(false);
				}
				nextIndex = objects.IndexOf(obs[i]);
				break;
			}
			else
			{
				obs[i].SetActive(true);
				count++;
			}
		}
		checkMoveButtons(obs.GetRange(0, count));
	}

	void checkMoveButtons(List<GameObject> obs)
	{
		if (obs.Contains(objects[0]) && obs.Contains(objects[objects.Count-1]))
		{
			upButton.gameObject.SetActive(false);
			downButton.gameObject.SetActive(false);
		}
		else if (obs.Contains(objects[0]))
		{
			upButton.gameObject.SetActive(false);
			downButton.gameObject.SetActive(true);
		}
		else if (obs.Contains(objects[objects.Count-1]))
		{
			upButton.gameObject.SetActive(true);
			downButton.gameObject.SetActive(false);
		}
		else
		{
			upButton.gameObject.SetActive(true);
			downButton.gameObject.SetActive(true);
		}
		upButton.transform.SetAsFirstSibling();
		downButton.transform.SetAsLastSibling();
	}

	public float objectsHeight(List<GameObject> obs)
	{
		VerticalLayoutGroup layoutVertical = gameObject.GetComponent<VerticalLayoutGroup>();
		float h = layoutVertical.padding.bottom + layoutVertical.padding.top + (obs.Count+1)*layoutVertical.spacing;
		float hI = h;

		foreach (GameObject g in defaultObs)
		{
			if (g.activeSelf)
			{
				h += g.GetComponent<RectTransform>().sizeDelta.y;
				//count++;
			}
		}

		foreach (GameObject o in obs)
		{
			h += o.GetComponent<RectTransform>().sizeDelta.y;
		}
		
		return h;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!startPosSet)
		{
			startPos = transform.position;
			startPosSet = true;
		}
		startPosDrag = eventData.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
		dy = eventData.position.y-startPosDrag.y;
		
		Vector3 currentPos = new Vector3(startPos.x, startPos.y+50*dy/threshold, startPos.z);
		transform.position = currentPos;

		if (dy>0 && downButton.gameObject.activeSelf)
		{
			downButton.gameObject.GetComponent<CanvasGroup>().alpha = dy/threshold+0.2f;
		}
		else if (dy<0 && upButton.gameObject.activeSelf)
		{
			upButton.gameObject.GetComponent<CanvasGroup>().alpha = -1*dy/threshold+0.2f;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		
		pointersUp();
		if (dy > threshold && downButton.gameObject.activeSelf)
		{
			StartCoroutine(scrollRoutine(animTime));
		}
		else if (-1*dy > threshold && upButton.gameObject.activeSelf)
		{
			StartCoroutine(scrollRoutine(animTime, false));
		}
		else
		{
			StartCoroutine(smoothMove(transform, animTime, transform.position, startPos));
		}
		downButton.gameObject.GetComponent<CanvasGroup>().alpha = 0.2f;
		upButton.gameObject.GetComponent<CanvasGroup>().alpha = 0.2f; 
	}

	IEnumerator smoothMove(Transform t, float duration, Vector3 iPos, Vector3 fPos)
	{
		float time = 0.0f;
		while (time < duration)
		{
			t.position = Vector3.Lerp(iPos, fPos, Mathf.Pow(time/duration, 1));

			yield return null;
			time += Time.deltaTime;
		}
		t.position = fPos;
	}

	void pointersUp()
	{
		ButtonAnimator[] animators = FindObjectsOfType<ButtonAnimator>();
		foreach (ButtonAnimator a in animators)
		{
			StartCoroutine(a.pointerActuallyUp());
		}
	}

	IEnumerator fadeOutFadeIn(CanvasGroup group, float duration, float initial, float final)
	{
		float time = 0.0f;
		AnimationCurve curve = AnimationCurve.EaseInOut(time, initial, duration, final);
		
		while(time < duration)
		{
			float currentAlpha = curve.Evaluate(time);
			group.alpha = currentAlpha;
			yield return null;
			time += Time.deltaTime;
		}
		group.alpha = final;
	}

	public IEnumerator scrollRoutine(float duration, bool down = true, GameObject protectedObject = null)
	{
		if (!startPosSet)
		{
			startPos = transform.position;
			startPosSet = true;
		}

		float anchor = 1000;
		if (!down)
		{
			anchor *= -1;
		}
		Vector3 fPos = new Vector3(startPos.x, startPos.y+anchor, startPos.z);
		Vector3 inverseFPos = new Vector3(startPos.x, startPos.y-anchor, startPos.z);

		StartCoroutine(fadeOutFadeIn(group, scrollTime, 1, 0));
		yield return StartCoroutine(smoothMove(transform, scrollTime, transform.position, fPos));

		if (down) {scrollDown(protectedObject);}
		else {scrollUp(protectedObject);}

		StartCoroutine(fadeOutFadeIn(group, scrollTime, 0, 1));
		yield return StartCoroutine(smoothMove(transform, scrollTime, inverseFPos, startPos));
	}

	public void swapObjects(GameObject g1, GameObject g2)
	{
		int index1 = objects.IndexOf(g1);
		int index2 = objects.IndexOf(g2);

		objects[index1] = g2;
		objects[index2] = g1;

		cacheObjectsOrder();
	}

	public void moveObjects(GameObject g1, int swapAmount)
	{
		int index1 = objects.IndexOf(g1);
		int index2 = index1 + swapAmount;
		GameObject g2 = objects[index2];

		objects[index1] = g2;
		objects[index2] = g1;

		cacheObjectsOrder();
	}

	public void cacheObjectsOrder()
	{
		try
		{
			foreach (GameObject g in objects)
				g.GetComponent<ProjectButton>().project.cacheIndex = objects.IndexOf(g);
		}
		catch
		{
			try
			{
				foreach (GameObject g in objects)
					g.GetComponent<TaskButtonLogic>().task.cacheIndexTask = objects.IndexOf(g);
			}
			catch
			{
				foreach (GameObject g in objects)
					g.GetComponent<ToDoTaskLogic>().task.cacheIndexToDo = objects.IndexOf(g);
			}
		}
		/*catch 
		{
			foreach (GameObject g in objects)
				g.GetComponent<ToDoTaskLogic>().task.cacheIndexToDo = objects.IndexOf(g);
		}*/
	}

	public void loadObjectsFromCache()
	{
		List<GameObject> temp = objects;
		foreach (GameObject g in temp)
			try
			{
				objects[g.GetComponent<ProjectButton>().project.cacheIndex] = g;
			}
			catch (System.Exception e)
			{
				objects[g.GetComponent<TaskButtonLogic>().task.cacheIndexTask] = g;
			}
			catch
			{
				objects[g.GetComponent<ToDoTaskLogic>().task.cacheIndexToDo] = g;
			}
	}
}
