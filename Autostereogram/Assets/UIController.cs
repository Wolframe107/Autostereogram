using UnityEngine;

public class UIController : MonoBehaviour
{
    private GameObject currentCanvas;

    // Set the current canvas (optional initialization)
    public void SetCurrentCanvas(GameObject canvas)
    {
        currentCanvas = canvas;
    }

    // Switch to a new canvas
    public void SwitchCanvas(GameObject newCanvas)
    {
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false); // Disable the current canvas
        }

        newCanvas.SetActive(true); // Enable the new canvas
        currentCanvas = newCanvas; // Update the reference
    }

    public void setText(GameObject text, string value)
    {
        text.GetComponent<UnityEngine.UI.Text>().text = value;
    }

    public void activateRound(GameObject oldObject, GameObject newObject)
    {   
        // Deactivate old object
        if(oldObject != null)
        {
            oldObject.SetActive(false);
        }

        newObject.SetActive(true);
    }
}
