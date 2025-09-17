using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using static InputMapper;

public class LightUp : MonoBehaviour
{
  private Color originalColor;
    private InputMapper mapper;
  private void Start()
  {
    Renderer r = GetComponent<Renderer>();
    Material m = r.material;
    originalColor = m.color;

        mapper = FindObjectsByType<InputMapper>(FindObjectsSortMode.None)[0];
  }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider name: "+ other.name);
        if (other.name == "Right Controller")
        {
            mapper.Register(Actions.LIGHT_ON, (DataEventHandler)this.OnLightOn);
            mapper.Register(Actions.LIGHT_OFF, (DataEventHandler)this.OnLightOff);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Right Controller")
        {
            mapper.DeRegister((DataEventHandler)this.OnLightOn);
            mapper.DeRegister((DataEventHandler)(this.OnLightOff));
        }
    }

    void OnLightOn(object info)
  {
    Renderer r = GetComponent<Renderer>();
    Material m = r.material;
    m.color = Color.white;
  }


  void OnLightOff(object info)
  {
    Renderer r = GetComponent<Renderer>();
    Material m = r.material;
    m.color = originalColor;
  }
}
