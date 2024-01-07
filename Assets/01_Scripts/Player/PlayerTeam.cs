using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
	//ToDo component
	public PlayerConfiguration playerConfiguration;
	public int TeamIndex {get{return playerConfiguration ? playerConfiguration.teamNr : 0;} }
}
