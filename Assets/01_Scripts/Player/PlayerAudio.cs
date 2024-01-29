using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;

public class PlayerAudio : NetworkBehaviour
{
	[SerializeField] float audiofadeTime = 0.5f;
	[SerializeField] AudioPlayer audioSwimming;
	[SerializeField] AudioPlayer audioDashing;
	[SerializeField] AudioAsset audioThrow;
	[SerializeField] AudioAsset audioCatch;
	[SerializeField] AudioAsset audioInhale;
	[SerializeField] AudioAsset audioSteal;

	PlayerInputProcessing playerInput;

	protected override void SetPackageData() {}

    // Start is called before the first frame update
    void Start()
    {
        Owner = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>().IsLocalPlayer();
		playerInput = transform.parent.GetComponentInChildren<PlayerInputProcessing>();
    }

    // Update is called once per frame
    void Update()
    {
		UdateSwimmingAudio();
		UdateDashAudio();
    }

	void UdateSwimmingAudio()
	{
		bool isSwimming = playerInput.ForwardPressed && !playerInput.DashPressed;

		if (isSwimming && !audioSwimming.IsPlaying())
			audioSwimming.Play();
		else if (!isSwimming && audioSwimming.IsPlaying())
			audioSwimming.FadeOut(audiofadeTime);
	}

	void UdateDashAudio()
	{
		bool isDashing = playerInput.ForwardPressed && playerInput.DashPressed;

		if (isDashing && !audioDashing.IsPlaying())
			audioDashing.Play();
		else if (!isDashing && audioDashing.IsPlaying())
			audioDashing.FadeOut(audiofadeTime);
	}

	[RPCAttribute]
	public void PlayAudioThrow()
	{
		if (Owner)
			InvokeRPC("PlayAudioThrow");

		Debug.Log("PlayAudioThrow");
        audioThrow.PlayAtPos (transform.position);
	}

	[RPCAttribute]
	public void PlayAudioCatch()
	{
		if (Owner)
			InvokeRPC("PlayAudioCatch");

		Debug.Log("PlayAudioCatch");
        audioCatch.PlayAtPos (transform.position);
	}

	[RPCAttribute]
	public void PlayAudioInhale()
	{
		if (Owner)
			InvokeRPC("PlayAudioInhale");

		Debug.Log("PlayAudioInhale");
        audioInhale.PlayAtPos (transform.position);
	}

	[RPCAttribute]
	public void PlayAudioSteal()
	{
		if (Owner)
			InvokeRPC("PlayAudioSteal");

		Debug.Log("PlayAudioSteal");
        audioSteal.PlayAtPos (transform.position);
	}
}
