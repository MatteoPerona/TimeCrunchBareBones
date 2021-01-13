using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeInput : MonoBehaviour
{
    public TMP_InputField input;
    public Slider slider;
    public float growthExponent = 3f;
    private int lenTime;

    // Start is called before the first frame update
    void Start()
    {
        lenTime = input.text.Length;

        slider.value = valueFromTime(input.text);

        input.onValueChanged.AddListener(delegate{
            updateCharInfo();
        });
        input.onSelect.AddListener(delegate{
            input.text = "";
        }); 
        input.onEndEdit.AddListener(delegate{
            autocomplete();
            slider.value = valueFromTime(input.text);
        }); 

        slider.onValueChanged.AddListener(delegate{
            updateTextFromSlider();    
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateCharInfo()
    {
        string t = input.text;
        if (t.Length >= 3 && t.Length-lenTime > 0)
        {
            if (t.Length == 3)
            {
                t = t.Insert(1, ":");
                input.text = t;
                input.MoveToEndOfLine(false, false);
            }
            else if (t.Length == 5)
            {
                lenTime = t.Length;
                string[] tArr = t.Split(':');
                t = tArr[0]+tArr[1].Substring(0,1)+":"+tArr[1].Substring(1);
                input.text = t;
                input.MoveToEndOfLine(false, false);
            }
        }
        else if (t.Length >= 2 && t.Length-lenTime < 0)
        {
            if (t.Length == 4)
            {
                lenTime = t.Length;
                string[] tArr = t.Split(':');
                t = tArr[0].Substring(0,1)+":"+tArr[0].Substring(1,1)+tArr[1].Substring(0);
                input.text = t;
                input.MoveToEndOfLine(false, false);
            }
            else if (t.Length == 3)
            {
                lenTime = t.Length;
                t = t.Remove(1, 1);
                input.text = t;
                input.MoveToEndOfLine(false, false);
            }
        }

        lenTime = t.Length;
    }

    public void autocomplete()
    {
        int diff = "00:00".Length-input.text.Length;
        if (diff > 0)
        {
            input.text = "00:00".Substring(0, diff) + input.text;
        }
    }

    public void updateTextFromSlider()
    {
        float v = slider.value;
        input.text = timeFromValue(v);
    }

    public float valueFromTime(string t)
    {
        string[] time = t.Split(':');
        float totalMins = float.Parse(time[0])*60 + float.Parse(time[1]);
        float v = totalMins/1440;
        v = Mathf.Pow(v, growthExponent);
        if (v > 1){v=1;}
        return v;
    }

    public float rawValueFromTime(string t)
    {
        string[] time = t.Split(':');
        float totalMins = float.Parse(time[0])*60 + float.Parse(time[1]);
        float v = totalMins/1440;
        return v;
    }

    public string timeFromValue(float v)
    {
        int totalMins = Mathf.RoundToInt(Mathf.Pow(v, 1/growthExponent)*1440f);
        int hours = Mathf.FloorToInt(totalMins/60f);
        int minutes = totalMins-hours*60;
        string time = hours.ToString("00")+":"+minutes.ToString("00");
        return time;
    }

    public string timeFromRawValue(float v)
    {
        int totalMins = Mathf.RoundToInt(v*1440f);
        int hours = Mathf.FloorToInt(totalMins/60f);
        int minutes = totalMins-hours*60;
        string time = hours.ToString("00")+":"+minutes.ToString("00");
        return time;
    }
}
