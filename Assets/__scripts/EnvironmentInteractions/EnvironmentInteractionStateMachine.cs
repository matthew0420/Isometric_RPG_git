using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

//accepts Enum State (EState) generic
public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EEnvironmentInteractionState>
{
    //holds the different states involved in touching a wall as we walk near it
    public enum EEnvironmentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,
    }

    private EnvironmentInteractionContext _context;
    
    //values for our hand IK and rotation
    [SerializeField] private TwoBoneIKConstraint _leftIKConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIKConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _rootCollider;

    private void Awake()
    {
        ValidateConstraints();

        _context = new EnvironmentInteractionContext(_leftIKConstraint, _leftMultiRotationConstraint,
            _rightIKConstraint, _rightMultiRotationConstraint, _rigidbody, _rootCollider);
        InitializeStates();
    }
    
    //error checking for serialize field assignments above
    private void ValidateConstraints()
    { 
        Assert.IsNotNull(_leftIKConstraint, "Left IK Constraint not assigned.");
        Assert.IsNotNull(_leftMultiRotationConstraint, "Left multi-rotation constraint is not assigned.");
        Assert.IsNotNull(_rightIKConstraint, "Right IK constraint not assigned.");
        Assert.IsNotNull(_rightMultiRotationConstraint, "Right multi-rotation constraint is not assigned.");
        Assert.IsNotNull(_rigidbody, "Character Rigidbody is not assigned.");
        Assert.IsNotNull(_rootCollider, "RootCollider attached to character is not assigned");
    }

    private void InitializeStates()
    {
        //add states to StateManager dictionary and set initial state
        States.Add(EEnvironmentInteractionState.Reset, new ResetState(_context, EEnvironmentInteractionState.Reset));
        States.Add(EEnvironmentInteractionState.Search, new SearchState(_context, EEnvironmentInteractionState.Search));
        States.Add(EEnvironmentInteractionState.Approach, new ApproachState(_context, EEnvironmentInteractionState.Approach));
        States.Add(EEnvironmentInteractionState.Rise, new RiseState(_context, EEnvironmentInteractionState.Rise));
        States.Add(EEnvironmentInteractionState.Touch, new TouchState(_context, EEnvironmentInteractionState.Touch));
        CurrentState = States[EEnvironmentInteractionState.Reset];
    }
}
