using UnityEngine.InputSystem.Composites;
using static InputMapper;

public struct EventDescription
{
    public string name;
    public Modifier modifier;

    public EventDescription(string name)
    {
        this.name = name;
        modifier = Modifier.ANY; 
    }

    public EventDescription(string name,Modifier m)
    {
        this.name = name;  
        modifier = m;
    }

}
