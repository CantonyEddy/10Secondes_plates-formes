using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerBehaviour : MonoBehaviour
{
    public float timerDuration = 10f; // Durée du timer
    private float currentTime;
    public TextMeshProUGUI timerText; // Texte pour afficher le timer
    public GameObject gameOverScreen; // Écran de Game Over

    private CubeBehaviour playerScript;

    void Start()
    {
        currentTime = timerDuration;

        // Trouve le joueur dans la scène et récupère son script CubeBehaviour
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<CubeBehaviour>();
        }
        else
        {
            Debug.LogError("Le joueur avec le script CubeBehaviour n'a pas été trouvé !");
        }
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = currentTime.ToString("F3"); // Format millisecondes
        }
        else
        {
            if (playerScript != null)
            {
                currentTime = 10;
                playerScript.GameOver(); // Appelle la fonction GameOver du joueur
            }
            else
            {
                Debug.LogError("La fonction GameOver n'a pas pu être appelée !");
            }
        }
    }

    public void AddTime(float timeToAdd)
    {
        currentTime += timeToAdd;
    }
}
