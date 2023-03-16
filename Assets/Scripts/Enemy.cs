using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;       // Geschwindigkeit für Gegner
    [SerializeField]
    private float moveSmooth;         // idfk
    [SerializeField]
    private int maxHealth = 100;        // Max Gesundheit Gegner
    [SerializeField]
    private int currentHealth;          // Aktuelle Gesundheit Gegner
    [SerializeField]
    private GameObject itemPrefab;      // Prefab vom DropItem (Gegnerdrop)
    [SerializeField]
    private float dropChance = 0.8f;    // Wahrscheinlichkeit für Itemdrop
    [SerializeField]
    private float detectRange;          // Reichweite in der Gegner den Spieler finden

    private Vector2 smoothVelocity = Vector2.zero;       // idfk
    private Animator anim;              // Animator Gegners
    private GameObject player;          // Player
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameManager.Instance.Player.gameObject;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        bool inRange = (player.transform.position - this.transform.position).sqrMagnitude <= detectRange * detectRange;
        if (inRange == true)
        {
            Vector2 moveDir = (player.transform.position - this.transform.position).normalized;
            Vector2 moveDirSpeed = moveDir * this.moveSpeed;
            this.rb.velocity = Vector2.SmoothDamp(
                this.rb.velocity,
                moveDirSpeed,
                ref this.smoothVelocity,
                this.moveSmooth
                );
        }
        else if (inRange == false) { this.rb.velocity = Vector2.zero; }
        anim.SetFloat("Speed", rb.velocity.magnitude);
        if (rb.velocity.x > 0)
        {
            sprite.flipX = true;
        }
        else if (rb.velocity.x < 0)
        {
            sprite.flipX = false;
        }
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