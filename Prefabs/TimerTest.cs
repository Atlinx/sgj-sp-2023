using Godot;
using System;

public partial class TimerTest : Timer
{
    public override void _Ready()
    {
        Timeout += () => { GD.Print("Ding ding ding!"); };
    }
}
