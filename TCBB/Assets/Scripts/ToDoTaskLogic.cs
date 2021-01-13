﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToDoTaskLogic : MonoBehaviour
{
    public Task task;
    public TMP_Text title;
    private Button button;
    public GameObject crunchScreen;

    // Start is called before the first frame update
    void Start()
    {
        title.text = task.title;

        updateHeight();

        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate{
            openCrunch();
        });
    }

    // Update is called once per frame
    void Update()
    {
        title.text = task.title;
    }

    public void updateHeight()
    {
        //30 min = min height
        //maxTime/30min = 48
        float size = task.timeEstimate*48;
        float h = 150*size;
        if (h < 150) {h = 150;}
        else if (h > 1500) {h = 1500;}
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  h);
    }

    void openCrunch()
    {
        GameObject newCrunch = Instantiate(crunchScreen, Input.mousePosition, Quaternion.identity);
        newCrunch.GetComponent<CrunchLogic>().task = task;
        newCrunch.transform.SetParent(FindObjectOfType<TodayLogic>().transform); 
    }
}
