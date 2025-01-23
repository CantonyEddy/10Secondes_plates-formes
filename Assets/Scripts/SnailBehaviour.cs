using UnityEngine;

public class SnailBehaviour : MonoBehaviour
{
    public ennemy_data enemyData; // ScriptableObject contenant les données de l'ennemi
    public float speed;
    public float groundCheckDistance = 0.5f; // Longueur du rayon pour vérifier le vide
    public float wallCheckDistance = 0.5f; // Longueur du rayon pour vérifier les murs
    public LayerMask groundLayer; // Layer pour le sol et les murs
    private bool movingRight = true;

    void Start()
    {
        speed = enemyData.speed;
    }

    void Update()
    {
        // Déplacement
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Vérification du vide (sol) et des murs
        if (!IsGroundAhead() || IsWallAhead())
        {
            Flip();
        }
    }

    bool IsGroundAhead()
    {
        // Envoie un rayon vers le bas devant l'ennemi
        Vector2 rayOrigin = new Vector2(transform.position.x + (movingRight ? wallCheckDistance : -wallCheckDistance), transform.position.y-0.3f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // Affiche le rayon dans la scène pour le debug
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.green);

        return hit.collider != null; // Retourne true si le rayon touche le sol
    }

    bool IsWallAhead()
    {
        // Envoie un rayon vers l'avant
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, wallCheckDistance, groundLayer);

        // Affiche le rayon dans la scène pour le debug
        Debug.DrawRay(transform.position, rayDirection * wallCheckDistance, Color.red);

        return hit.collider != null; // Retourne true si le rayon touche un mur
    }

    void Flip()
    {
        // Change la direction
        movingRight = !movingRight;
        speed = -speed;

        // Inverse l'échelle pour changer de direction visuelle
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
