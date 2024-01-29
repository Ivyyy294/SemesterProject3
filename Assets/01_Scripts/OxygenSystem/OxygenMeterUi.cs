using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ivyyy.Utils;

public class OxygenMeterUi : MonoBehaviour
{
	[Header ("Audio")]
	[SerializeField] AudioAsset audio75;
	[SerializeField] AudioAsset audio50;
	[SerializeField] AudioAsset audio25;

	[Header ("Lara Values")]
	[SerializeField] TextMeshProUGUI oxygenLabel;
	PlayerOxygen playerOxygen;

	[Header("Mushroom Values")]
	[SerializeField] RectTransform oxygenBar;
	
	BitSet audioMemory = new BitSet (1);
	private int OxygenBarWidth;

    // Start is called before the first frame update
    void Start()
    {
		playerOxygen = PlayerManager.LocalPlayer.GetComponentInChildren<PlayerOxygen>();
		OxygenBarWidth = 100;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (oxygenLabel && playerOxygen)
			oxygenLabel.text = ((int)playerOxygen.CurrentOxygenPercent).ToString();*/

		OxygenBarWidth = 100 - ((int)playerOxygen.CurrentOxygenPercent);
		oxygenBar.sizeDelta = new Vector2(OxygenBarWidth,64);

        UpdateAudioMemory();
		PlayAudio();
    }

	void UpdateAudioMemory()
	{
		if (playerOxygen.CurrentOxygenPercent > 75f && audioMemory.Check(0))
			audioMemory.SetBit (0, false);
		if (playerOxygen.CurrentOxygenPercent > 50f && audioMemory.Check(0))
			audioMemory.SetBit (1, false);
		if (playerOxygen.CurrentOxygenPercent > 25f && audioMemory.Check(0))
			audioMemory.SetBit (2, false);
	}

	void PlayAudio()
	{
		if (playerOxygen.CurrentOxygenPercent <= 25f)
			PlayAudio (audio25, 2);
		else if (playerOxygen.CurrentOxygenPercent <= 50f)
			PlayAudio (audio50, 1);
		else if (playerOxygen.CurrentOxygenPercent <= 75f)
			PlayAudio (audio75, 0);
	}

	void PlayAudio (AudioAsset audioAsset, int bitNr)
	{
		if (!audioMemory.Check (bitNr))
		{
			audioAsset.Play();
			audioMemory.SetBit (bitNr, true);
		}
	}
}
