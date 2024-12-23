using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectsController : MonoBehaviour
{   
    [Tooltip("Shader used to displace the texture")]
    public Shader textureAndDisplaceShader;

    [Tooltip("Shader used draw strip to screen")]
    public Shader drawStripShader;

    private Material displacementMaterial;
    private Material drawStripMaterial;

    [Tooltip("The base texture for the first strip.")]
    public Texture2D initialTexture;   // The base texture for the first strip

    [Tooltip("Number of vertical tiles to draw.")]
    public int numVerticalTiles = 1;   // Number of vertical tiles to draw

    [Tooltip("Total number of strips to draw.")]
    public int numStrips = 1;        // Total number of strips to draw

    [Tooltip("Controls displacement strength.")]
    public float depthFactor = 1.0f; // Controls displacement strength

    private Texture2D stripTexture;

    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;

    }

    private Texture2D CreateStripTexture(int width, int height)
    {
        // Create a new texture with the intended width and height
        Texture2D texture = new Texture2D(width, height, initialTexture.format, false);

        // Calculate the scale factors
        float widthScale = (float)width / initialTexture.width;
        float heightScale = (float)height / (initialTexture.height * numVerticalTiles);

        // Create an array to hold the colors
        Color[] colors = new Color[width * height];

        // Loop through each pixel in the new texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate the corresponding pixel in the initial texture
                int initialX = Mathf.FloorToInt(x / widthScale);
                int initialY = Mathf.FloorToInt((y % (initialTexture.height * numVerticalTiles)) / heightScale);

                // Get the color from the initial texture
                Color color = initialTexture.GetPixel(initialX, initialY);

                // Set the color in the new texture
                colors[y * width + x] = color;
            }
        }

        // Set the pixels and apply the changes
        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {   
        // Initialize the displacement material
        if (displacementMaterial == null)
        {
            displacementMaterial = new Material(textureAndDisplaceShader);
        }

        if (drawStripMaterial == null)
        {
            drawStripMaterial = new Material(drawStripShader);
        }

        int stripWidth = src.width / numStrips;

        //stripTexture = CreateStripTexture(stripWidth, src.height);
        stripTexture = initialTexture;

        RenderTexture previousTexture = RenderTexture.GetTemporary(stripWidth, src.height);
        RenderTexture tempTexture = RenderTexture.GetTemporary(stripWidth, src.height);

        // Initialize the first strip with the initial texture
        Graphics.Blit(stripTexture, previousTexture);

        //Debug.Log("Screensize is: " + src.width);
        //Debug.Log("Strip width: " + stripWidth);

        for (int i = 0; i < numStrips; i++)
        {   
            float offsetX = (float)i / numStrips;

            displacementMaterial.SetTexture("_Strip", previousTexture);
            displacementMaterial.SetTexture("_CameraView", src);
            displacementMaterial.SetFloat("_StripWidth", stripWidth);
            displacementMaterial.SetFloat("_ScreenWidth", src.width);
            displacementMaterial.SetFloat("_DepthFactor", depthFactor);
            displacementMaterial.SetFloat("_OffsetX", offsetX);

            // Apply displacement to using the previous strip
            Graphics.Blit(previousTexture, tempTexture, displacementMaterial);

            drawStripMaterial.SetTexture("_Strip", tempTexture);
            drawStripMaterial.SetFloat("_StripWidth", stripWidth);
            drawStripMaterial.SetFloat("_ScreenWidth", src.width);
            drawStripMaterial.SetFloat("_OffsetX", offsetX);

            // Draw temp to dest
            Graphics.Blit(tempTexture, dest, drawStripMaterial);

            // Set previous to temp
            Graphics.Blit(tempTexture, previousTexture);
        }

        RenderTexture.ReleaseTemporary(previousTexture);
        RenderTexture.ReleaseTemporary(tempTexture);
    }
}

