using UnityEngine;
using System.Collections;
using System.Reflection;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
