using UnityEngine;
using System.Collections;

public abstract class Menu : MonoBehaviour
{
    [HideInInspector]
    public ControllerDataScript controllerData;
    public abstract void Work();
}