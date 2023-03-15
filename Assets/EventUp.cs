using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUp : MonoBehaviour
{
    private PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }
    void Shoot() { playerManager.Shoot(); }

}
