using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class StartScreenManager : MonoBehaviour
{
    public List<GameObject> startSpheres;
    public ControllerDataScript controllerData;

	void Update ()
    {
        int numDevices = InputManager.Devices.Count;

        for(int i = 0; i < numDevices; ++i)
        {
            InputDevice device = InputManager.Devices[i];

            if(device.Action1.WasPressed)
            {
                int index = controllerData.Contains(device);
                if(index != -1)
                {
                    controllerData.controllers[index].ready = !controllerData.controllers[index].ready;
                }
                else
                {
                    controllerData.Add(device);
                }
            }

            if(device.Action2.WasPressed)
            {
                controllerData.Remove(device);
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
                startSpheres[i].renderer.material.color = Color.green;

                if(controllerData.controllers[i].ready)
                {
                    startSpheres[i].renderer.material.color = Color.yellow;
                }
            }
        }

        if(controllerData.ReadyCount() == controllerData.Count() && controllerData.ReadyCount() > 0)
        {
            Application.LoadLevel("Game");
        }
	}
}