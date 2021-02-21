using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public List<Project> activeProjects;
    public List<Project> finishedProjects;

    public UserData (List<Project> aP, List<Project> fP)
    {
        activeProjects = aP;
        finishedProjects = fP;
    }
}
