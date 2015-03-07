using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartScreenManager : MonoBehaviour
{
    public List<Menu> menus;
    private int currentMenu;

    void Start()
    {
        currentMenu = 0;
    }

	void Update ()
    {
        menus[currentMenu].Work();
	}
}