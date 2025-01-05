using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    // Folder to save the screenshot
    public string screenshotFolder = "Screenshots";
    public string screenshotFileName = "screenshot";
    public int screenshotWidth = 3840; // 4K width
    public int screenshotHeight = 2160; // 4K height

    void Start()
    {
        // Ensure the screenshot folder exists
        if (!System.IO.Directory.Exists(screenshotFolder))
        {
            System.IO.Directory.CreateDirectory(screenshotFolder);
        }
    }

    void Update()
    {
        // Check for "G" key press
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        // Generate a unique file name with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = System.IO.Path.Combine(screenshotFolder, $"{screenshotFileName}_{timestamp}.png");

        // Take the screenshot
        ScreenCapture.CaptureScreenshot(filePath, 1); // "1" means original size, for 4K ensure width and height are set
        Debug.Log($"Screenshot saved to: {filePath}");
    }
}
