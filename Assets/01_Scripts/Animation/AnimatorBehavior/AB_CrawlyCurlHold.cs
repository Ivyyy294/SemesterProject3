using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AB_CrawlyCurlHold : StateMachineBehaviour
{
    private CrawlyVisuals _visuals;
    private TimeCounter _spherizeTimer;
    public bool startCurledUp;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _spherizeTimer = new (0.1f);
        _visuals = animator.gameObject.GetComponent<CrawlyVisuals>();
        _visuals.spherize = startCurledUp? 1 : 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _spherizeTimer.Update();
        _visuals.spherize = startCurledUp ? 1 : Mathf.Clamp01(_spherizeTimer.ProgressNormalized);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _visuals.spherize = 0;
    }
    
}
