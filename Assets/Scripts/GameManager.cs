using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Players
{
    Invalid = -1,
    Player1,
    Player2,
    Player3,
    Player4
}

public class GameManager : MonoBehaviour
{
    //Singleton shit cred http://unitypatterns.com/singletons/
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    private CharacterControllerScript[] chars;
    private Text[] charText;
    public List<Image> charPanels;
    public Text ballText;

    private ControllerDataScript controllerData;

    void Awake()
    {
        controllerData = (ControllerDataScript)FindObjectOfType(typeof(ControllerDataScript));
        chars = new CharacterControllerScript[ControllerDataScript.MAX_CONTROLLERS];
        charText = new Text[ControllerDataScript.MAX_CONTROLLERS];
        CreatePlayers();
    }

    private void CreatePlayers()
    {
        int movementIter = 0;
        for(int i = 0; i < ControllerDataScript.MAX_CONTROLLERS; ++i)
        {
            if(controllerData.controllers[i].device != null)
            { 
                GameObject character = (GameObject)Instantiate(Resources.Load("Prefabs/Character"));
                CharacterControllerScript cs = character.GetComponent<CharacterControllerScript>();
                cs.controller = controllerData.controllers[i].device;
                cs.me = (Players)i;
                cs.transform.position = new Vector3(1 + movementIter * 2, 1.75f, 0);
                chars[i] = cs;
                movementIter++;

                Text text = charPanels[i].GetComponentInChildren<Text>();
                charText[i] = text;
                charText[i].text = "100";
            }
            else
            {
                Color color = charPanels[i].color;
                color.a = 0.0f;
                charPanels[i].color = color;
            }
        }
    }

    public void UpdateCharText(CharacterControllerScript cs, string newText)
    {
        int index = -1;
        for(int i = 0; i < chars.Length && index == -1; ++i)
        {
            if(chars[i] == cs)
            {
                index = i;
            }
        }

        if(charText[index] != null)
        {
            charText[index].text = newText;
        }
    }

    public void UpdateBallText(string newText)
    {
        ballText.text = newText;
    }
}
