using UnityEngine;
using System.Collections;
using InControl;

public class ArrowControls : CustomInputDeviceProfile
{
    public ArrowControls()
    {
        Name = "Arrows + .";
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
                Source = KeyCodeButton(KeyCode.Period)
            },

            new InputControlMapping
            {
                Handle = "Start",
                Target = InputControlType.Menu,
                Source = KeyCodeButton(KeyCode.I)
            },

            new InputControlMapping
            {
                Handle = "Next",
                Target = InputControlType.RightBumper,
                Source = KeyCodeButton(KeyCode.O)
            },

            new InputControlMapping
            {
                Handle = "Grab",
                Target = InputControlType.Action3,
                Source = KeyCodeButton(KeyCode.Comma)
            },
        };

        AnalogMappings = new[]
        {
            new InputControlMapping
            {
                Handle = "Vertical",
                Target = InputControlType.LeftStickY,
                Source = new UnityKeyCodeAxisSource(KeyCode.DownArrow, KeyCode.UpArrow)
            },

            new InputControlMapping
            {
                Handle = "Horizontal",
                Target = InputControlType.LeftStickX,
                Source = new UnityKeyCodeAxisSource(KeyCode.LeftArrow, KeyCode.RightArrow)
            }

        };
    }
}