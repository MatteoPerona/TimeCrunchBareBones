using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeleteQPanelLogic : MonoBehaviour
{
    public Project project;
    public Task task;
    public Button cancelBtn;
    public Button doneBtn;
    private CanvasGroup group;
    private GameObject targetButton;
    // Start is called before the first frame update
    void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>();
        cancelBtn.onClick.AddListener(delegate{
            StartCoroutine(fadeCanvasGroup(group, 0.15f, 1, 0));
        });
        doneBtn.onClick.AddListener(delegate{
            if (task == null)
			{
                FindObjectOfType<Logic>().activeProjects.Remove(project);
                targetButton.GetComponent<ProjectButton>().destroyMe();
                StartCoroutine(fadeCanvasGroup(group, 0.15f, 1, 0));
            }
            else
			{
                if (targetButton.GetComponent<TaskButtonLogic>().isComplete)
				{
                    project.completedTasks.Remove(task);
				}
                else
				{
                    project.incompleteTasks.Remove(task);
                    FindObjectOfType<ProjectPanelLogic>().numTasks.text = project.incompleteTasks.Count.ToString();
                }
                targetButton.GetComponent<TaskButtonLogic>().destroyMeCompletely();
                StartCoroutine(fadeCanvasGroup(group, 0.15f, 1, 0));
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator fadeCanvasGroup(CanvasGroup group, float duration, float startAlpha, float endAlpha)
    {
        float time = 0.0f;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, startAlpha, duration, endAlpha);
        while(time < duration)
        {
            float currentAlpha = curve.Evaluate(time);
            group.alpha = currentAlpha;

            yield return null;
            time += Time.deltaTime;
        }
        group.alpha = endAlpha;
        desroyMe();
    }

    void desroyMe()
    {
        Destroy(gameObject);
    }

    public void setTargetButton(GameObject b)
    {
        targetButton = b;
    }
}
