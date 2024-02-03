using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRecoverFromDisabledPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
		if (Ball.Me && Ball.Me.CurrentPlayerId != -1 && !Ball.Me.isActiveAndEnabled)
		{
			Vector3 pos = Ball.Me.transform.position;
			Ball.Me.GetComponent<BallParentController>().ResetParent();
			Ball.Me.gameObject.SetActive (true);
			Ball.Me.BallDrop (pos);
		}
    }
}
