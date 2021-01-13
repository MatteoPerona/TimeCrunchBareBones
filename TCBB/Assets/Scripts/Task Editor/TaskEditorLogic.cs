using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskEditorLogic : MonoBehaviour
{
    public Task task;
    public GameObject container;
    public Button doneButton;
    public Button dateUpBtn;
    public Button dateDownBtn;
    public TMP_InputField title;
    public TMP_InputField description;
    public TMP_InputField timeEstimate;
    public TMP_InputField dateInput;
    public Slider timeEstimateSlider;
    public System.DateTime date;

    // Start is called before the first frame update
    void Start()
    {
        if (task == null)
        {
            task = FindObjectOfType<ProjectPanelLogic>().activeTask;
            if (task.dateToDo == new System.DateTime()){task.dateToDo = System.DateTime.Now.Date.AddDays(1);}
        }
        title.text = task.title;
        description.text = task.description;
        timeEstimate.text = FindObjectOfType<TimeInput>().timeFromRawValue(task.timeEstimate);
        date = task.dateToDo;
        dateInput.text = date.ToString("d");

        doneButton.onClick.AddListener(delegate{save();});
    }

    void updateTimeEstimate()
    {
        
    }

    void save()
    {
        task.title = title.text;
        task.description = description.text;
        task.dateToDo = date;
        task.timeEstimate = FindObjectOfType<TimeInput>().rawValueFromTime(timeEstimate.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
