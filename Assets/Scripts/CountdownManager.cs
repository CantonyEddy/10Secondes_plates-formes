using UnityEngine;
using UnityEngine.UI; // Pour manipuler l'interface utilisateur (UI)
using UnityEngine.SceneManagement; // Pour gérer les scènes
using System.Collections; // Pour utiliser IEnumerator
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Assignez un objet Text depuis l'éditeur Unity
    public float countdownDuration = 3f; // Durée du compte à rebours en secondes

    private float countdownTimer;
    private bool countdownActive = true;

    void Start()
    {
        countdownTimer = countdownDuration; // Initialiser le compte à rebours
        Time.timeScale = 0f; // Mettre le jeu en pause
    }

    void Update()
    {
        if (countdownActive)
        {
            countdownTimer -= Time.unscaledDeltaTime; // Compte à rebours en temps réel (non affecté par Time.timeScale)

            if (countdownTimer > 0)
            {
                // Afficher les secondes restantes (arrondi à l'entier inférieur)
                countdownText.text = Mathf.Ceil(countdownTimer).ToString();
            }
            else
            {
                // Terminer le compte à rebours
                countdownActive = false;
                countdownText.text = "GO!";
                StartCoroutine(StartGameAfterDelay(1f)); // Optionnel : Attendre avant de démarrer le jeu
            }
        }
    }

    private IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Attendre un court moment (indépendamment de Time.timeScale)
        countdownText.gameObject.SetActive(false); // Cacher le texte
        Time.timeScale = 1f; // Reprendre le jeu
    }
}
