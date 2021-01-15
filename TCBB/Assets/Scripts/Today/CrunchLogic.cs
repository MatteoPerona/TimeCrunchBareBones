using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrunchLogic : MonoBehaviour
{
    public Button crunchButton;
    public TMP_Text timer;
    public Button pausePlay;
    public Button addTimeBtn;
    public TMP_Text title;
    public TMP_Text description;
    public Task task;
    public Image playIm;
    public Image pauseIm;
    private bool playing;
    public Button doneBtn;
    public float time = 0.0f;
    public bool adderClicked;
    public GameObject completedQuestion;
    public float crunchTime = 0.25f;
    private float timePaused;

    void Start()
    {
        time = task.timeEstimate*86400;
        title.text = task.title;
        description.text = task.description;
        setTimeText(time);

        pauseIm.gameObject.SetActive(false);
        pausePlay.onClick.AddListener(delegate{
            if (playing) 
            {
                playing = false;
                playIm.gameObject.SetActive(true);
                pauseIm.gameObject.SetActive(false);
            }
            else 
            {
                playing = true;
                StartCoroutine(timerCoroutine());
                playIm.gameObject.SetActive(false);
                pauseIm.gameObject.SetActive(true);
            }
        });

        doneBtn.onClick.AddListener(delegate{
            task.timeEstimate = time/86400;
            FindObjectOfType<TodayLogic>().updateScroll();
        });

        crunchButton.onClick.AddListener(delegate{
            StartCoroutine(crunch(crunchTime));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void setTimeText(float t)
    {
        float temp = t;
        int hours = Mathf.FloorToInt(temp/3600);
        temp -= hours*3600;
        int minutes = Mathf.FloorToInt(temp/60);
        temp -= minutes*60;
        int seconds = Mathf.FloorToInt(temp);
        timer.text = hours.ToString("00")+":"+minutes.ToString("00")+":"+seconds.ToString("00");
    }

    IEnumerator timerCoroutine()
    {
        while (playing)
        {
            setTimeText(time);
            yield return null;
            time -= Time.deltaTime;
        }
        task.timeEstimate = time/86400;
    }
    
    IEnumerator stopwatchCoroutine()
    {
        float t = 0.0f;
        while (playing)
        {
            setTimeText(t);
            yield return null;
            t += Time.deltaTime;
        }
    }

    public IEnumerator addTime()
    {
        time += 1;
        setTimeText(time);
        float t = 0.0f;
        float tHold = 0.5f;
        while (adderClicked)
        {
            if (t >= tHold)
            {
                t = 0.0f;
                tHold *= 0.45f;
                time += 1;
                setTimeText(time);
            }
            yield return null;
            t += Time.deltaTime;
        }
    }

    public IEnumerator crunch(float duration, bool reverse = false, RectTransform rect = null)
    {
        Vector3 startPos = new Vector3(0, 2446, 0);
        Vector3 endPos = transform.localPosition;
        float exponent = 0.35f;
        if (reverse)
        {
            endPos = new Vector3(0, 2446, 0);
            startPos = transform.localPosition;
            exponent = 1/exponent;
        }
        else
        {
            GameObject newCompletedQuestion = Instantiate(completedQuestion, startPos, Quaternion.identity);
            newCompletedQuestion.transform.SetParent(transform.parent);
            rect = newCompletedQuestion.GetComponent<RectTransform>();
        }
        float t = 0.0f;
        while (t < duration)
        {
            rect.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Pow(t/duration, exponent));
            yield return null;
            t += Time.deltaTime;
        }
        rect.localPosition = endPos;
        if (reverse)
        {
            Destroy(rect.gameObject);
        }
    }

    public void destroyMe()
    {
        Destroy(gameObject);
    }
     
    private void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus)
        {
            Debug.Log("Paused");
            timePaused = Time.time;
        }
        else
        {
            Debug.Log("Unpaused");
            time -= Time.time-timePaused;
        }
    }
}
