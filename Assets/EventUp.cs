using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUp : MonoBehaviour
{
    private PlayerManager playerManager;
    [SerializeField]
    private AudioSource shoot;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();

    }
    void Shoot() { playerManager.Shoot();shoot.pitch = Random.Range(0.95f,1.05f); shoot.Play(); }

}
