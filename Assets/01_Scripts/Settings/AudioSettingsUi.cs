using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUi : MonoBehaviour
{
	[SerializeField] Slider musicSetting;
	[SerializeField] Slider ambientSetting;
	[SerializeField] Slider sfxSetting;
	[SerializeField] Slider voiceSetting;
	[SerializeField] Slider uiSetting;
	[SerializeField] Toggle subtitle;

	AudioSettings audioSettings;

	private void OnEnable()
	{
		audioSettings = GameSettings.Me().audioSettings;
		musicSetting.value = audioSettings.musicVolume * 10f;
		ambientSetting.value = audioSettings.ambientVolume * 10f;
		sfxSetting.value = audioSettings.sfxVolume * 10f;
		voiceSetting.value = audioSettings.voiceLine * 10f;
		uiSetting.value = audioSettings.uiVolume * 10f;
		subtitle.isOn = audioSettings.subtitle;
	}

	private void OnDisable()
	{
		Save();
	}

	public void Save()
	{
		audioSettings.musicVolume = musicSetting.value * 0.1f;
		audioSettings.ambientVolume = ambientSetting.value * 0.1f;
		audioSettings.sfxVolume = sfxSetting.value * 0.1f;
		audioSettings.voiceLine = voiceSetting.value * 0.1f;
		audioSettings.uiVolume = uiSetting.value * 0.1f;
		audioSettings.subtitle = subtitle.isOn;
		audioSettings.SaveSettings();
	}
}
