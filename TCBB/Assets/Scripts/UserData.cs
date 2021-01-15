using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public Project[] activeProjects;
    public Project[] finishedProjects;

    public UserData (Logic logic)
    {
        int aCount = logic.activeProjects.Count;
        int fCount = logic.finishedProjects.Count;

        activeProjects = new Project[aCount];
        finishedProjects = new Project[fCount];

        int i = 0;
        foreach (Project p in logic.activeProjects)
        {
            activeProjects[i] = p;
            i++;
        }
        i = 0;
        foreach (Project p in logic.finishedProjects)
        {
            finishedProjects[i] = p;
            i++;
        }
    }
}
