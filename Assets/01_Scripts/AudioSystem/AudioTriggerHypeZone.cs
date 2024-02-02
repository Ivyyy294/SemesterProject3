using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioPlayer))]
public class AudioTriggerHypeZone : MonoBehaviour
{
	[SerializeField] float fadeTime = 1f;

	AudioPlayer audioPlayer;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent <AudioPlayer>();
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ball") && !audioPlayer.IsPlaying())
			audioPlayer.Play();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Ball") && audioPlayer.IsPlaying())
			audioPlayer.FadeOut(fadeTime);	
	}
}
