using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnterHostAddress : MonoBehaviour
{
	[SerializeField] NetworkManagerCallback networkManagerCallback;
    [SerializeField] TMP_InputField inputIp;

	public void JoinHostSession ()
	{
		networkManagerCallback.OnClientStarted (inputIp.text);
	}
}
