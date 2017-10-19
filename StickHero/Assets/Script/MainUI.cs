using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour {
    GameObject HighScore, NowScore;
    void Start () {
		
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            SceneManager.LoadScene("Game");

        }
	}
}
