using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkManagerNotificationUi : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textUi;
	[SerializeField] float displayTime = 1f;
	[SerializeField] Color colorNotification;
	[SerializeField] Color colorError;

	float timer;

	//Public Methods
	public void ShowNotification(string text)
	{
		textUi.text = text;
		textUi.color = colorNotification;
		gameObject.SetActive (true);
	}

	public void ShowError(string text)
	{
		textUi.text = text;
		textUi.color = colorError;
		gameObject.SetActive (true);
	}

	//Private Methods
	private void OnEnable()
	{
		timer = 0f;
	}

    // Update is called once per frame
    void Update()
    {
        if (timer < displayTime)
			timer += Time.deltaTime;
		else
			gameObject.SetActive (false);
    }
}
