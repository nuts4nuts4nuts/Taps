using UnityEngine;
using System.Collections;
using InControl;

public class ControllerDataScript : MonoBehaviour
{
    public struct ControllerInfo
    {
        public InputDevice device;
        public bool ready;
    }

    [HideInInspector]
    public ControllerInfo[] controllers;
    [HideInInspector]
    public const int MAX_CONTROLLERS = 4;

    void Awake()
    {
        DontDestroyOnLoad(this);

        controllers = new ControllerInfo[MAX_CONTROLLERS];
    }

    public int Contains(InputDevice device)
    {
        for(int i = 0; i < MAX_CONTROLLERS; ++i)
        {
            if(controllers[i].device == device)
            {
                return i;
            }
        }

        return -1;
    }

    public void Add(InputDevice device)
    {
        ControllerInfo newInfo = new ControllerInfo();
        newInfo.device = device;
        newInfo.ready = false;

        int index = FirstOpenIndex();
        if(index != -1)
        {
            controllers[index] = newInfo;
        }
    }

    public void Remove(InputDevice device)
    {
        for(int i = 0; i < MAX_CONTROLLERS; ++i)
        {
            if(controllers[i].device == device)
            {
                controllers[i] = new ControllerInfo();
            }
        }
    }

    public int Count()
    {
        int counter = 0;
        for(int i = 0; i < MAX_CONTROLLERS; ++i)
        {
            if(controllers[i].device != null)
            {
                counter++;
            }
        }

        return counter;
    }

    public int ReadyCount()
    {
        int counter = 0;
        for(int i = 0; i < MAX_CONTROLLERS; ++i)
        {
            if(controllers[i].ready)
            {
                counter++;
            }
        }

        return counter;
    }

    public int FirstOpenIndex()
    {
        for(int i = 0; i < MAX_CONTROLLERS; ++i)
        {
            if(controllers[i].device == null)
            {
                return i;
            }
        }

        return -1;
    }
}
