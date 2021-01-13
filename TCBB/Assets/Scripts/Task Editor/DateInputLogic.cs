using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DateInputLogic : MonoBehaviour
{
    public TMP_InputField input;
    public Button upBtn;
    public Button downBtn;
    public System.DateTime date;
    private int lenDate;
    public bool clicked;

    // Start is called before the first frame update
    void Start()
    {
        date = FindObjectOfType<TaskEditorLogic>().date;
        
        input.onSelect.AddListener(delegate{
            //input.text = "";
        });
        input.onValueChanged.AddListener(delegate{
            updateCharInfo();
        });
        input.onEndEdit.AddListener(delegate{
            autocomplete();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateCharInfo()
    {
        string t = input.text;

        if (t.Length == 6 || t.Length == 3)
        {
            if (t.Substring(t.Length-1, 1) != "/" && t.Length-lenDate > 0)
            {
                t = t.Insert(t.Length-1, "/");
                Debug.Log(t.Length-1);
                input.text = t;
                input.MoveToEndOfLine(false, false);
            }
        }

        lenDate = t.Length;
        FindObjectOfType<TaskEditorLogic>().date = date;
    }

    public void autocomplete()
    {

        int diff = date.ToString("d").Length-input.text.Length;
        if (diff > 0)
        {
            input.text += date.ToString("d").Substring(input.text.Length, diff);
        }
    }

    public IEnumerator modifyDateWithTime(Button b, bool add = true)
    {
        double i = -1;
        if(add)
        {
            i = 1;
        }
        date = date.AddDays(i);
        input.text = date.ToString("d");

        float growthCondition = 1;

        float time = 0.0f;

        while (clicked)
        {
            if (growthCondition == 2)
            {
                Debug.Log(growthCondition);
                growthCondition = 0;
                date = date.AddDays(i);
                input.text = date.ToString("d");
            }

            yield return null;
            time += Time.deltaTime;
            growthCondition += Time.deltaTime;
        }
        FindObjectOfType<TaskEditorLogic>().date = date;
    }
}
