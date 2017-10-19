using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGHit : MonoBehaviour {

    GameObject GroundR, GroundL;
	void Start () {
        GroundL = GameObject.Find("BGL");
        GroundR = GameObject.Find("BGR");

    }
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="HitR")
        {
            GroundR.transform.position += new Vector3(24, 0, 0);
        }
        if(other.name=="HitL")
        {
            GroundL.transform.position += new Vector3(24, 0, 0);
        }
    }
}
