using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InputMapper : MonoBehaviour
{
    public enum Actions { TELEPORT,TOGGLE,PUSH_LEFT,PUSH_RIGHT,LIGHT_ON,LIGHT_OFF}
    public enum InputType { UP,DOWN,HELD,MOVE}
    public enum Modifier { ANY, LEFT, RIGHT }
    public delegate void DataEventHandler(object vals);
    private List<Tuple<Actions, Delegate>> actions = new List<Tuple<Actions, Delegate>>();
    private List<EventCombo> mapping = new List<EventCombo>();

    public struct EventCombo
    {
        public EventDescription[] format;
        public bool[] met;
        public Actions action;
        public object[] state;
        public EventCombo(EventDescription[] format, Actions action)
        {
            this.format = format;
            this.action = action;
            this.met = new bool[format.Length];
            this.state = new object[format.Length];
        }
    }

    /// <summary>
    /// Register this function to be called back when an
    /// meaningful action occurs
    /// </summary>
    /// <param name="a"> the action </param>
    /// <param name="func"> the callback </param>
    /// 
    public void Register(Actions a ,Delegate func)
    {
        actions.Add(new Tuple<Actions, Delegate>(a, func));
    }

    /// <summary>
    /// Remove the request to have a callback to this function
    /// </summary>
    /// <param name="func"> The function no longer to be called </param>
    
    public void DeRegister(Delegate func)
    {
        Tuple<Actions,Delegate>t = null;
        foreach(Tuple<Actions,Delegate>map in actions)
        {
            if (map.Item2 == func)
            {
                t = map;
            }
        }
        if(t != null)
            actions.Remove(t);
    }

    /// <summary>
    /// Register a device input event to an actoin meaning
    /// </summary>
    /// <param name="inputName"></param>
    /// <param name="a"></param>
    /// 
    public void RegisterMap(string inputName,Actions a)
    {
        EventDescription[] format = { new EventDescription(inputName) };
        EventCombo combo = new EventCombo(format, a);
        mapping.Add(combo);
    }
    /// <summary>
    /// Register a modifed device input event to an action meaning
    /// </summary>
    /// <param name="inputName"></param>
    /// <param name="a"></param>
    public void RegisterMap(EventDescription[] formats, Actions a)
    {
        EventCombo combo = new EventCombo(formats, a);
        mapping.Add(combo);
    }
    private void CheckForEvents(string name, Modifier m)
    {
        CheckForEvents(new EventDescription(name, m));
    }
    private void CheckForEvents(EventDescription inputEvent, object state = null)
    {
        // part 1: update all mappings
        foreach (EventCombo combo in mapping)
        {
            for (int i = 0; i < combo.format.Length; i++)
            {
                EventDescription c = combo.format[i];
                // found a match
                if ((inputEvent.name.ToLower() == c.name.ToLower() && inputEvent.modifier == c.modifier)
                || (inputEvent.name.ToLower() == c.name.ToLower() && c.modifier == Modifier.ANY))
                {
                    combo.met[i] = true;
                    combo.state[i] = state;
                }
            }
        }
        // part 2: check if any mappings completed
        foreach (EventCombo combo in mapping)
        {
            // will be true only if all met values are true
            bool complete = true;
            for (int i = 0; i < combo.met.Length; i++)
            {
                complete = complete && combo.met[i];
            }
            if (complete)
            {
                // part 3: original check to find all objects that want to know about this event
                foreach (Tuple<Actions, Delegate> actions in actions)
                {
                    if (combo.action == actions.Item1)
                    {
                        object info = combo.state[combo.state.Length - 1];
                        actions.Item2.DynamicInvoke(info);
                        //mark event as completed
                        combo.met[0] = false;
                    }
                }
            }
        }
    }

    public void OnDown(string buttonName,Modifier m = Modifier.ANY)
    {
        CheckForEvents(InputType.DOWN + " " + buttonName, m);
    }
    public void OnUp(string buttonName,Modifier m = Modifier.ANY)
    {
        CheckForEvents(InputType.UP + " "+ buttonName, m);
    }
    public void IsDown(string buttonName,Modifier m = Modifier.ANY)
    {
        CheckForEvents(InputType.HELD+ " "+ buttonName, m);
    }
    public void OnMove(string buttonName,Vector3 axis, Modifier m = Modifier.ANY)
    {
        //CheckForEvents(InputType.MOVE+ " "+ buttonName, m);
        CheckForEvents(new EventDescription(InputType.MOVE + " " + buttonName, m), axis);
    }
    public void StartFrame()
    {
        foreach (EventCombo c in mapping)
        {
            for (int i = 0; i < c.met.Length; i++)
            {
                c.met[i] = false;
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
