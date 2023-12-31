using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfigurationContainer : MonoBehaviour
{
	public PlayerConfiguration playerConfiguration {get;set;}

	public int TeamIndex { get {return playerConfiguration ? playerConfiguration.teamNr : 0;}}
	public short PlayerID { get {return playerConfiguration ? playerConfiguration.playerId : (short)0;}}
}
