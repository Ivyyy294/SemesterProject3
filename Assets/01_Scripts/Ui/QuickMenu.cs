using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ivyyy.Network;
using UnityEngine.SceneManagement;

public class QuickMenu : MonoBehaviour
{
	[SerializeField] PlayerManager playerManager;
	[SerializeField] GameObject uiObj;
    // Start is called before the first frame update
    void Start()
    {
        uiObj.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.Escape))
			uiObj.SetActive (!uiObj.activeInHierarchy);
    }

	public void OnContinuePressed()
	{
		uiObj.SetActive (false);
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
