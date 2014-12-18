using UnityEngine;
using System.Collections;

public class DifficultyMenuScript : MonoBehaviour {

    private GameInfo data;
    private string seedText = "";

	// Use this for initialization
	void Start () {
        data = GameObject.Find("Persis Data").GetComponent<GameInfo>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (GUI.Button(new Rect(10,10,50,50),"easy"))
        {
            data.Difficulty = "easy";
            loadLevel();
        }
        if (GUI.Button(new Rect(10, 60, 50, 50), "medium"))
        {
            data.Difficulty = "medium";
            loadLevel();
        }
        if (GUI.Button(new Rect(10, 110, 50, 50), "hard"))
        {
            data.Difficulty = "hard";
            loadLevel();
        }
        
        seedText = GUI.TextField(new Rect(10, 160, 200, 25), seedText);
        GUI.Label(new Rect(10,185, 200, 25), "enter number to generate seed");
    }

    void loadLevel()
    {
        int tempSeed;
        data.isSeed = false;
        if (seedText != "")
        {
            tempSeed = int.Parse(seedText);
            data.Seed = tempSeed;
            data.isSeed = true;
        }
        data.testDifficulty();
        Application.LoadLevel("level gen test Scene");
        
    }
}
