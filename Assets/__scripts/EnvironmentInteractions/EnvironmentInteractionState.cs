using UnityEngine;

public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    protected EnvironmentInteractionContext Context;

    //stateKey must be one of the five interactions described in EI state machine
    public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EEnvironmentInteractionState stateKey) : base(stateKey)
    {
        Context = context;
    }

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIKTargetPositionTracking(Collider intersectingCollider)
    {
        Vector3 closestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
        Context.SetCurrentSide(closestPointFromRoot);
    }
    protected void UpdateIKTargetPositionTracking(Collider intersectingCollider)
    {
        
    }
    protected void ResetIKTargetPositionTracking(Collider intersectingCollider)
    {
        
    }
}
