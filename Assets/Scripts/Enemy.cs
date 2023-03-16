using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;        // Geschwindigkeit für Gegner
    public int maxHealth = 100;         // Max Gesundheit Gegner
    public int currentHealth;           // Aktuelle Gesundheit Gegner
    public GameObject itemPrefab;       // Prefab vom DropItem (Gegnerdrop)
    public float dropChance = 0.8f;     // Wahrscheinlichkeit für Itemdrop

    private Animator anim;              // Animator Gegners

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Bewegung alter
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.Play("Damage", 0, 0f);

        //Ist Gegner tot??
        if (currentHealth <= 0)
        {
            Die();
        }
    }

 
    void Die()
    {
        // Zufällige Chance für Itemdrop
        if (Random.value < dropChance)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        //vergebe Punkte - Referenz auf ScoreManager Skript
        ScoreManager.instance.AddScore(10);

        //Gegner Sprite Bums zerstören
        Destroy(gameObject);
    }
}