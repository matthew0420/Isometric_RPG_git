using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TouchState : EnvironmentInteractionState
{
    public TouchState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState estate) : base(context, estate)
    {
        EnvironmentInteractionContext Context = context;
    }
    
    public override void EnterState()
    {
        Debug.Log("TouchState enter");
        // Adjust weights for the arm depending on the detected side
        Context.AdjustRigWeight(50f);  // Enable rig weight for environment interaction
    }
    public override void ExitState(){Debug.Log("TouchState exit");}
    public override void UpdateState()
    {
        // Adjust IK weights based on the current body side (left or right)
        Context.AdjustIKWeights(Context.CurrentBodySide, 50f);
    }
    public override void OnTriggerEnter(Collider other)
    {Debug.Log("TouchState trigger enter");
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            StartIKTargetPositionTracking(other);
            Context.RotateHandTargetToWall(Context.CurrentBodySide, other);
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        UpdateIKTargetPosition(other);
        Context.RotateHandTargetToWall(Context.CurrentBodySide, other);
    }
    public override void OnTriggerExit(Collider other)
    {
        ResetIKTargetPositionTracking(other);
        Context.ResetAllWeights();  // Reset IK weights and rig weight when leaving the wall
    }
    
    public override EnvironmentInteractionStateMachine.EEnvironmentInteractionState GetNextState()
    {
        if (Context.CurrentIntersectingCollider == null)  // If no more wall interaction
        {
            return EnvironmentInteractionStateMachine.EEnvironmentInteractionState.Search;
        }

        // Remain in TouchState
        return StateKey;
    }
}
