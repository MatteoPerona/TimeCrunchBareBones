using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project
{
    public string title;
    public string description;
    public List<Task> completedTasks = new List<Task>();
    public List<Task> incompleteTasks = new List<Task>();
    public int cacheIndex;

    public float progress()
    {
        float completedTaskCount = (float)completedTasks.Count;
        float incompleteTaskCount = (float)incompleteTasks.Count;
        if (completedTaskCount == 0)
        {
            return 0f;
        }
        return completedTaskCount/(incompleteTaskCount+completedTaskCount);
    }    
}
