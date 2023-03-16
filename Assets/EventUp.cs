using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUp : MonoBehaviour
{
    private PlayerManager playerManager;
    private SpriteRenderer sprite;
    private bool shouldFlip;
    [SerializeField]
    private AudioSource shoot;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        shouldFlip = playerManager.RotateGun();
        if (shouldFlip == false)
        {
            sprite.flipX = true;
            this.transform.rotation = Quaternion.Euler(0,0,42);
        }
        if (shouldFlip == true)
        {
            sprite.flipX = false;
            this.transform.rotation = Quaternion.Euler(0, 0, -42);
        }
    }
    void Shoot() { playerManager.Shoot();shoot.pitch = Random.Range(0.95f,1.05f); shoot.Play(); }


}
