using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleLogic : MonoBehaviour
{
    public Image inProgressIm;
    public Image completedIm;
    public float animTime = 0.06f;
    private Button button;
    private bool completedActive;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(delegate{
            if (completedActive)
            {
                StartCoroutine(changeImage(completedIm, inProgressIm, animTime));
                completedActive = false;
            }
            else
            {
                StartCoroutine(changeImage(inProgressIm, completedIm, animTime));
                completedActive = true;
            }
        });

        completedIm.color = new Color(completedIm.color.r, completedIm.color.g, completedIm.color.b, 0f);
    }

    public void swapImages()
	{
        if (completedActive)
        {
            StartCoroutine(changeImage(completedIm, inProgressIm, animTime));
            completedActive = false;
        }
        else
        {
            StartCoroutine(changeImage(inProgressIm, completedIm, animTime));
            completedActive = true;
        }
    }

    IEnumerator changeImage(Image im1, Image im2, float duration)
    {
        float time = 0.0f;
        AnimationCurve alphaCurve = AnimationCurve.Linear(time, 1f, duration, 0f);
        while (time < duration)
        {
            float currentAlpha = alphaCurve.Evaluate(time);
            im1.color = new Color(im1.color.r, im1.color.g, im1.color.b, currentAlpha);
            im2.color = new Color(im2.color.r, im2.color.g, im2.color.b, 1-currentAlpha);
            yield return null;
            time += Time.deltaTime;
        }
        im1.color = new Color(im1.color.r, im1.color.g, im1.color.b, 0f);
        im2.color = new Color(im2.color.r, im2.color.g, im2.color.b, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
