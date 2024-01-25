using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoldBall : MonoBehaviour
{
	[SerializeField] Transform ballHoldPos;
	int playerId;

	public static List <PlayerHoldBall> InstanzList {get; private set;}

	public int PlayerId => playerId;
	public Transform BallHoldTransform => ballHoldPos;

    // Start is called before the first frame update
    void Start()
    {
		playerId = transform.parent.GetComponentInChildren<PlayerConfigurationContainer>().PlayerID;

		if (InstanzList == null)
			InstanzList = new List<PlayerHoldBall>();

		InstanzList.Add (this);
    }

	private void OnDestroy()
	{
		InstanzList.Remove (this);
	}
}
