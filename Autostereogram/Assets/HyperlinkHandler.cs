using TMPro;
using UnityEngine;

public class HyperlinkHandler : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private Camera mainCamera;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect mouse click
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, mainCamera);
            if (linkIndex != -1) // If a link is clicked
            {
                TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID(); // Get the link ID (e.g., URL)
                OpenLinkInNewTab(linkId);

            }
        }
    }

    //private string indexCode = "<script>function openLinkInNewTab(url) {window.open(url, '_blank');}</script>";

    public void OpenLinkInNewTab(string url)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval($"openLinkInNewTab('{url}')");
        Debug.Log("Opened link in new tab: " + url);

        #else
        Application.OpenURL(url); // Fallback for non-WebGL platforms
        Debug.Log("Opened link reguraly: " + url);

        #endif
    }
}
