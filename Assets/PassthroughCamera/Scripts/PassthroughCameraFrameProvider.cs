using UnityEngine;
using PassthroughCameraSamples;
using System;

public class PassthroughCameraFrameProvider : MonoBehaviour
{
    [SerializeField] private WebCamTextureManager m_webCamTextureManager;
    public Frame CurrentFrame { get; private set; }
    public event Action<Frame> OnNewFrame;

    private void Update()
    {
        var texture = m_webCamTextureManager.WebCamTexture;

        if (texture != null && texture.isPlaying && texture.didUpdateThisFrame)
        {
            UpdateCurrentFrame();
        }
    }

    private void UpdateCurrentFrame()
    {
        var texture = m_webCamTextureManager.WebCamTexture;

        if (texture == null)
        {
            Debug.LogError("WebCamTexture is null");
            return;
        }

        var pixels = texture.GetPixels32();

        if (pixels == null || pixels.Length == 0)
        {
            Debug.LogError("Pixel array is invalid");
            return;
        }

        var (width, height) = (texture.width, texture.height);

        if (width == 0 || height == 0)
        {
            Debug.LogError("Camera resolution is invalid");
        }

        CurrentFrame = new Frame
        {
            Pixels = pixels,
            Width = width,
            Height = height
        };

        OnNewFrame?.Invoke(CurrentFrame);
    }
}