using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTaskBtnLogic : MonoBehaviour
{
    private Task task;
    // Start is called before the first frame update
    void Start()
    {
        if (task == null)
        {
            task = FindObjectOfType<ProjectPanelLogic>().activeTask;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
