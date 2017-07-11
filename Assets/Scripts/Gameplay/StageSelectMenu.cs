using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class StageSelectMenu : Menu
{
    public List<string> levels;
    public List<GameObject> levelSprites;
    private int currentPair = 0;

    public override void Work()
    {
        string currentLevel = levels[currentPair];
        GameObject currentSprite = levelSprites[currentPair];
        currentSprite.GetComponent<Renderer>().material.color = Color.green;

        for(int i = 0; i < controllerData.controllers.Length; ++i)
        {
            ControllerDataScript.ControllerInfo controllerInfo = controllerData.controllers[i];

            if(controllerInfo.device != null)
            {
                if (controllerInfo.device.MenuWasPressed)
                {
                    Application.LoadLevel(currentLevel);
                }

                float x = controllerInfo.device.LeftStickX;

                bool movedStick = controllerInfo.justMovedLeftStick;
                if (x >= 0.5f && !movedStick)
                {
                    currentSprite.GetComponent<Renderer>().material.color = Color.gray;
                    RotateLevels(1);
                    controllerData.controllers[i].justMovedLeftStick = true;
                }
                else if (x <= -0.5f && !movedStick)
                {
                    currentSprite.GetComponent<Renderer>().material.color = Color.gray;
                    RotateLevels(-1);
                    controllerData.controllers[i].justMovedLeftStick = true;
                }
                else if (x < 0.5f && x > -0.5f)
                {
                    controllerData.controllers[i].justMovedLeftStick = false;
                }
            }
        }
    }

    private void RotateLevels(int forward)
    {
        int newPair = (currentPair + forward) % levels.Count;

        if(newPair < 0)
        {
            newPair = levels.Count - 1;
        }

        currentPair = newPair;
    }
}