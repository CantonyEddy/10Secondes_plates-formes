using UnityEngine;

public class BeeBehaviour : MonoBehaviour
{
    public ennemy_data enemyData; // ScriptableObject contenant les données de l'ennemi
    public float circleRadius = 3f; // Rayon du cercle de vol
    public float flySpeed; // Vitesse de vol en cercle
    public float attackSpeed = 5f; // Vitesse d'attaque vers le joueur
    public float detectionRadius = 5f; // Distance à laquelle l'abeille détecte le joueur
    public LayerMask playerLayer; // Layer du joueur
    public float attackCooldown = 2f; // Temps d'attente entre deux attaques
    public Animator _animator;

    private Vector3 centerPosition; // Position centrale pour le mouvement circulaire
    private float angle; // Angle actuel pour calculer la position circulaire
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;
    private Transform player;

    void Start()
    {
        // Définir la position centrale de l'abeille comme son point de départ
        centerPosition = transform.position;
        flySpeed = enemyData.speed;
    }

    void Update()
    {
        // Réduire le temps de cooldown de l'attaque
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (!isAttacking)
        {
            // Vérifie si un joueur est dans la zone de détection
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

            if (playerCollider != null && attackCooldownTimer <= 0f)
            {
                // Détecte le joueur et prépare une attaque
                _animator.SetBool("isAtk", true);
                // Retourner le sprite de l'abeille en direction du joueur
                Vector3 direction = playerCollider.transform.position - transform.position;
                player = playerCollider.transform;
                isAttacking = true;
            }
            else
            {
                // Si aucun joueur n'est détecté, voler en cercle
                _animator.SetBool("isAtk", false);
                FlyInCircle();
            }
        }
        else
        {
            // Attaque le joueur
            AttackPlayer();
        }
    }

    void FlyInCircle()
    {
        // Calculer la nouvelle position en cercle
        angle += flySpeed * Time.deltaTime;
        float x = Mathf.Cos(angle) * circleRadius;
        float y = Mathf.Sin(angle) * circleRadius;

        transform.position = centerPosition + new Vector3(x, y, 0);
    }

    void AttackPlayer()
    {
        if (player != null)
        {
            // Déplacer l'abeille vers la position du joueur
            transform.position = Vector3.MoveTowards(transform.position, player.position, attackSpeed * Time.deltaTime);
            // Retourner le sprite de l'abeille en direction du joueur
            Vector3 direction = player.position - transform.position;
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            // Si elle atteint le joueur, termine l'attaque
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                EndAttack();
            }
        }
        else
        {
            // Si le joueur est perdu (par exemple détruit), retourner au mouvement circulaire
            EndAttack();
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        attackCooldownTimer = attackCooldown; // Appliquer le cooldown avant une nouvelle attaque
        player = null; // Réinitialiser la cible
    }

    private void OnDrawGizmosSelected()
    {
        // Dessine la zone de détection dans la vue scène pour le debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
