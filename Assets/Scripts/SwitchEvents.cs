using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static InputMapper;

public class SwitchEvents : MonoBehaviour
{
    private List<GameObject> nodes = new List<GameObject>();
    private int location = 0;
    [SerializeField]
    private float timeDelay = 1f;
    private float lastCall = 0;

    /// <summary>
    /// When called, calculate the amount of time that has passed.
    /// Flip one switch at a time for each timeout
    /// </summary>
    /// 

    public void OnToggle(object info)
    {
        // on each timeout, toggle another switch    
        if(Time.realtimeSinceStartup - lastCall > timeDelay)
        {
            // toggle is just a rotation
            GameObject obj = nodes[location];
            obj.transform.localRotation = Quaternion.Euler(60,0,0);
            location = (location +1) % nodes.Count;
            lastCall = Time.realtimeSinceStartup;
        }
    }

    void Start()
    {
        foreach(Transform child in transform)
        {
            nodes.Add(child.gameObject);
        }

        InputMapper mapper = FindObjectsByType<InputMapper>(FindObjectsSortMode.None)[0];
        mapper.Register(Actions.TOGGLE, (DataEventHandler)OnToggle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
