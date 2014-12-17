using UnityEngine;
using System.Collections;

public class DifficultyMenuScript : MonoBehaviour {

    private GameInfo data;

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
            data.testDifficulty();
            Application.LoadLevel("level gen test Scene");
        }
        if (GUI.Button(new Rect(10, 60, 50, 50), "medium"))
        {
            data.Difficulty = "medium";
            data.testDifficulty();
            Application.LoadLevel("level gen test Scene");
        }
        if (GUI.Button(new Rect(10, 110, 50, 50), "hard"))
        {
            data.Difficulty = "hard";
            data.testDifficulty();
            Application.LoadLevel("level gen test Scene");
        }
    }
}
