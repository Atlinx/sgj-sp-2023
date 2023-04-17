using Godot;
using System;

public partial class Speedometer : ColorRect
{
    [Export]
    private ColorRect _fill;

    public void SetFill(float ratio, Color color)
    {
        GD.Print(ratio);
        _fill.Size = new Vector2(ratio * Size.X, Size.Y);
        _fill.Color = color;
    }

    public void SetFill(float ratio)
    {
        SetFill(ratio, _fill.Color);
    }
}

