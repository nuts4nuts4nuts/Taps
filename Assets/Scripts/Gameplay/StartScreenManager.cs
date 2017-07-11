using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartScreenManager : MonoBehaviour
{
    public List<Menu> menus;
    private int currentMenu;
    private float menuOffset = 19.2f;
    public ControllerDataScript controllers;

    void Start()
    {
        currentMenu = 0;
        controllers = GameObject.FindObjectOfType<ControllerDataScript>();

        foreach(Menu menu in menus)
        {
            menu.controllerData = controllers;
        }
    }

	void Update ()
    {
        menus[currentMenu].Work();

        for (int i = 0; i < controllers.controllers.Length; ++i)
        {
            ControllerDataScript.ControllerInfo controllerInfo = controllers.controllers[i];

            if(controllerInfo.device != null && controllerInfo.device.RightBumper.WasPressed)
            {
                RotateMenus(1);
            }
            else if(controllerInfo.device != null && controllerInfo.device.LeftBumper.WasPressed)
            {
                RotateMenus(-1);
            }
        }
	}

    public void RotateMenus(int forward)
    {
        int newMenu = (currentMenu + forward) % menus.Count;

        if(newMenu < 0)
        {
            newMenu = menus.Count - 1;
        }

        currentMenu = newMenu;

        //Move menus
        //Left shift
        for(int i = currentMenu; i > -1; --i)
        {
            int difference = i - currentMenu;
            Vector2 pos = menus[i].gameObject.transform.position;
            pos.x = difference * menuOffset;
            menus[i].gameObject.transform.position = pos;
        }

        //Right shift
        for(int i = currentMenu + 1; i < menus.Count; ++i)
        {
            int difference = i - currentMenu;
            Vector2 pos = menus[i].gameObject.transform.position;
            pos.x = difference * menuOffset;
            menus[i].gameObject.transform.position = pos;
        }
    }
}