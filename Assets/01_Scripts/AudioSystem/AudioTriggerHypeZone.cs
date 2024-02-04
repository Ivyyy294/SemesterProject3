using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioPlayer))]
public class AudioTriggerHypeZone : MonoBehaviour
{
	[SerializeField] float fadeTime = 1f;
	[SerializeField] float radius = 6f;
	[SerializeField] LayerMask layerMask;

	AudioPlayer audioPlayer;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent <AudioPlayer>();
    }

	private void FixedUpdate()
	{
		var colliders = Physics.OverlapSphere (transform.position, radius, layerMask);

		bool playAudio = colliders.Length > 0;
		bool isPlaying = audioPlayer.IsPlaying();

		if (playAudio && !isPlaying)
			audioPlayer.Play();
		else if (!playAudio && isPlaying)
			audioPlayer.FadeOut(fadeTime);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (transform.position, radius);
	}
}
