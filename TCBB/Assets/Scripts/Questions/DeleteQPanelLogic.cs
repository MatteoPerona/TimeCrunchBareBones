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
    private GameObject projectButton;
    // Start is called before the first frame update
    void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>();
        cancelBtn.onClick.AddListener(delegate{
            StartCoroutine(fadeCanvasGroup(group, 0.15f, 1, 0));
        });
        doneBtn.onClick.AddListener(delegate{
            FindObjectOfType<Logic>().activeProjects.Remove(project);
            projectButton.GetComponent<ProjectButton>().destroyMe();
            StartCoroutine(fadeCanvasGroup(group, 0.15f, 1, 0));
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

    public void setProjectButton(GameObject b)
    {
        projectButton = b;
    }
}
