using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet
{
    public PlayerAction Jump;
    public PlayerAction Grab;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerOneAxisAction Move;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerOneAxisAction AimVertical;

    public PlayerActions()
    {
        Jump  = CreatePlayerAction( "Jump" );
        Grab  = CreatePlayerAction( "Grab" );
        Left  = CreatePlayerAction( "Left" );
        Right = CreatePlayerAction( "Right" );
        Move  = CreateOneAxisPlayerAction( Left, Right );
        Up    = CreatePlayerAction( "Up" );
        Down  = CreatePlayerAction( "Down" );
        AimVertical = CreateOneAxisPlayerAction( Up, Down );
    }

    public static PlayerActions CreateWithDefaultBindings()
    {
        PlayerActions playerActions = new PlayerActions();

        playerActions.Jump.AddDefaultBinding( Key.Period );
        playerActions.Jump.AddDefaultBinding( InputControlType.Action1 );

        playerActions.Grab.AddDefaultBinding( Key.Period );
        playerActions.Grab.AddDefaultBinding( InputControlType.Action1 );

        playerActions.Left.AddDefaultBinding( Key.LeftArrow );
        playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );

        playerActions.Right.AddDefaultBinding( Key.RightArrow );
        playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );

        playerActions.Up.AddDefaultBinding( Key.UpArrow );
        playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );

        playerActions.Down.AddDefaultBinding( Key.DownArrow );
        playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );

        return playerActions;
    }
}
