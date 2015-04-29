using UnityEngine;
using System.Collections;

public class CommonBehaviour : MonoBehaviour {

    [HideInInspector]
    public Transform Trnsfrm { get; private set; }

    protected void Awake() {
        Trnsfrm = transform;
    }
}
