using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootinglul : MonoBehaviour {
    private GameObject player;
    private float damage = 1;
    private void FireWeapon() {
        Vector2 origin = player.transform.position; //Make this an empty at the tip of the gun preferrably :D
        Vector2 direction = player.transform.forward;
        RaycastHit hitInfo;
        float maxDistance = 500f;
        int layerMask = LayerMask.GetMask("Shootables"); //Enemies AND obstacles?
        Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask);

        if (hitInfo.collider?.gameObject is GameObject Enemy) {
            // Enemy.Damage(damage);
        }
    }
}
