using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {

    private string difficulty;
    private int seed;
    public bool isSeed;

    public string Difficulty
    {
        get { return difficulty; }
        set { difficulty = value; }
    }

    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void testDifficulty()
    {
        Debug.Log(difficulty);
        Debug.Log(seed);
    }
}
