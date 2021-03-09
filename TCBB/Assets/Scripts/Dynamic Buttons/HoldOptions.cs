using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldOptions : MonoBehaviour, IPointerDownHandler
{
	public bool holdOptsOpen;
	public float holdTime = 1f;
	public bool onProjectButton;
	public bool onTaskButton;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		StartCoroutine(holdCoroutine(holdTime));
	}

	public IEnumerator holdCoroutine(float duration)
	{
		float time = 0.0f;
		while (gameObject.GetComponent<ButtonAnimator>().pressed)
		{
			if (time >= duration)
			{
				if (onProjectButton) { gameObject.GetComponent<ProjectButton>().startHoldOptionsProcess(); }
				else if (onTaskButton) { gameObject.GetComponent<TaskButtonLogic>().startHoldOptionsProcess(); }
				else { gameObject.GetComponent<ToDoTaskLogic>().startHoldOptionsProcess(); }
				holdOptsOpen = true;
				break;
			}
			yield return null;
			time += Time.deltaTime;
		}
		StartCoroutine(checkDone());
	}

	public IEnumerator checkDone()
	{
		bool correctPress = false;
		float time = 0.0f;
		while (holdOptsOpen)
		{
			if (correctPress)
			{
				bool mainPressed = gameObject.GetComponent<Button>().GetComponent<ButtonAnimator>().pressed;
				bool delPressed;
				if (onProjectButton) { delPressed = gameObject.GetComponent<ProjectButton>().delete.GetComponent<ButtonAnimator>().pressed; }
				else if (onTaskButton) { delPressed = gameObject.GetComponent<TaskButtonLogic>().delete.GetComponent<ButtonAnimator>().pressed; }
				else { delPressed = gameObject.GetComponent<ToDoTaskLogic>().delete.GetComponent<ButtonAnimator>().pressed; }

				if (!mainPressed && !delPressed)
				{
					correctPress = false;
				}
			}
			else if (time >= 0)
			{
				if (Input.touchCount > 0)
				{
					
					bool mainPressed = gameObject.GetComponent<Button>().GetComponent<ButtonAnimator>().pressed;
					bool delPressed;
					if (onProjectButton) { delPressed = gameObject.GetComponent<ProjectButton>().delete.GetComponent<ButtonAnimator>().pressed; }
					else if (onTaskButton) { delPressed = gameObject.GetComponent<TaskButtonLogic>().delete.GetComponent<ButtonAnimator>().pressed; }
					else { delPressed = gameObject.GetComponent<ToDoTaskLogic>().delete.GetComponent<ButtonAnimator>().pressed; }
					if (mainPressed || delPressed)
					{
						correctPress = true;
					}
					else
					{
						
						if (onProjectButton) { gameObject.GetComponent<ProjectButton>().endHoldOptionsProcess(); }
						else if (onTaskButton) { gameObject.GetComponent<TaskButtonLogic>().endHoldOptionsProcess(); }
						else { gameObject.GetComponent<ToDoTaskLogic>().endHoldOptionsProcess(); }
						holdOptsOpen = false;
						break;
					}
				}
			}
			yield return null;
			time += Time.deltaTime;
		}
	}
}
