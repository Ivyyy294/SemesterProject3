using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioPlayer))]
public class AmbientMusicPlayer : MonoBehaviour
{
	[SerializeField] AudioAsset ambientNormal;
	[SerializeField] AudioAsset ambientDeep;
	[SerializeField] float transitionTime = 1f;

	AudioPlayer audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
    }

	public void PlayAmbientNormal()
	{
		PlayAudio (ambientNormal);
	}

	public void PlayAmbientDeep()
	{
		PlayAudio (ambientDeep);
	}

	void PlayAudio (AudioAsset audioAsset)
	{
		if (audioPlayer.IsPlaying())
			audioPlayer.Transition (audioAsset, transitionTime);
		else
			audioPlayer.FadeIn(audioAsset, transitionTime);
	}
}
