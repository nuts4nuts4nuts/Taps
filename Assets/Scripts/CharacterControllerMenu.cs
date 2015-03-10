using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class CharacterControllerMenu : Menu
{
    public List<GameObject> startSpheres;
    public StartScreenManager manager;

    public override void Work()
    {
        int numDevices = InputManager.Devices.Count;

        for(int i = 0; i < numDevices; ++i)
        {
            InputDevice device = InputManager.Devices[i];

            int index = controllerData.Contains(device);
            if(device.Action1.WasPressed && index == -1)
            {
                controllerData.Add(device);
                index = controllerData.Contains(device);
                controllerData.controllers[index].ready = true;
            }

            if(device.Action2.WasPressed)
            {
                controllerData.Remove(device);
            }

            if(device.Action3.WasPressed)
            {
                controllerData.teams = !controllerData.teams;
            }

            float x = device.LeftStickX;

            if(index != -1)
            {
                bool movedStick = controllerData.controllers[index].justMovedLeftStick;
                if(x >= 0.5f && !movedStick)
                {
                    Color currentColor = controllerData.controllers[index].color;
                    controllerData.controllers[index].color = controllerData.GetNextValidColor(currentColor);
                    controllerData.controllers[index].justMovedLeftStick = true;
                }
                else if(x <= -0.5f && !movedStick)
                {
                    Color currentColor = controllerData.controllers[index].color;
                    controllerData.controllers[index].color = controllerData.GetPrevValidColor(currentColor);
                    controllerData.controllers[index].justMovedLeftStick = true;
                }
                else if(x < 0.5f && x > -0.5f)
                {
                    controllerData.controllers[index].justMovedLeftStick = false;
                }
            }
        }

        for(int i = 0; i < ControllerDataScript.MAX_CONTROLLERS; ++i)
        {
            if(i < numDevices)
            {
                startSpheres[i].renderer.material.color = Color.gray;
            }
            else
            {
                startSpheres[i].renderer.material.color = Color.black;
            }

            if (controllerData.controllers[i].device != null)
            {
                startSpheres[i].renderer.material.color = controllerData.controllers[i].color;
            }
        }
    }
}
