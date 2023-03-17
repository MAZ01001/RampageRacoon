using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;       // Geschwindigkeit f�r Gegner
    [SerializeField]
    private float moveSmooth;         // idfk
    [SerializeField]
    private int maxHealth = 100;        // Max Gesundheit Gegner
    [SerializeField]
    private int currentHealth;          // Aktuelle Gesundheit Gegner
    [SerializeField]
    private GameObject itemPrefab;      // Prefab vom DropItem (Gegnerdrop)
    [SerializeField]
    private float dropChance = 0.8f;    // Wahrscheinlichkeit f�r Itemdrop
    [SerializeField]
    private float detectRange;          // Reichweite in der Gegner den Spieler finden

    private Vector2 smoothVelocity = Vector2.zero;       // idfk
    private Animator anim;              // Animator Gegners
    private GameObject player;          // Player
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    [Header("Damage Anim")]
    [SerializeField][Tooltip("Time in Seconds to fade between damage color and normal")]
    private float damageEffectDecay;
    private float timer = 0;            //TimerBuffer
    private float dTimer;               //LERP interpolation of timer

    [HideInInspector]
    public UnityEvent OnDie;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameManager.Instance.Player.gameObject;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
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
        //Damage Animation
        timer -= Time.fixedDeltaTime;
        dTimer = Mathf.Lerp(1, 0, timer / damageEffectDecay);
        sprite.color = new Color(1, dTimer, dTimer);
    }

    public void Damage(int damage)
    {
        Debug.Log(damage);
        currentHealth -= damage;
        Debug.Log("Health after Attack: " + currentHealth);
        timer = damageEffectDecay;  //Init DamageAnimTimer

        //Ist Gegner tot??
        if (currentHealth <= 0)
        {
            Die();
        }
    }

 
    void Die()
    {
        
        // Zuf�llige Chance f�r Itemdrop
        if (Random.value < dropChance)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        //vergebe Punkte - Referenz auf ScoreManager Skript
        ScoreManager.instance.AddScore(10);

        this.OnDie.Invoke();

        //Gegner Sprite Bums zerst�ren
        Destroy(gameObject);
    }
}