using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    private TwoBoneIKConstraint _leftIKConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private TwoBoneIKConstraint _rightIKConstraint; 
    private MultiRotationConstraint _rightMultiRotationConstraint; 
    private Rigidbody _rigidbody; 
    private CapsuleCollider _rootCollider;

    //constructor
    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, MultiRotationConstraint leftMultiRotationConstraint, 
        TwoBoneIKConstraint rightIKConstraint, MultiRotationConstraint rightMultiRotationConstraint, Rigidbody rigidbody, CapsuleCollider rootCollider)
    {
        _leftIKConstraint = leftIKConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightIKConstraint = rightIKConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
    }
    
    //read only properties
    public TwoBoneIKConstraint LeftIKConstraint => _leftIKConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotationConstraint;
    public TwoBoneIKConstraint RightIKConstraint => _rightIKConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotationConstraint;
    public Rigidbody Rb => _rigidbody;
    public CapsuleCollider Rootcollider => _rootCollider;
}
