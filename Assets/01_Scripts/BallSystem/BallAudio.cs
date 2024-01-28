using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioPlayer))]
public class BallAudio : MonoBehaviour
{
	[SerializeField] AudioAsset audioSwimming;
	[SerializeField] AudioAsset audioSleeping;
	[SerializeField] float fadeTime = 0.5f;

	AudioPlayer audioPlayer;
	CrawlyBrain crawlyBrain;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
		crawlyBrain = GetComponent<CrawlyBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (crawlyBrain.IsSleeping && audioPlayer.AudioAsset() != audioSleeping)
			audioPlayer.Play (audioSleeping);
		else if (crawlyBrain.BrainActive 
			&& (audioPlayer.AudioAsset() != audioSleeping || !audioPlayer.IsPlaying()))
			audioPlayer.Play (audioSwimming);
		else if (!crawlyBrain.BrainActive && audioPlayer.IsPlaying ())
			audioPlayer.FadeOut (fadeTime);
    }
}
