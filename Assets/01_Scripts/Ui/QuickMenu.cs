using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.SceneManagement;
using Ivyyy.GameEvent;

public class QuickMenu : MonoBehaviour
{
	[SerializeField] PlayerManager playerManager;
	[SerializeField] GameObject uiObj;

	[Header ("Cursor")]
	[SerializeField] GameEvent showCursor;
	[SerializeField] GameEvent hideCursor;

	MatchGameOver matchGameOver;

    // Start is called before the first frame update
    void Start()
    {
        uiObj.SetActive (false);

		if (MatchController.Me)
			matchGameOver = MatchController.Me.MatchGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.Escape) && (matchGameOver == null || !matchGameOver.GameOver()))
		{
			if (!uiObj.activeInHierarchy)
			{
				uiObj.SetActive (true);
				showCursor.Raise();
			}
			else
				OnContinuePressed();
		}
    }

	public void OnContinuePressed()
	{
		uiObj.SetActive (false);
		hideCursor.Raise();
	}

	public void OnReturnToMenuPressed()
	{
		if (NetworkManager.Me)
			NetworkManager.Me.ShutDown();

		if (NetworkSceneController.Me)
		{
			NetworkSceneController.Me.Owner = true;
			NetworkSceneController.Me.LoadScene(0);
		}
		else
			SceneManager.LoadScene (0);
	}

	public void OnQuitGamePressed()
	{
		if (NetworkManager.Me)
			NetworkManager.Me.ShutDown();

		Application.Quit();
	}
}
