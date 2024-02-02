using System.Collections;
using System.Collections.Generic;
using Ivyyy.GameEvent;
using UnityEngine;

[RequireComponent(typeof(GameEventListener))]
public class MatchVisuals : MonoBehaviour
{
    public void OnGoalScored()
    {
        GlobalPostProcessing.Me.Transition();
    }
}
