using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public ennemy_data enemyData; // ScriptableObject contenant les données de l'ennemi
    private CubeBehaviour playerScript;
    public float detectionRadius = 5f; // Distance à laquelle l'abeille détecte le joueur
    public LayerMask playerLayer; // Layer du joueur
    public Animator _animator;

    void Start()
    {
        // Récupère l'objet avec le tag "Player"
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Récupère le script CubeBehaviour du joueur
            playerScript = player.GetComponent<CubeBehaviour>();
        }
        else
        {
            Debug.LogError("Player non trouvé !");
        }
    }

    void Update()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (playerScript != null && playerCollider != null)
        {
            _animator.SetBool("isRunning", true);
            // Récupère la position du joueur via son script
            Vector3 playerPosition = playerScript.PlayerPosition;
            
            // Ne déplace l'ennemi que sur l'axe X
            float direction = Mathf.Sign(playerPosition.x - transform.position.x); // -1 pour gauche, 1 pour droite
            // Retourne le sprite de l'ennemi en fonction de la direction du joueur
            if (direction > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            Vector3 movement = new Vector3(direction * enemyData.speed * Time.deltaTime, 0, 0);

            transform.Translate(movement, Space.World);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

    }
    private void OnDrawGizmosSelected()
    {
        // Dessine la zone de détection dans la vue scène pour le debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
