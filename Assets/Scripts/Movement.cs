using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static InputMapper;

public class Movement : MonoBehaviour
{
    private GameObject player;
    private List<GameObject> nodes = new List<GameObject>();
    private int location = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach(Transform child in transform)
        {
            nodes.Add(child.gameObject);
        }
        InputMapper mapper = FindObjectsByType<InputMapper>(FindObjectsSortMode.None)[0];
        mapper.Register(Actions.TELEPORT,
        (DataEventHandler)this.Teleport);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Teleport(object info)
    {
        location = (location + 1) % nodes.Count;
        player.transform.position = nodes[location].transform.position;
        player.transform.localRotation = nodes[location].transform.localRotation;
    }
}
