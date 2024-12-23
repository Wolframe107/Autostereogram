using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectsController : MonoBehaviour
{
    public Shader textureAndDisplaceShader;
    public Shader drawStripShader;
    private Material displacementMaterial;
    private Material drawStripMaterial;

    public Texture2D initialTexture;   // The base texture for the first strip
    public float depthFactor = 1.0f; // Controls displacement strength
    public int numStrips = 1;        // Total number of strips to draw

    //private RenderTexture tempTexture;
    private RenderTexture[] stripTextures;

    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;

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

        Debug.Log("Strip width: " + stripWidth);

        RenderTexture previousTexture = RenderTexture.GetTemporary(stripWidth, src.height);

        // Initialize the first strip with the initial texture
        /*
        Graphics.Blit(initialTexture, previousTexture);

        drawStripMaterial.SetTexture("_Strip", previousTexture);
        drawStripMaterial.SetFloat("_StripWidth", stripWidth);
        drawStripMaterial.SetFloat("_ScreenWidth", src.width);
        drawStripMaterial.SetFloat("_OffsetX", 0.0f);

        Graphics.Blit(previousTexture, dest, drawStripMaterial);

        drawStripMaterial.SetFloat("_OffsetX", 0.666f);
        Graphics.Blit(previousTexture, dest, drawStripMaterial);
        */

        // Initialize the first strip with the initial texture
        Graphics.Blit(initialTexture, previousTexture);

        Debug.Log("Screensize is: " + src.width);
        
        for (int i = 0; i < numStrips; i++)
        {   
            RenderTexture tempTexture = RenderTexture.GetTemporary(stripWidth * (i + 1), src.height);

            float offsetX = (float)i / numStrips;

            displacementMaterial.SetTexture("_Strip", previousTexture);
            displacementMaterial.SetTexture("_CameraView", src);
            displacementMaterial.SetFloat("_StripWidth", stripWidth);
            displacementMaterial.SetFloat("_ScreenWidth", src.width);
            displacementMaterial.SetFloat("_ScreenHeight", src.height);
            displacementMaterial.SetFloat("_DepthFactor", depthFactor);
            displacementMaterial.SetFloat("_OffsetX", offsetX);

            Graphics.Blit(previousTexture, tempTexture, displacementMaterial);

            drawStripMaterial.SetTexture("_Strip", tempTexture);
            drawStripMaterial.SetFloat("_StripWidth", stripWidth);
            drawStripMaterial.SetFloat("_ScreenWidth", src.width);
            drawStripMaterial.SetFloat("_OffsetX", offsetX);

            Graphics.Blit(tempTexture, dest, drawStripMaterial);

            Graphics.Blit(tempTexture, previousTexture);

            if (i == numStrips - 1)
            {
                Graphics.Blit(tempTexture, previousTexture);
            }



        }
        /*
        displacementMaterial.SetTexture("_Strip", previousTexture);
        displacementMaterial.SetTexture("_CameraView", src);
        displacementMaterial.SetFloat("_StripWidth", stripWidth);
        displacementMaterial.SetFloat("_ScreenWidth", src.width);
        displacementMaterial.SetFloat("_ScreenHeight", src.height);
        displacementMaterial.SetFloat("_DepthFactor", depthFactor);
        displacementMaterial.SetFloat("_OffsetX", 0.0f);

        Debug.Log("prevTex width: " + previousTexture.width);
        Debug.Log("tempTexture width: " + tempTexture.width);

        Graphics.Blit(previousTexture, tempTexture, displacementMaterial);

        //Graphics.Blit(tempTexture, dest, displacementMaterial);

        drawStripMaterial.SetTexture("_Strip", tempTexture);
        drawStripMaterial.SetFloat("_StripWidth", stripWidth);
        drawStripMaterial.SetFloat("_ScreenWidth", src.width);
        drawStripMaterial.SetFloat("_OffsetX", 0.0f);
        // First strip drawing
        Graphics.Blit(tempTexture, dest, drawStripMaterial);

        Graphics.Blit(tempTexture, previousTexture);

        // Do it again but with the previous slice
        displacementMaterial.SetTexture("_Strip", previousTexture);
        displacementMaterial.SetTexture("_CameraView", src);
        displacementMaterial.SetFloat("_StripWidth", stripWidth);
        displacementMaterial.SetFloat("_ScreenWidth", src.width);
        displacementMaterial.SetFloat("_ScreenHeight", src.height);
        displacementMaterial.SetFloat("_DepthFactor", depthFactor);
        displacementMaterial.SetFloat("_OffsetX", 0.5f);

        Graphics.Blit(previousTexture, tempTexture, displacementMaterial);

        drawStripMaterial.SetTexture("_Strip", tempTexture);
        drawStripMaterial.SetFloat("_StripWidth", stripWidth);
        drawStripMaterial.SetFloat("_ScreenWidth", src.width);
        drawStripMaterial.SetFloat("_OffsetX", 0.5f);

        Graphics.Blit(tempTexture, dest, drawStripMaterial);
        */
        RenderTexture.ReleaseTemporary(previousTexture);
        RenderTexture.ReleaseTemporary(tempTexture);
    }
}

