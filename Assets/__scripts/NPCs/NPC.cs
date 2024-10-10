using UnityEngine;
using UnityEngine.AI;

//NPC base class
public class NPC : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform player;
    protected bool isRunning = false;
    
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    protected void MoveTo(Vector3 target)
    {
        agent.SetDestination(target);
        if (!isRunning)
        {
            animator.SetBool("isRunning", false);
            agent.speed = 2f; // Walking speed
        }
    }
    
    public virtual void ReactToPlayer() {}
    
    protected void UpdateAnimation()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    protected virtual void Update()
    {
        UpdateAnimation();
    }
}
