using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player;  // The player the camera will follow
    
    private Vector3 offset; 

    private void Start() => offset = transform.position - player.position;
    private void LateUpdate() => transform.position = player.position + offset;
}
