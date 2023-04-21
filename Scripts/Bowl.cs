using Game;
using Godot;
using System;
using System.Collections.Generic;

public partial class Bowl : Node2D
{
    [ExportCategory("Settings")]
    [Export]
    private float resistance;
    [Export]
    private int angularVelocitySamples = 30;

    [ExportCategory("Dependencies")]
    [Export]
    public Node2D CenterPoint { get; private set; }

    [Export]
    public Area2D Area { get; private set; }

    [Export]
    private AnimatedSprite2D Fill;
    [Export]
    private AnimatedSprite2D X;

    private float[] angularVelocityDeltaBuffer;
    private int angularVelocityCurrIndex = 0;
    private List<Whisk> whisks = new List<Whisk>();

    public float TotalAverageAngularVelocity { get; private set; }

    public override void _Ready()
    {
        Fill.Play();
        X.Play();
        angularVelocityDeltaBuffer = new float[angularVelocitySamples];
    }

    public void RegisterWhisk(Whisk whisk)
    {
        whisks.Add(whisk);
    }

    public void LogAvgAngularVelocity()
    {
        float sum = 0;
        foreach (Whisk whisk in whisks)
        {
            sum += whisk.AverageAngularVelocity;
        }

        GD.Print(sum);

        angularVelocityDeltaBuffer[angularVelocityCurrIndex++] = sum /= whisks.Count;
        if (angularVelocityCurrIndex >= angularVelocityDeltaBuffer.Length)
            angularVelocityCurrIndex = 0;

        float grandSum = 0;
        foreach (float i in angularVelocityDeltaBuffer)
            grandSum += i;

        TotalAverageAngularVelocity = grandSum / angularVelocityDeltaBuffer.Length;
    }

    public override void _Process(double delta)
    {
        LogAvgAngularVelocity();
        Fill.SpeedScale = TotalAverageAngularVelocity / resistance;
    }
}
