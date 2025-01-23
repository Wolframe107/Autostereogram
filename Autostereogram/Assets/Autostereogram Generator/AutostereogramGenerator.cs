using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class AutostereogramGenerator : MonoBehaviour
{
    private Material autoStereogramMaterial;
    private Material drawStripMaterial;

    [Tooltip("The initial texture for the first strip.")]
    public Texture2D initialTexture;

    [Tooltip("Enable Autostereogram")]
    public bool enableEffect = true;

    [Tooltip("Number of vertical tiles to draw.")]
    [Range(1, 30)] // You can remove the ranges if you want to allow any value.
    public int numVerticalTiles = 4;

    [Tooltip("Total number of strips to draw.")]
    [Range(8, 24)] 
    public int numStrips = 10;

    [Tooltip("Controls displacement strength.")]
    [Range(0.0f, 1.0f)]
    public float depthFactor = 0.1f;

    [Tooltip("Invert Depth Map for interesting effects.")]
    public bool invertDepthMap = true;

    [Tooltip("Instead of rendering the object with depth, render it flat.")]
    public bool flatMode = false;

    [Tooltip("Shows the underlying depth map.")]
    public bool depthMode = false;

    private Texture2D stripTexture;

    void Start()
    {
        InitializeMaterials();
        SetCameraDepthMode();
    }

    private void InitializeMaterials()
    {
        Shader autoStereogramShader = Shader.Find("Custom/AutostereogramShader");
        Shader drawStripShader = Shader.Find("Custom/DrawStripShader");

        if (autoStereogramShader != null && autoStereogramMaterial == null)
        {
            autoStereogramMaterial = new Material(autoStereogramShader);
        }

        if (drawStripShader != null && drawStripMaterial == null)
        {
            drawStripMaterial = new Material(drawStripShader);
        }
    }

    private void SetCameraDepthMode()
    {
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.depthTextureMode = DepthTextureMode.Depth;
        }
    }

    private Texture2D CreateStripTexture(int width, int height)
    {
        int adjustedWidth = Mathf.CeilToInt(width / 4f) * 4;
        int adjustedHeight = Mathf.CeilToInt(height / 4f) * 4;

        Texture2D texture = new Texture2D(adjustedWidth, adjustedHeight, TextureFormat.RGBA32, false);
        Color[] colors = new Color[adjustedWidth * adjustedHeight];

        float widthScale = (float)adjustedWidth / initialTexture.width;
        float heightScale = (float)adjustedHeight / (initialTexture.height * numVerticalTiles);

        for (int y = 0; y < adjustedHeight; y++)
        {
            for (int x = 0; x < adjustedWidth; x++)
            {
                int initialX = Mathf.FloorToInt(x / widthScale);
                int initialY = Mathf.FloorToInt((y % (initialTexture.height * numVerticalTiles)) / heightScale);
                colors[y * adjustedWidth + x] = initialTexture.GetPixel(initialX, initialY);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!enableEffect)
        {
            Graphics.Blit(src, dest);
            return;
        }

        int stripWidth = numStrips > 0 ? src.width / numStrips : 0;
        stripTexture = CreateStripTexture(stripWidth, src.height);

        ConfigureMaterialProperties(src, stripWidth);

        if (depthMode)
        {
            Graphics.Blit(src, dest, autoStereogramMaterial);
            return;
        }

        RenderTexture previousTexture = RenderTexture.GetTemporary(stripWidth, src.height);
        RenderTexture tempTexture = RenderTexture.GetTemporary(stripWidth, src.height);

        Graphics.Blit(stripTexture, previousTexture);

        for (int i = 0; i < numStrips; i++)
        {
            float offsetX = (float)i / numStrips;
            autoStereogramMaterial.SetTexture("_Strip", previousTexture);
            autoStereogramMaterial.SetFloat("_OffsetX", offsetX);

            Graphics.Blit(previousTexture, tempTexture, autoStereogramMaterial);

            drawStripMaterial.SetTexture("_Strip", tempTexture);
            drawStripMaterial.SetFloat("_OffsetX", offsetX);
            Graphics.Blit(tempTexture, dest, drawStripMaterial);

            Graphics.Blit(tempTexture, previousTexture);
        }

        RenderTexture.ReleaseTemporary(previousTexture);
        RenderTexture.ReleaseTemporary(tempTexture);
    }

    private void ConfigureMaterialProperties(RenderTexture src, int stripWidth)
    {
        autoStereogramMaterial.SetTexture("_CameraView", src);
        autoStereogramMaterial.SetFloat("_StripWidth", stripWidth);
        autoStereogramMaterial.SetFloat("_ScreenWidth", src.width);
        autoStereogramMaterial.SetFloat("_DepthFactor", flatMode ? Mathf.Lerp(0.0f, 0.1f, depthFactor) : depthFactor);
        autoStereogramMaterial.SetFloat("_InvertDepth", invertDepthMap ? -1.0f : 1.0f);
        autoStereogramMaterial.SetFloat("_DepthMode", depthMode ? 1.0f : -1.0f);
        autoStereogramMaterial.SetFloat("_FlatMode", flatMode ? 1.0f : -1.0f);

        drawStripMaterial.SetFloat("_StripWidth", stripWidth);
        drawStripMaterial.SetFloat("_ScreenWidth", src.width);
    }
}
