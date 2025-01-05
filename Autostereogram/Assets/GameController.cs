using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class GameController : MonoBehaviour
{   
    public string results = "Round, Success, CorrectObject, GuessedObject, Flat, Time\n";
    public string fileName = "results.txt";
    public GameObject roundText;
    public GameObject UIController;
    private UIController uiController;
    public GameObject gameCanvas;
    public GameObject resultCanvas;
    public GameObject resultText;
    public int round = 1;
    public float startTime;

    private bool isFlat = false;
    public GameObject[] objects_start3D;
    public GameObject[] objects_start2D;

    public GameObject[] objects;
    
    //string code = "<script>function SaveTextFile(filename, text) {const blob = new Blob([text], { type: 'text/plain' });const link = document.createElement('a');link.href = URL.createObjectURL(blob);link.download = filename;link.click();URL.revokeObjectURL(link.href);}</script>";

    void Start()
    {   
        results = "Round, Success, CorrectObject, GuessedObject, Flat, Time\n";
        uiController = UIController.GetComponent<UIController>();
    }

    public void SaveFile()
    {   
        

        #if UNITY_WEBGL && !UNITY_EDITOR
        string jsFunction = $"SaveTextFile('{fileName}', `{results}`)";
        Application.ExternalEval(jsFunction);
        #else
        Debug.Log("Saving is only supported in WebGL builds.");
        #endif
    }

    public void startGame()
    {   
        startTime = Time.time;

        if(Random.value < 0.5f){
            Debug.Log("Starting with 2D object");

            isFlat = true;

            objects = objects_start2D;
        } 
        else
        {
            Debug.Log("Starting with 3D object");

            objects = objects_start3D;
        }

        uiController.activateRound(null, objects[0]);
    }

    public void addResult(TextMeshProUGUI resultText)
    {   
        string success = resultText.text == objects[round-1].name ? "True" : "False";
        string correctObject = objects[round-1].name;
        string guessedObject = resultText.text;
        float time = Time.time - startTime;

        string result = round + ", " + success + ", "  + correctObject + ", "  +  guessedObject + ", "  + isFlat + ", "  + time.ToString(CultureInfo.InvariantCulture) + "\n";
        results += result;
    }

    public void nextRound()
    {   
        round++;
        if(round > objects.Length)
        {
            Debug.Log("Finished all rounds");
            
            gameCanvas.SetActive(false);
            resultCanvas.SetActive(true);
            // Write results to UI
            resultText.GetComponent<TextMeshProUGUI>().text = results;
        }
        
        else{
            Debug.Log("Starting round " + round);
            uiController.activateRound(objects[round - 2], objects[round - 1]);

            startTime = Time.time;
            isFlat = !isFlat;
            roundText.GetComponent<TextMeshProUGUI>().text = round + "/6";
        }
    }
}
