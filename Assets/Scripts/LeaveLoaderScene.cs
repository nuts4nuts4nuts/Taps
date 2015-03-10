using UnityEngine;
using System.Collections;

public class LeaveLoaderScene : MonoBehaviour
{
    public string firstScene;

	void Start ()
    {
        Application.LoadLevel(firstScene);
	}
}
