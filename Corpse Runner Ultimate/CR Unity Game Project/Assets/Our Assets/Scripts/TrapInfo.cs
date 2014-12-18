using UnityEngine;
using System.Collections;

public class TrapInfo : MonoBehaviour {

    public string difficulty;
    public int weight = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void lowerWeight()
    {
        weight -= 1;
    }
}
