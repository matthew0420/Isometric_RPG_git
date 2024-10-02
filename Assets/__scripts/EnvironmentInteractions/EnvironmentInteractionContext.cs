using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    public enum EBodySide
    {
        RIGHT,
        LEFT
    }
    
    private TwoBoneIKConstraint _leftIKConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private TwoBoneIKConstraint _rightIKConstraint; 
    private MultiRotationConstraint _rightMultiRotationConstraint; 
    private Rigidbody _rigidbody; 
    private CapsuleCollider _rootCollider;
    private Transform _rootTransform;

    private float _characterShoulderHeight;
    //constructor
    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, MultiRotationConstraint leftMultiRotationConstraint, 
        TwoBoneIKConstraint rightIKConstraint, MultiRotationConstraint rightMultiRotationConstraint, Rigidbody rigidbody, CapsuleCollider rootCollider, Transform rootTransform)
    {
        _leftIKConstraint = leftIKConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightIKConstraint = rightIKConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
        _rootTransform = rootTransform;
        
        _characterShoulderHeight = leftIKConstraint.data.root.transform.position.y;
    }
    
    //read only context properties
    public TwoBoneIKConstraint LeftIKConstraint => _leftIKConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotationConstraint;
    public TwoBoneIKConstraint RightIKConstraint => _rightIKConstraint;
    public MultiRotationConstraint RightMultiRotationConstraint => _rightMultiRotationConstraint;
    public Rigidbody Rb => _rigidbody;
    public CapsuleCollider Rootcollider => _rootCollider;
    public Transform RootTransform => _rootTransform;
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;

    public float CharacterShoulderHeight => _characterShoulderHeight;
    
    //current tracking for which side of body to activate hand-to-wall interaction
    public Collider CurrentIntersectingCollider { get; set; }
    public TwoBoneIKConstraint CurrentIKConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIKTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIKConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIKConstraint.data.root.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);
        if (isLeftCloser)
        {
            Debug.Log("Left side is closer");
            CurrentBodySide = EBodySide.LEFT;
            CurrentIKConstraint = _leftIKConstraint;
            CurrentMultiRotationConstraint = _leftMultiRotationConstraint;
        }
        else
        {
            Debug.Log("Right side is closer");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIKConstraint = _rightIKConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
        }

        CurrentShoulderTransform = CurrentIKConstraint.data.root.transform;
        CurrentIKTargetTransform = CurrentIKConstraint.data.target.transform;
    }
}
