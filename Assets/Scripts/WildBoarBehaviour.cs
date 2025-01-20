using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public ennemy_data enemyData; // ScriptableObject contenant les données de l'ennemi
    private CubeBehaviour playerScript;

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
        if (playerScript != null)
        {
            // Récupère la position du joueur via son script
            Vector3 playerPosition = playerScript.PlayerPosition;

            // Ne déplace l'ennemi que sur l'axe X
            float direction = Mathf.Sign(playerPosition.x - transform.position.x); // -1 pour gauche, 1 pour droite
            Vector3 movement = new Vector3(direction * enemyData.speed * Time.deltaTime, 0, 0);

            transform.Translate(movement, Space.World);
        }

    }
}
