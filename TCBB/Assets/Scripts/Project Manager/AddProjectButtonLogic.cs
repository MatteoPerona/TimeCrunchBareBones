using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddProjectButtonLogic : MonoBehaviour
{
    public GameObject projectPanel;
    public GameObject parentPanel;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate{
            GameObject newEditor = Instantiate(projectPanel, transform.position, Quaternion.identity);
            newEditor.transform.SetParent(parentPanel.transform);
            newEditor.transform.position = Input.mousePosition;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
