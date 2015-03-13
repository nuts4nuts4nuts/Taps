using UnityEngine;
using System.Collections;
using InControl;

public class WASDControl :  UnityInputDeviceProfile
{
    public WASDControl()
    {
        Name = "WASD + Space";
        Meta = "Platformer Controls";

        SupportedPlatforms = new[]
        {
            "Windows",
            "Mac",
            "Linux"
        };

        Sensitivity = 1.0f;
        LowerDeadZone = 0.0f;
        UpperDeadZone = 1.0f;

        ButtonMappings = new[]
        {
            new InputControlMapping
            {
                Handle = "SpaceJump",
                Target = InputControlType.Action1,
                Source = KeyCodeButton(KeyCode.Space)
            },

            new InputControlMapping
            {
                Handle = "Start",
                Target = InputControlType.Menu,
                Source = KeyCodeButton(KeyCode.Z)
            },

            new InputControlMapping
            {
                Handle = "Next",
                Target = InputControlType.RightBumper,
                Source = KeyCodeButton(KeyCode.R)
            },

            new InputControlMapping
            {
                Handle = "Grab",
                Target = InputControlType.Action3,
                Source = KeyCodeButton(KeyCode.F)
            },
        };

        AnalogMappings = new[]
        {
            new InputControlMapping
            {
                Handle = "Vertical",
                Target = InputControlType.LeftStickY,
                Source = KeyCodeAxis(KeyCode.S, KeyCode.W)
            },

            new InputControlMapping
            {
                Handle = "Horizontal",
                Target = InputControlType.LeftStickX,
                Source = KeyCodeAxis(KeyCode.A, KeyCode.D)
            }

        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
