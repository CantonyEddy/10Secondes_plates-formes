using UnityEngine;
using System.Collections;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;

public class CubeBehaviour : MonoBehaviour
{
    public float moveSpeed = 10f; // Vitesse de déplacement
    public float jumpForce = 10f; // Force de saut
    public float groundLoadForce = -20f; // Force de saut
    public float dashSpeed = 20f; // Vitesse pendant le dash
    public float dashDuration = 0.2f; // Durée du dash
    public float dashCooldown = 1f; // Temps entre deux dashs
    public float slideSpeed = 0.99f; //
    public Transform groundCheck; // Point pour vérifier le contact avec le sol
    public Transform leftGroundCheck; // Point pour vérifier le contact avec le mur Gauche
    public Transform rightGroundCheck; // Point pour vérifier le contact avec le mur Gauche
    public Transform atkLeft;
    public Transform atkRight;
    public float checkRadius = 0.2f; // Rayon de détection
    public Vector2 groundCheckSize = new Vector2(1f, 0.1f); // Taille de la zone de détection
    public LayerMask groundLayer; // Layer correspondant au sol
    public LayerMask enemyLayer;

    public Vector3 PlayerPosition => transform.position;

    public GameObject gameOverScreen; // Référence au Game Over Canvas
    public GameObject winScreen; // Référence au Game Over Canvas

    private Rigidbody2D rb;
    //private bool isGrounded;
    private bool isDashing = false;
    private bool isSliding = false;
    private bool inAir = false;
    private float dashCooldownTimer = 0f;
    private float vitesseSpeed = 0f;
    private TimerBehaviour timerScript;

    void Start()
    {
        // Récupère le Rigidbody2D du joueur
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            timerScript = player.GetComponent<TimerBehaviour>();
        }
        else
        {
            Debug.LogError("Le joueur avec le script TimerBehaviour n'a pas été trouvé !");
        }
    }

    void Update()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        // Gestion du dash
        if (Input.GetKeyDown(KeyCode.Return) && dashCooldownTimer <= 0f  && IsGrounded())
        {
            StartCoroutine(Dash());
            vitesseSpeed = dashSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            float moveInput = Input.GetAxis("Horizontal");
            if (moveInput < 0)
            {
                IsAtkLeftEnemy();
            }
            else
            {
                IsAtkRightEnemy();
            }
        }
        if (IsGroundedEnemy() && !IsGroundedEnemyLeftOrRight())
        {
            // Récupère tous les ennemis dans le rayon
            Collider2D enemies = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, enemyLayer);
            // Détruit ennemi
            if (enemies.gameObject.GetComponent<EnemyData>().enemyData.isDangerous){
                timerScript.AddTime(enemies.gameObject.GetComponent<EnemyData>().enemyData.time);
                enemies.gameObject.GetComponent<EnemyData>().destroy();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && !IsGrounded() && Input.GetKey(KeyCode.DownArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, groundLoadForce);
            vitesseSpeed = 0;
        }

        // Déplacement gauche/droite si pas en dash
        if (!isDashing && !isSliding && vitesseSpeed <= moveSpeed && vitesseSpeed >= -moveSpeed)
        {
            //Debug.Log(IsGroundedLeft() + "Left");
            //Debug.Log(IsGroundedRight() + "Right");
            float moveInput = Input.GetAxis("Horizontal");
            if ((moveInput <= 0 && !IsGroundedLeft()) || (moveInput >= 0 && !IsGroundedRight()))
            {
                vitesseSpeed = moveInput * moveSpeed;
            }
            else
            {
                vitesseSpeed = 0;
            }
        }

        // Saut (seulement si au sol)
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Réduire le cooldown du dash
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        float moveInputD = Input.GetAxis("Horizontal");
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(vitesseSpeed, rb.linearVelocity.y);
            if (Input.GetKey(KeyCode.DownArrow) && IsGrounded())
            {
                //Debug.Log("Touche bas");
                vitesseSpeed *= slideSpeed;
                isSliding = true;
                
            }else if (IsGrounded()){
                //Debug.Log("Pas touche bas");
                vitesseSpeed = 0;
                isSliding = false;
                inAir = false;
            }else if (!IsGrounded() && SameSign(moveInputD, vitesseSpeed)){
                inAir = true;
            }else{
                isSliding = false;
                if (IsGrounded())
                {
                    vitesseSpeed = 0;
                    inAir = false;
                }
                if ((!(moveInputD <= 0 && !IsGroundedLeft()) || !(moveInputD >= 0 && !IsGroundedRight())) && !IsGrounded())
                {
                    vitesseSpeed = 0;
                }
            }
        }
        else
        {
            //Debug.Log("Dashing");
            vitesseSpeed = moveInputD * dashSpeed;
        }
        //Debug.Log(isDashing);
        if (IsGroundedLeft() && moveInputD < 0){
            rb.linearVelocity = new Vector2(0.1f, rb.linearVelocity.y);
        }
        if (IsGroundedRight() && moveInputD > 0){
            rb.linearVelocity = new Vector2(-0.1f, rb.linearVelocity.y);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        // Obtenir la direction actuelle
        float dashDirection = Input.GetAxisRaw("Horizontal");

        if (dashDirection != 0)
        {
            // Appliquer la vitesse de dash
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
        }

        // Attendre la durée du dash
        yield return new WaitForSeconds(dashDuration);

        // Arrêter le dash
        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si l'objet avec lequel on entre en collision est un ennemi
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log(collision.gameObject.GetComponent<EnemyData>().enemyData.isDangerous);
            if (!isDashing && collision.gameObject.GetComponent<EnemyData>().enemyData.isDangerous)
            {
                GameOver();
            }
            else if (isDashing && collision.gameObject.GetComponent<EnemyData>().enemyData.isDangerous)
            {
                // Détruit l'ennemi
                timerScript.AddTime(collision.gameObject.GetComponent<EnemyData>().enemyData.time);
                collision.gameObject.GetComponent<EnemyData>().destroy();
            }
        }
        if (collision.gameObject.CompareTag("End"))
        {
            Win();
        }
         if (collision.gameObject.CompareTag("DeathZone"))
        {
                GameOver(); 
        }
    }

    public void GameOver()
    {
        // Affiche l'écran de Game Over
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        // Arrête le temps
        Time.timeScale = 0f;
    }
    public void Win()
    {
        // Affiche l'écran de Win
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        // Arrête le temps
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        // Redémarre le niveau
        Time.timeScale = 1f; // Réinitialise le temps
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        // Charge le niveau suivant
        Time.timeScale = 1f; // Réinitialise le temps
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
    private IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // Vous pouvez afficher une barre de progression ici
            Debug.Log("Progression : " + (operation.progress * 100) + "%");
            yield return null;
        }
    }
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si le joueur touche le sol
        if (collision.gameObject.CompareTag("Ground"))
        {   
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Quand le joueur quitte le sol
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }*/
    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    public bool IsGroundedEnemy()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, enemyLayer);
    }

    public bool IsGroundedEnemyLeftOrRight()
    {
        return Physics2D.OverlapCircle(leftGroundCheck.position, checkRadius, enemyLayer)||Physics2D.OverlapCircle(rightGroundCheck.position, checkRadius, enemyLayer);
    }

    public bool IsGroundedLeft()
    {
        return Physics2D.OverlapCircle(leftGroundCheck.position, checkRadius, groundLayer);
    }

    public bool IsGroundedRight()
    {
        return Physics2D.OverlapCircle(rightGroundCheck.position, checkRadius, groundLayer);
    }

    public void IsAtkLeftEnemy()
    {
        // Récupère tous les ennemis dans le rayon
        Collider2D[] enemies = Physics2D.OverlapCircleAll(atkLeft.position, checkRadius, enemyLayer);

        // Parcourt tous les ennemis détectés
        foreach (Collider2D enemy in enemies)
        {
            // Détruit chaque ennemi
            if (enemy.gameObject.GetComponent<EnemyData>().enemyData.isDangerous){
                timerScript.AddTime(enemy.gameObject.GetComponent<EnemyData>().enemyData.time);
                enemy.gameObject.GetComponent<EnemyData>().destroy();
            }     
        }
    }
    public void IsAtkRightEnemy()
    {
        // Récupère tous les ennemis dans le rayon
        Collider2D[] enemies = Physics2D.OverlapCircleAll(atkRight.position, checkRadius, enemyLayer);

        // Parcourt tous les ennemis détectés
        foreach (Collider2D enemy in enemies)
        {
            // Détruit chaque ennemi
            if (enemy.gameObject.GetComponent<EnemyData>().enemyData.isDangerous){
                timerScript.AddTime(enemy.gameObject.GetComponent<EnemyData>().enemyData.time);
                enemy.gameObject.GetComponent<EnemyData>().destroy();
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (atkRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atkRight.position, checkRadius);
        }
    }


    public bool SameSign(float a, float b)
{
    // 0 est neutre, donc a ou b étant 0 est considéré comme "même signe"
    if (a == 0 || b == 0)
        return true;

    // Vérifie si les deux nombres ont le même signe
    return (a > 0 && b > 0) || (a < 0 && b < 0);
}

}
