using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//*code has been added for punching, would most likely be delegated to an attack class later on, or even an 'action' class that would pause certain IK interaction
//**states have been made for different phases of the state machine to show how they would ideally flow, but not all are being used for the purpose of this demo
//***placeholder melee attack, not really realistic
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
        if (!Context.MyCharacterController.IsPunching)
        {
            Context.AdjustRigWeight(50f);  // Enable rig weight for environment interaction
        }
    }
    public override void ExitState(){Debug.Log("TouchState exit");}
    public override void UpdateState()
    {
        // Adjust IK weights based on the current body side (left or right)
        Context.AdjustIKWeights(Context.CurrentBodySide, 50f);
    }
    public override void OnTriggerEnter(Collider other)
    {Debug.Log("TouchState trigger enter");
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable") && !Context.MyCharacterController.IsPunching)
        {
            StartIKTargetPositionTracking(other);
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (!Context.MyCharacterController.IsPunching)
        {
            Context.AdjustRigWeight(50f);
            StartIKTargetPositionTracking(other);
            UpdateIKTargetPosition(other);
        }
        else
        {
            ResetIKTargetPositionTracking(other);
            Context.ResetAllWeights();  // Reset IK weights and rig weight when leaving the wall
        }
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
