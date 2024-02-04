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
        if (crawlyBrain.IsSleeping)
			SwitchAudio (audioSleeping);
		else if (crawlyBrain.CanMove)
			SwitchAudio (audioSwimming);
		else if (audioPlayer.IsPlaying ())
			audioPlayer.FadeOut (fadeTime);
    }

	private void SwitchAudio(AudioAsset newAudio)
	{
		if (audioPlayer.AudioAsset() != newAudio || !audioPlayer.IsPlaying())
			audioPlayer.Play (newAudio);
	}
}
