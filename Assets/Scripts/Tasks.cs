using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    public List<GameObject> ListPrefabs = new List<GameObject> ();
    public TextMeshProUGUI Task;


    public void Start()
    {
        Task.text = Task.text + "\n" + RandomNameObject();
    }
    public string RandomNameObject() 
    { 
        GameObject randomObj = ListPrefabs[Random.Range(0, ListPrefabs.Count)];
        return randomObj.GetComponent<InformationAboutObject>()._name;
    }
}