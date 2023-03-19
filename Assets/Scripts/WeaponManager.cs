using UnityEngine;

public class WeaponManager : MonoBehaviour {
    //~ inspector (private)
    [SerializeField][Tooltip("The shoot sfx")]
    private AudioSource shoot;
    
    [SerializeField][Tooltip("If the gun is default facing to the right")]
    private bool facingRight = true;

    //~ private
    private PlayerManager playerManager;
    private SpriteRenderer sprite;

    //~ unity methods (private)
    private void Start() {
        playerManager = GetComponentInParent<PlayerManager>();
        sprite = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate() {
        sprite.flipX = playerManager.IsFacingRight != this.facingRight;
    }

    //~ private methods
    private void Shoot() {
        playerManager.Shoot();
        shoot.pitch = Random.Range(0.95f, 1.05f);
        shoot.Play();
    }
}
