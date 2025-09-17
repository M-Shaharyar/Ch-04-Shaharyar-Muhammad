using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static InputMapper;

public class KeyboardMouseHandler : MonoBehaviour
{
    private InputMapper mapper;
    private List<KeyCode>held = new List<KeyCode>();
    private Dictionary<KeyCode,bool>keyState = new Dictionary<KeyCode,bool>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapper = GetComponent<InputMapper>();

        KeyCode[] codes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        foreach (KeyCode code in codes)
        {
            if(!keyState.ContainsKey(code))
                keyState.Add(code,false);
        }

        mapper.RegisterMap(InputType.DOWN + " " + KeyCode.Space, Actions.TELEPORT);
        mapper.RegisterMap(InputType.HELD + " " + KeyCode.Z, Actions.TOGGLE);

        EventDescription[] formatter = new EventDescription[] {
            new EventDescription(InputType.HELD + "Mouse0"),
            new EventDescription(InputType.MOVE + "Mouse"), };

        mapper.RegisterMap(formatter, Actions.PUSH_LEFT);

        formatter = new EventDescription[] { 
            new EventDescription(InputType.HELD + "Mouse1"),
            new EventDescription(InputType.MOVE + "Mouse") };

        mapper.RegisterMap(formatter, Actions.PUSH_RIGHT);

        mapper.RegisterMap(InputType.DOWN + " " + KeyCode.G,
            Actions.LIGHT_ON);
        mapper.RegisterMap(InputType.UP + " " + KeyCode.G,
            Actions.LIGHT_OFF);

    }

    // Update is called once per frame
    void Update()
    {
        mapper.StartFrame();
        KeyCode[] codes = (KeyCode[])System.Enum.GetValues (typeof(KeyCode));
        foreach (KeyCode code in codes)
        {
            if (Input.GetKeyDown(code))
            {
                mapper.OnDown(code.ToString());
                held.Add(code);
            }
            else if (Input.GetKeyUp(code))
            {
                mapper.OnUp(code.ToString());
                held.Remove(code);
            }
            else 
            {
                if (held.Contains(code))
                {
                    mapper.IsDown(code.ToString());
                }    
            }
        }
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        Vector3 mouseMove = new Vector3(x, y, 0);

        if(mouseMove.magnitude > 0.1)
        {
            mapper.OnMove("Move", mouseMove);
        }
    }
}
