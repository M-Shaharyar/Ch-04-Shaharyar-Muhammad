using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static InputMapper;

public class VRHandler : MonoBehaviour
{
    private InputMapper mapper;
    private List<InputDevice> controllers = new List<InputDevice>();
    private Dictionary<string, bool> buttonStateRight = new Dictionary<string, bool>();
    private Dictionary<string, bool> buttonStateLeft = new Dictionary<string, bool>();

    private void Awake()
    {
        mapper = GetComponent<InputMapper>();
        // get all button codes. These are not in an enum, so they have to be done by hand
        AddInputFeature(CommonUsages.grip.name);
        AddInputFeature(CommonUsages.gripButton.name);
        AddInputFeature(CommonUsages.menuButton.name);
        AddInputFeature(CommonUsages.primary2DAxis.name);
        AddInputFeature(CommonUsages.primary2DAxisClick.name);
        AddInputFeature(CommonUsages.primary2DAxisTouch.name);
        AddInputFeature(CommonUsages.primaryButton.name);
        AddInputFeature(CommonUsages.primaryTouch.name);
        AddInputFeature(CommonUsages.secondary2DAxis.name);
        AddInputFeature(CommonUsages.secondary2DAxisClick.name);
        AddInputFeature(CommonUsages.secondary2DAxisTouch.name);
        AddInputFeature(CommonUsages.secondaryButton.name);
        AddInputFeature(CommonUsages.secondaryTouch.name);
        AddInputFeature(CommonUsages.trigger.name);
        AddInputFeature(CommonUsages.triggerButton.name);
        // register callback to update if controller lost\added
        InputDevices.deviceConnected += deviceRecheck;
        InputDevices.deviceDisconnected += deviceRecheck;

        mapper.RegisterMap("down " + CommonUsages.primary2DAxisClick.name, Actions.TELEPORT);
        mapper.RegisterMap("held " + CommonUsages.triggerButton.name,Actions.TELEPORT);

        mapper.RegisterMap("DOWN " + CommonUsages.primaryButton.name, Actions.LIGHT_ON);
        mapper.RegisterMap("UP " + CommonUsages.primaryButton.name, Actions.LIGHT_OFF);
    }
    void deviceRecheck(InputDevice device)
    {
        // just refresh...slow but easy
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.HeldInHand
        | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(characteristics, controllers);
    }
    void AddInputFeature(String usage)
    {
        buttonStateLeft.Add(usage, false);
        buttonStateRight.Add(usage, false);
    }

    private void CheckButtons(Modifier v, InputDevice device)
    {
        List<InputFeatureUsage> supportedFeatures = new List<InputFeatureUsage>();
        device.TryGetFeatureUsages(supportedFeatures);

        // switch to right hand if needed
        Dictionary<string, bool> pastControllerState = buttonStateLeft;
        if (v == Modifier.RIGHT)
        {
            pastControllerState = buttonStateRight;
        }

        //Poll Buttons
        foreach (InputFeatureUsage feature in supportedFeatures)
        {
            if (feature.type == typeof(bool))
            {
                bool state;
                bool success = device.TryGetFeatureValue(feature.As<bool>(), out state);

                if (success)
                {
                    if (!pastControllerState.ContainsKey(feature.name))
                    {
                        continue;
                    }

                    if (!pastControllerState[feature.name])
                    {
                        if (state)
                        {
                            mapper.OnDown(feature.name, v);
                        }
                    }
                    // OnUp OR isDown
                    else if (pastControllerState[feature.name])
                    {   // OnUp
                        if (!state)
                        {
                            mapper.OnUp(feature.name, v);
                        }
                        // isDown
                        else
                        {
                            mapper.IsDown(feature.name, v);
                        }
                    }
                    pastControllerState[feature.name] = state;//Update its State
                }
            }
            else if (feature.type == typeof(Vector2)) {
                Vector2 state;
                bool success = device.TryGetFeatureValue(feature.As<Vector2>(), out state);
                if (success){
                    mapper.OnMove(feature.name,new Vector3(state.x,state.y,0),v);
                }
            }
        }

    }



    private void Update()
    {
        mapper.StartFrame();
        foreach (InputDevice device in controllers)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                CheckButtons(Modifier.LEFT, device);
            }
            else
            {
                CheckButtons(Modifier.RIGHT,device);
            }
        }
    }
}
