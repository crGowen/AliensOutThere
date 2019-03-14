using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperNovaFlare
{
    public float timeLeft;
    public GameObject audio;
    public GameObject flash;

    public SuperNovaFlare(GameObject inputFlash, GameObject inputAudio, float audioDelay)
    {
        timeLeft = 1.0f;
        flash = inputFlash;
        audio = inputAudio;
        audio.GetComponent<AudioSource>().PlayDelayed(audioDelay);
        audio.GetComponent<AudioSource>().mute = Core.isMuted;
    }

    public void Kill()
    {
        GameObject.Destroy(audio);
        GameObject.Destroy(flash);
    }
}
