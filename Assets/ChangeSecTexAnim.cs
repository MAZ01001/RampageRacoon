using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSecTexAnim : MonoBehaviour
{
    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private Material material;
    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void CycleTex()
    {
        material.SetTexture("_SecTex", sprites[index].texture);
        index = (index + 1) % sprites.Length;
    }
    void DefaultTex()
    {
        material.SetTexture("_SecTex", defaultSprite.texture);
    }
}
