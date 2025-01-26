using UnityEngine;
using System.Collections;
using System;

public class EnemyData : MonoBehaviour
{
    public ennemy_data enemyData; // ScriptableObject contenant les données de l'ennemi
    public Animator _animator;
    void Start()
    {
        enemyData.isDangerous = true;
    }
    void Update()
    {
        Debug.Log(enemyData.isDangerous);
    }

    public void destroy()
    {
        //enemyData.isDangerous = false;
        _animator.SetBool("isDeath", true);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        StartCoroutine(DestroyAfterDelay());
    }
     private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
            //enemyData.isDangerous = true;
        }
}
