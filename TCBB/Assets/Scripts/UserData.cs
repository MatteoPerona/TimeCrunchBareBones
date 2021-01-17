using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public List<string> projectTitles;

    public UserData (Logic logic)
    {
        foreach (Project p in logic.finishedProjects)
        {
            projectTitles.Add(p.title);
        }
    }
}
