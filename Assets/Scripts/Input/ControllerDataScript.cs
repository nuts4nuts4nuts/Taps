using UnityEngine;
using System.Collections;
using InControl;

public class ControllerDataScript : MonoBehaviour
{
    private Color[] colors;

    public struct ControllerInfo
    {
        public InputDevice device;
        public bool ready;
        public Color color;
        public bool justMovedLeftStick;
    }

    [HideInInspector]
    public ControllerInfo[] controllers;
    [HideInInspector]
    public const int MAX_CONTROLLERS = 4;
    [HideInInspector]
    public bool teams = false;

    void Awake()
    {
        controllers = new ControllerInfo[MAX_CONTROLLERS];
        colors = new Color[4];
        colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow };
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
        newInfo.color = GetNextValidColor(Color.yellow);
        newInfo.justMovedLeftStick = false;

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

    public Color GetNextValidColor(Color color)
    {
        Color newColor = color;
        int colorIndex = 0; 
        bool colorFound = false;

        //Get index of color
        for(; !colorFound; ++colorIndex)
        {
            if(colors[colorIndex] == color)
            {
                colorFound = true;
            }
        }
        //colorIndex gets iterated on the way out
        colorIndex--;

        //Assign new color
        bool colorIsUsed = true;
        int i = (colorIndex + 1) % colors.Length;

        while(i != colorIndex && colorIsUsed)
        {
            //Check is color is used
            colorIsUsed = false;
            for(int j = 0; j < controllers.Length; ++j)
            {
                if(controllers[j].color == colors[i])
                {
                    colorIsUsed = true;
                }

                if(teams)
                {
                    colorIsUsed = false;
                }
            }

            if(!colorIsUsed)
            {
                newColor = colors[i];
            }

            i = (i + 1) % colors.Length;
        }

        return newColor;
    }

    public Color GetPrevValidColor(Color color)
    {
        Color newColor = color;
        int colorIndex = 0; 
        bool colorFound = false;

        //Get index of color
        for(; !colorFound; ++colorIndex)
        {
            if(colors[colorIndex] == color)
            {
                colorFound = true;
            }
        }
        //colorIndex gets iterated on the way out
        colorIndex--;

        //Assign new color
        bool colorIsUsed = true;
        int i = colorIndex - 1;
        //Wrap it around
        if(i < 0)
        {
            i = colors.Length - 1;
        }

        while(i != colorIndex && colorIsUsed)
        {
            //Check is color is used
            colorIsUsed = false;
            for(int j = 0; j < controllers.Length; ++j)
            {
                if(controllers[j].color == colors[i])
                {
                    colorIsUsed = true;
                }

                if(teams)
                {
                    colorIsUsed = false;
                }
            }

            if(!colorIsUsed)
            {
                newColor = colors[i];
            }

            i = i - 1;
            //Wrap it around
            if(i < 0)
            {
                i = colors.Length - 1;
            }
        }

        return newColor;
    }
}
