using UnityEngine;
using UnityEngine.AI;
public class PlayerDetection : MonoBehaviour
{
    private NPC npc;

    private void Start()
    {
        npc = GetComponent<NPC>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            npc.ReactToPlayer();
        }
    }
}