﻿using UnityEngine;
using System.Collections;

public class LevelGeneration : MonoBehaviour {

    public string difficulty;
    private int tempDiff;

    public GameObject button;
    public GameObject[] traps;

    // the table to show the raio of traps depending on the difficulty
    //[difficulty, trap difficulty]
    public int[,] ratio = new int[3,3];

    private int easyTraps;
    private int medTraps;
    private int hardTraps;
    private int totalTraps;

    public int seed;
    public bool useSeed;

    void Awake()
    {
        GameInfo data = GameObject.Find("Persis Data").GetComponent<GameInfo>();

        difficulty = data.Difficulty;
        if (data.isSeed == true)
        {
            seed = data.Seed;
            useSeed = true;
        }
    }

	// Use this for initialization
	void Start () {
        //easy - easy: 7, med: 2, hard: 1
        ratio[0, 0] = 7;
        ratio[0, 1] = 2;
        ratio[0, 2] = 1;

        //med - easy: 3, med: 5, hard: 2
        ratio[1, 0] = 3;
        ratio[1, 1] = 5;
        ratio[1, 2] = 2;

        //easy - easy: 0, med: 7, hard: 3
        ratio[2, 0] = 0;
        ratio[2, 1] = 7;
        ratio[2, 2] = 3;

        if (difficulty == "easy")
        {
            tempDiff = 0;
        }
        else if (difficulty == "medium")
        {
            tempDiff = 1;
        }
        else
        {
            tempDiff = 2;
        }

        easyTraps = ratio[tempDiff, 0];
        medTraps = ratio[tempDiff, 1];
        hardTraps = ratio[tempDiff, 2];
        totalTraps = easyTraps + medTraps + hardTraps;

        Debug.Log(difficulty + " ratio: " + easyTraps + " " + medTraps + " " + hardTraps);

        spawnTraps();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void spawnTraps()
    {
        if (useSeed == true)
        {
            Random.seed = seed;
            Debug.Log(Random.value + " " + Random.value + " " + Random.value + " " + Random.value + " " + Random.value + " ");
        }
        else
        {
            seed = Random.seed;
            Debug.Log(Random.value + " " + Random.value + " " + Random.value + " " + Random.value + " " + Random.value + " ");
        }
    }
}
