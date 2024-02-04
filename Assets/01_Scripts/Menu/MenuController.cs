using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using Ivyyy.GameEvent;
using UnityEngine.UI;

public 	enum MenuScreens
{
	START_SCREEN,
	PLAY_MODE_SCREEN,
	CREATE_LOBBY_SCREEN,
	JOIN_LOBBY_SCREEN,
	CREDITS_SCREEN,
}

[System.Serializable]
public struct MenuScreenSetting
{
	public MenuScreens screenIndex;
	public GameObject screenUiObj;
	public GameObject CameraObj;
	public Color fadeColor;
	public float fadeInTime;
	public float fadeOutTime;
	public float maxAlpha;
}

public class MenuController : MonoBehaviour
{
    [SerializeField] NetworkManagerCallback networkManagerCallback;
	
	[Header ("UI Screens")]
	[SerializeField] List <MenuScreenSetting> screenSettings;

	[Header ("Color Fade")]
	[SerializeField] Image colorFadeImage;

	[Header ("Audio")]
	[SerializeField] GameEvent playAmbient;

	public void OnShowStartScreen()
	{
		SwitchActiveScreen (MenuScreens.START_SCREEN);
	}

	public void OnShowPlayModeScreen (bool colorFade)
	{
		if (colorFade)
			StartCoroutine (ColorFadeInTask (MenuScreens.PLAY_MODE_SCREEN));
		else
			SwitchActiveScreen (MenuScreens.PLAY_MODE_SCREEN);
	}

	public void OnShowCreateLobbyScreen ()
	{
		StartCoroutine (ColorFadeInTask (MenuScreens.CREATE_LOBBY_SCREEN));
	}

	public void OnShowJoinLobbyScreen ()
	{
		StartCoroutine (ColorFadeInTask (MenuScreens.JOIN_LOBBY_SCREEN));
	}

	public void OnCreditsPressed()
	{
		StartCoroutine (ColorFadeInTask (MenuScreens.CREDITS_SCREEN));
	}

	public void OnQuitPressed()
	{
		Application.Quit();
	}

	void Start()
	{
		networkManagerCallback.ResetNetworkObjects();
		playAmbient?.Raise();
		Cursor.visible = true;
	}

	void SwitchActiveScreen (MenuScreens targetScreen)
	{
		GameObject activeCamera = null;

		foreach (var screen in screenSettings)
		{
			bool active = screen.screenIndex == targetScreen;
			screen.CameraObj.SetActive (false);
			screen.screenUiObj.SetActive (active);

			if (active)
				activeCamera = screen.CameraObj;
		}

		if (activeCamera)
			activeCamera.SetActive (true);
	}

	MenuScreenSetting GetMenuScreenSetting (MenuScreens targetScreen)
	{
		foreach (var screen in screenSettings)
		{
			if (screen.screenIndex ==targetScreen)
				return screen;
		}

		return default;
	}

	IEnumerator ColorFadeInTask (MenuScreens targetScreen)
	{
		float timer = 0f;
		MenuScreenSetting menuScreenSetting = GetMenuScreenSetting(targetScreen);
		Color color = menuScreenSetting.fadeColor;
		color.a = 0f;

		while (timer <= menuScreenSetting.fadeInTime)
		{
			color.a = menuScreenSetting.maxAlpha * EaseOutQuad (timer / menuScreenSetting.fadeInTime);
			colorFadeImage.color = color;
			timer += Time.deltaTime;
			yield return null;
		}

		SwitchActiveScreen (targetScreen);

		yield return StartCoroutine (ColorFadeOutTask(targetScreen));
	}

	IEnumerator ColorFadeOutTask(MenuScreens targetScreen)
	{
		float timer = 0f;
		MenuScreenSetting menuScreenSetting = GetMenuScreenSetting(targetScreen);
		Color color = menuScreenSetting.fadeColor;
		color.a = 1f;

		while (timer <= menuScreenSetting.fadeOutTime)
		{
			color.a = menuScreenSetting.maxAlpha * EaseOutQuad (1 - (timer / menuScreenSetting.fadeOutTime));
			colorFadeImage.color = color;
			timer += Time.deltaTime;
			yield return null;
		}
	}

	float EaseOutQuad (float x)
	{
		return 1 - (1 - x) * (1 - x);
	}
}
