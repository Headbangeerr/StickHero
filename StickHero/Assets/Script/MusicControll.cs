using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControll : MonoBehaviour {
    public AudioClip AddScore,StickHitPlatform,PlayerFall,Prefect,StickFail;
    AudioSource AudioManager;
	void Start () {
        AudioManager = gameObject.GetComponent<AudioSource>();
	}
    public void PlayAddScore()
    {
        AudioManager.clip = AddScore;
        AudioManager.volume = 0.2f;
        AudioManager.Play();
    }
    public void PlayStickHitPlatform()
    {
        AudioManager.clip = StickHitPlatform;
        AudioManager.volume = 1;
        AudioManager.Play();
    }
    public void PlayPlayerFall()
    {
        AudioManager.clip = PlayerFall;
        AudioManager.volume = 1;
        AudioManager.Play();
    }
    public void PlayPrefect()
    {
        AudioManager.clip = Prefect;
        AudioManager.volume = 1;
        AudioManager.Play();
    }
    public void PlayStickFail()
    {
        AudioManager.clip = StickFail;
        AudioManager.volume = 1;
        AudioManager.Play();
    }
}
