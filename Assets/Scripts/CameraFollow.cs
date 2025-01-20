using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Le joueur que la caméra doit suivre
    public Vector3 offset; // Décalage entre la caméra et le joueur
    public float smoothSpeed = 0.125f; // Pour lisser les mouvements

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
