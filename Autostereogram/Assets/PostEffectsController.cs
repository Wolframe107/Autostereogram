using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectsController : MonoBehaviour
{
    public Shader textureAndDisplaceShader;
    private Material displacementMaterial;

    public Texture initialTexture;   // The base texture for the first strip
    public float depthFactor = 1.0f; // Controls displacement strength
    public int numStrips = 1;        // Total number of strips to draw
    public int currentWidth = 0;


    private RenderTexture tempTexture;
    private RenderTexture[] stripTextures;

    void Start()
    {
        // Enable the camera depth texture
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }



    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {   
        // Initialize the displacement material
        if (displacementMaterial == null)
        {
            displacementMaterial = new Material(textureAndDisplaceShader);
        }

        Graphics.Blit(Texture2D.blackTexture, dest);

        int stripWidth = src.width / numStrips;

        displacementMaterial.SetTexture("_Strip", initialTexture);
        displacementMaterial.SetTexture("_CameraView", src);
        displacementMaterial.SetFloat("_StripWidth", stripWidth);
        displacementMaterial.SetFloat("_ScreenWidth", src.width);
        displacementMaterial.SetFloat("_ScreenHeight", src.height);
        displacementMaterial.SetFloat("_DepthFactor", depthFactor);
        displacementMaterial.SetFloat("_OffsetX", 0.0f);
        displacementMaterial.SetFloat("_CurrentStrip", 0.0f);
        
        currentWidth = stripWidth;
        // skriva om för att använda rendertexture istället
        RenderTexture tempTexture = RenderTexture.GetTemporary(currentWidth, dest.height);
        Graphics.Blit(tempTexture, dest, displacementMaterial);


        //Graphics.Blit(src, dest, displacementMaterial);

        /*
        for (int i = 1; i < numStrips; i++)
        {
            displacementMaterial.SetFloat("_OffsetX", (float)i/numStrips);
            displacementMaterial.SetFloat("_CurrentStrip", (float)i);
            Graphics.Blit(src, dest, displacementMaterial);
        }
        */

        RenderTexture.ReleaseTemporary(tempTexture);
        //stripTextures = new RenderTexture[numStrips];

        // Set up the final result texture for display
        //resultTexture = new RenderTexture(Screen.width, Screen.height, 0);

        // Step 1: Render the first strip using the initial texture and depth map
        
    }
}

