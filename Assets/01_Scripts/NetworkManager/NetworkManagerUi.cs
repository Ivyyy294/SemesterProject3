using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerUi : MonoBehaviour
{
	[SerializeField] NetworkManagerNotificationUi notificationUi;
	Queue<Tuple <string, bool>> notificationBuffer = new Queue<Tuple<string, bool>>();

	//Public Methods
	public void ShowNotification (string text)
	{
		notificationBuffer.Enqueue (new Tuple <string, bool> (text, true));
	}

	public void ShowError (string text)
	{
		notificationBuffer.Enqueue (new Tuple <string, bool> (text, false));
	}

	//Private Methods
    // Update is called once per frame
    void Update()
    {
        if (!notificationUi.gameObject.activeInHierarchy && notificationBuffer.Count > 0)
		{
			var notification = notificationBuffer.Dequeue();
			
			if (notification.Item2)
				notificationUi.ShowNotification(notification.Item1);
			else
				notificationUi.ShowError(notification.Item1);
		}
    }
}
