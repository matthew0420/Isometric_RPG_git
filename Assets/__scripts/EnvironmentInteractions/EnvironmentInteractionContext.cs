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
    private Rig _environmentInteractionRig; 

    private GameObject _characterShoulderHeight;
    //constructor
    public EnvironmentInteractionContext(TwoBoneIKConstraint leftIKConstraint, 
        MultiRotationConstraint leftMultiRotationConstraint, 
        TwoBoneIKConstraint rightIKConstraint, 
        MultiRotationConstraint rightMultiRotationConstraint, 
        Rigidbody rigidbody, 
        CapsuleCollider rootCollider, 
        Transform rootTransform, 
        Rig environmentInteractionRig, 
        GameObject characterShoulderHeight)
    {
        _leftIKConstraint = leftIKConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightIKConstraint = rightIKConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
        _rootTransform = rootTransform;
        _environmentInteractionRig = environmentInteractionRig;
        _characterShoulderHeight = characterShoulderHeight;
        //leftIKConstraint.data.root.parent.gameObject
        Debug.Log("Left IK constraint: " + leftIKConstraint.data.root);
        // leftIKConstraint.data.root.parent.transform.position.y-0.2f;

        ResetAllWeights();
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

    public GameObject CharacterShoulderHeight => _characterShoulderHeight;
    
    //current tracking for which side of body to activate hand-to-wall interaction
    public Collider CurrentIntersectingCollider { get; set; }
    public TwoBoneIKConstraint CurrentIKConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIKTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }

    // Reset both IK weights and the environment rig to 0
    
    // Method to control rig weight when close to a wall
    public void AdjustRigWeight(float targetWeight)
    {
        _environmentInteractionRig.weight = Mathf.Lerp(_environmentInteractionRig.weight, targetWeight, Time.deltaTime * 5f);
    }
    
    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIKConstraint.data.root.parent.transform.position;
        //Debug.Log("Constraing name:" + _leftIKConstraint.data.root.parent);
        Vector3 rightShoulder = _rightIKConstraint.data.root.parent.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);
        if (isLeftCloser)
        {
            //Debug.Log("Left side is closer");
            CurrentBodySide = EBodySide.LEFT;
            CurrentIKConstraint = _leftIKConstraint;
            CurrentMultiRotationConstraint = _leftMultiRotationConstraint;
        }
        else
        {
            //Debug.Log("Right side is closer");
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIKConstraint = _rightIKConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
        }

        CurrentShoulderTransform = CurrentIKConstraint.data.root.transform;
        CurrentIKTargetTransform = CurrentIKConstraint.data.target.transform;
    }
    
    private float _currentLeftIKWeight = 0f;
    private float _currentRightIKWeight = 0f;
    
    // Adjust the IK weight gradually over time
    public void AdjustIKWeights(EBodySide side, float targetWeight)
    {
        float lerpSpeed = Time.deltaTime * 2f;  // Speed of the transition

        if (side == EBodySide.LEFT)
        {
            _currentLeftIKWeight = Mathf.Lerp(_currentLeftIKWeight, targetWeight, lerpSpeed);
            _leftIKConstraint.weight = _currentLeftIKWeight;
            _leftMultiRotationConstraint.weight = _currentLeftIKWeight;

            // Set right side IK weight to zero instantly (disable right hand)
            _currentRightIKWeight = 0f;
            _rightIKConstraint.weight = _currentRightIKWeight;
            _rightMultiRotationConstraint.weight = _currentRightIKWeight;
        }
        else if (side == EBodySide.RIGHT)
        {
            _currentRightIKWeight = Mathf.Lerp(_currentRightIKWeight, targetWeight, lerpSpeed);
            _rightIKConstraint.weight = _currentRightIKWeight;
            _rightMultiRotationConstraint.weight = _currentRightIKWeight;

            // Set left side IK weight to zero instantly (disable left hand)
            _currentLeftIKWeight = 0f;
            _leftIKConstraint.weight = _currentLeftIKWeight;
            _leftMultiRotationConstraint.weight = _currentLeftIKWeight;
        }
    }

    // Method to reset both arm IK weights
    public void ResetAllWeights()
    {
        _currentLeftIKWeight = 0f;
        _currentRightIKWeight = 0f;

        _leftIKConstraint.weight = 0f;
        _leftMultiRotationConstraint.weight = 0f;
        _rightIKConstraint.weight = 0f;
        _rightMultiRotationConstraint.weight = 0f;
    }
}
