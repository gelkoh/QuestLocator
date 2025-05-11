using UnityEngine;

public struct Frame
{
    public Color32[] Pixels { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Frame(Color32[] pixels, int width, int height)
    {
        Pixels = pixels;
        Width = width;
        Height = height;
    }
}