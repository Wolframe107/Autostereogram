using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayButton : MonoBehaviour
{
    public float delayTime = 60.0f; // Time in seconds to delay interactability
    private Button button;

    void Awake()
    {
        // Get the Button component attached to the same GameObject
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        // Start the delay coroutine when the object is enabled
        StartCoroutine(EnableButtonAfterDelay());
    }

    private IEnumerator EnableButtonAfterDelay()
    {
        // Disable the button initially
        button.interactable = false;

        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);

        // Enable the button after the delay
        button.interactable = true;
    }
}
