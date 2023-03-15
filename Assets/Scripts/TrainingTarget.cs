using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTarget : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Damage(float damage)
    {
        anim.Play("Damage", 0, 0f);
    }
}
