using UnityEngine;
using System.Collections;


[RequireComponent(typeof (MeshFilter))]
public class Middle : MonoBehaviour {

    //[HideInInspector] 
    public PointListSource Src;

    void OnDrawGizmos() {
        Src.draw();
    }
}
