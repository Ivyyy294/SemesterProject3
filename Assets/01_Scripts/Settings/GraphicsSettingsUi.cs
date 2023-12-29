using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class GraphicsSettingsUi : MonoBehaviour
{
	[SerializeField] Toggle fullscreen;
	[SerializeField] TMP_Dropdown resolutionDropDown;

	GraphicSettings graphicSettings;
	int currentIndex;

	private void Awake()
	{
		currentIndex = -1;
		graphicSettings = GameSettings.Me().graphicSettings;

		fullscreen.isOn = graphicSettings.fullscreen;
		resolutionDropDown.options.Clear();

		for (int i = 0; i < graphicSettings.availableSettings.Count; ++i)
		{
			GraphicSettings.Setting tmp = graphicSettings.availableSettings[i];
			resolutionDropDown.options.Add(new TMP_Dropdown.OptionData(tmp.GetDisplayName()));

			if (tmp.Compare (graphicSettings.currentSetting))
				currentIndex = i;
		}
		
		resolutionDropDown.value = currentIndex;
	}

	private void Update()
	{
		if (resolutionDropDown.value != currentIndex)
		{
			currentIndex = resolutionDropDown.value;
			graphicSettings.currentSetting = graphicSettings.availableSettings[currentIndex];
			graphicSettings.SaveSettings();
			Screen.SetResolution (graphicSettings.currentSetting.width, graphicSettings.currentSetting.height, fullscreen.isOn);
		}
	}

	public void OnFullscreenToggle (bool val)
	{
		graphicSettings.fullscreen = fullscreen.isOn;
		graphicSettings.SaveSettings();
		Screen.fullScreen = graphicSettings.fullscreen;
	}
}
