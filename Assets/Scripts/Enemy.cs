using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;       // Geschwindigkeit f�r Gegner
    [SerializeField]
    private bool facingRight = true;    // defaultnm facing right
    [SerializeField]
    private float moveSmooth;           // idfk
    [SerializeField]
    private int maxHealth = 100;        // Max Gesundheit Gegner
    [SerializeField]
    private GameObject itemPrefab;      // Prefab vom DropItem (Gegnerdrop)
    [SerializeField]
    private float dropChance = 0.8f;    // Wahrscheinlichkeit f�r Itemdrop
    [SerializeField]
    private float detectRange;          // Reichweite in der Gegner den Spieler finden
    [SerializeField]
    private float damage;               // Schaden für den Spieler
    [SerializeField]
    private float attackRange;          // Reichweite in der Gegner den Spieler finden

    private int currentHealth;          // Aktuelle Gesundheit Gegner
    private Vector2 smoothVelocity = Vector2.zero;       // idfk
    private Animator anim;              // Animator Gegners
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    [SerializeField]
    private ParticleSystem particle;

    [Header("Damage Anim")]
    [SerializeField][Tooltip("Time in Seconds to fade between damage color and normal")]
    private float damageEffectDecay;
    private float timer = 0;            //TimerBuffer
    private float dTimer;               //LERP interpolation of timer
    private bool alive = true;

    [HideInInspector]
    public UnityEvent OnDie;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, this.detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.attackRange);
    }
    void FixedUpdate()
    {
        float playerDistanceSqr = (GameManager.Instance.Player.transform.position - this.transform.position).sqrMagnitude;
        bool inRange = playerDistanceSqr <= (detectRange * detectRange);
        bool inAttackRange = playerDistanceSqr <= (attackRange * attackRange);
        //~ move
        Vector2 moveVec = alive && inRange
            ? (GameManager.Instance.Player.transform.position - this.transform.position).normalized * this.moveSpeed
            : Vector2.zero;
        this.rb.velocity = Vector2.SmoothDamp(
            this.rb.velocity,
            moveVec,
            ref this.smoothVelocity,
            this.moveSmooth
        );
        //~ damage player
        if(alive && inAttackRange){
            StartCoroutine(GameManager.Instance.Player.Damage(damage));
        }
        //~ flip sprite
        if (rb.velocity.x > 0) sprite.flipX = this.facingRight;
        else if (rb.velocity.x < 0) sprite.flipX = !this.facingRight;
        //~ set animator values
        anim.SetFloat("Speed", alive ? rb.velocity.magnitude : 0f);
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
        particle.Play();

        //Ist Gegner tot??
        if (currentHealth <= 0)
        {
            DeathAnimation();
        }
    }
    void DeathAnimation()
    {
        anim.Play("Die");
        alive = false;
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