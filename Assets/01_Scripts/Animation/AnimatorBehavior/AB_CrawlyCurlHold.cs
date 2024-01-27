using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_CrawlyCurlHold : StateMachineBehaviour
{
    private CrawlyVisuals _visuals;
    private TimeCounter _spherizeTimer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _spherizeTimer = new (0.1f);
        _visuals = animator.gameObject.GetComponent<CrawlyVisuals>();
        _visuals.spherize = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _spherizeTimer.Update();
        _visuals.spherize = Mathf.Clamp01(_spherizeTimer.ProgressNormalized);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _visuals.spherize = 0;
    }
    
}
