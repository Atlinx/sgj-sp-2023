using Game;
using Godot;
using System;

public partial class WhiskPlayerConnector : Node
{
    [ExportCategory("Dependencies")]
    [Export]
    private Grabbable grabbable;
    [Export]
    private Whisk whisk;

    public override void _Ready()
    {
        grabbable.GrabStarted += OnGrabStarted;
        grabbable.GrabEnded += OnGrabEnded;
    }

    private void OnGrabStarted()
    {
        if (grabbable.Grabber is Hand hand)
        {
            if (hand.GetParent() is Player player)
            {
                player.SetWhisk(whisk);
            }
        }
    }

    private void OnGrabEnded()
    {
        if (grabbable.Grabber is Hand hand)
        {
            if (hand.GetParent() is Player player)
            {
                player.RemoveWhisk();
            }
        }
    }
}
