using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    //~ custom datatypes (private)
    private class Matrix2x2 {
        /// <summary>
        ///     For referencing a location in the matrix:
        ///     <br/> [ xx, yx ]
        ///     <br/> [ xy, yy ]
        /// </summary>
        public enum Point : int {
            xx = 0, yx = 1,
            xy = 2, yy = 3
        }
        private float[] points;
        /// <summary> Creates a 2x2 float matrix (filled with 0s) </summary>
        public Matrix2x2(){ this.points = new float[4]{0f, 0f, 0f, 0f}; }
        /// <summary>
        ///     Creates a 2x2 float matrix from given points
        /// </summary>
        public Matrix2x2(float xx, float yx, float xy, float yy){ this.points = new float[4]{xx, yx, xy, yy}; }
        /// <summary>
        ///     Creates a 2x2 identity (float) matrix
        ///     <br/> [ 1, 0 ]
        ///     <br/> [ 0, 1 ]
        /// </summary>
        public static Matrix2x2 Identity => new Matrix2x2(1f, 0f, 0f, 1f);
        /// <summary> Set a <paramref name="value"/> at the given location (<paramref name="point"/>) </summary>
        /// <param name="point"> The location in the matrix (first x/y for column, second x/y for row) </param>
        /// <param name="value"> The new value </param>
        public void SetPoint(Point point, float value) => this.points[(int)point] = value;
        /// <summary> Transform the given <paramref name="vector"/> with this matrix and returns the result </summary>
        /// <param name="vector"> The given 2D vector for transformation </param>
        /// <returns> The resulting 2D vector </returns>
        public Vector2 TransformVec2(Vector2 vector) => new Vector2(
            vector.x * this.points[(int)Point.xx] + vector.y * this.points[(int)Point.yx],
            vector.x * this.points[(int)Point.xy] + vector.y * this.points[(int)Point.yy]
        );
    }

    //~ inspector (private)
    [SerializeField][Tooltip("If the player is default facing to the right")]
    private bool facingRight = true;

    [Header("Damage Anim")]
    [SerializeField][Tooltip("Time in Seconds to fade between damage color and normal")]
    private float damageEffectDecay;
    [SerializeField][Tooltip("The particle to show when the player takes damage")]
    private ParticleSystem particle;

    [Header("Movement")]

    //! removed from inspector
    [HideInInspector][Range(-80f, 80f)][Tooltip("The angle of the shear effect to the Y axis for movement")]
    private float yAxisShear = 0f;

    [SerializeField][Min(0f)][Tooltip("The movement speed of the player")]
    private float moveSpeed = 3f;

    [SerializeField][Min(0f)][Tooltip("The smoothing factor of the player movement")]
    private float moveSmooth = 0.1f;

    [Header("Animation")]

    [SerializeField]
    [Tooltip("Animator of the Weapon Sprite")]
    private Animator weaponAnim;

    [Header("Attacking")]
    [SerializeField]
    [Tooltip("Damage Done per Shot")]
    private int damage;
    [SerializeField]
    [Tooltip("Max shoot distance")]
    private float shootMaxDistance;
    //~ private
    private Animator anim;
    private InputManager inputManager;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Matrix2x2 shearMatrix = Matrix2x2.Identity;
    private Vector2 smoothVelocity = Vector3.zero;
    private Enemy enemyScript;
    private bool hurt = false;

    //~ public
    public bool IsFacingRight => this.spriteRenderer.flipX != this.facingRight;

    //~ unity methods (private)
    private void OnDrawGizmosSelected() {
        //~ draw walking axis of player
        this.shearMatrix.SetPoint(Matrix2x2.Point.yx, Mathf.Tan(this.yAxisShear * Mathf.Deg2Rad));
        this.shearMatrix.SetPoint(Matrix2x2.Point.yy, Mathf.Cos(this.yAxisShear * Mathf.Deg2Rad));
        //~ Y axis
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position, (Vector3)this.shearMatrix.TransformVec2(Vector2.up));
        //~ X axis
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, (Vector3)this.shearMatrix.TransformVec2(Vector2.right));
        //~ shoot range
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(this.transform.position, this.transform.right * this.shootMaxDistance);
    }
    private void Start() {
        //~ get components
        this.inputManager = this.GetComponent<InputManager>();
        this.rb = this.GetComponent<Rigidbody2D>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.anim = this.GetComponent<Animator>();
        //~ create shear matrix
        this.shearMatrix.SetPoint(Matrix2x2.Point.yx, Mathf.Tan(this.yAxisShear * Mathf.Deg2Rad));
        this.shearMatrix.SetPoint(Matrix2x2.Point.yy, Mathf.Cos(this.yAxisShear * Mathf.Deg2Rad));
    }
    private void FixedUpdate() {
        //~ move
        bool isMoving = this.inputManager.move.sqrMagnitude >= 0.01f;
        anim.SetBool("Moving", isMoving);
        Vector2 moveDir = isMoving
            ? this.shearMatrix.TransformVec2(this.inputManager.move)
            : Vector2.zero;
        Vector2 moveDirSpeed = moveDir * this.moveSpeed;
        this.rb.velocity = Vector2.SmoothDamp(
            this.rb.velocity,
            moveDirSpeed,
            ref this.smoothVelocity,
            this.moveSmooth
        );
        //~ player facing direction (only change while moving)
        if(Input.mousePosition.x > Screen.width/2) this.spriteRenderer.flipX = !this.facingRight;
        else if(Input.mousePosition.x < Screen.width/2) this.spriteRenderer.flipX = this.facingRight;
        //~ update animator values
        weaponAnim.SetBool("Firing", this.inputManager.shoot);
    }

    /// <summary> Fire the Weapon </summary>
    public void Shoot() {
        Debug.Log("Bang");
        Vector2 origin = this.transform.position; //Make this an empty at the tip of the gun preferrably :D
        Vector2 direction = this.transform.right;
        if (this.IsFacingRight) direction = this.transform.right;
        else if (!this.IsFacingRight) direction = this.transform.right*-1;
        int layerMask = LayerMask.GetMask("Shootables"); //Enemies AND obstacles?
        RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, this.shootMaxDistance, layerMask);
        if (hitInfo.collider?.gameObject is GameObject enemy){
            Debug.Log("hit");
            enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.Damage(damage);
        }
    }
    /// <summary> [Coroutine] damages the player </summary>
    /// <param name="damage"> the amount of damage </param>
    public IEnumerator Damage(float damage){
        if(!this.hurt){
            this.hurt = true;
            DrugEffectManager.Instance.IncreaseTimer(-damage);
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            float timer = this.damageEffectDecay;
            float dTimer = 0f;
            particle.Play();
            while(timer > 0f){
                timer -= Time.deltaTime;
                dTimer = Mathf.Lerp(1, 0, timer / this.damageEffectDecay);
                spriteRenderer.color = new Color(1, dTimer, dTimer);
                yield return null;
            }
            this.hurt = false;
        }
    }
}
