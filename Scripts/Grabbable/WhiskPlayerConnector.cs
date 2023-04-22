using Game;
using Godot;

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
        if (grabbable.Grabber is Player player)
        {
            player.SetWhisk(whisk);
            whisk.currentPlayer = player;
        }
    }

    private void OnGrabEnded()
    {
        if (grabbable.Grabber is Player player)
        {
            player.RemoveWhisk();
            whisk.currentPlayer = null;
        }
    }
}
