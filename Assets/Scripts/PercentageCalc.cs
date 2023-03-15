using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentageCalc : MonoBehaviour
{
    //~ custom datatype (private)
    [System.Serializable]
    private struct SpriteSpawn
    {
        [Tooltip("The sprite texture")]
        public Sprite texture;

        [Range(0f, 1f)]
        [Tooltip("The weight of this sprite for random generation")]
        public float randomWeigth;
    }

    //~ inspector (private)
    [Header("General")]
    [SerializeField]
    [Tooltip("The random seed for the environment generation")]
    private int randomSeed;

    [SerializeField]
    [Tooltip("The amount of backgrounds for the environment to generate")]
    private int backgroundCount;

    [SerializeField]
    [Tooltip("The start of the ground (from the top) in the background image")]
    private float groundYstart;

    [Header("# Sprite generation")]
    [SerializeField]
    [Tooltip("The prefab for spawning sprites")]
    private GameObject spritePrefab;

    [SerializeField]
    [Tooltip("The different backgrounds for spawning")]
    private SpriteSpawn[] backgroundSprites;

    [SerializeField]
    [Tooltip("The different foregrounds for spawning")]
    private SpriteSpawn[] foregroundSprites;

    //~ private
    private Vector2 backgroundSize;
    private float[] backgroundSpriteWeightRangeStart;
    private float[] backgroundSpriteWeightRangeEnd;

    //~ unity methods (private)
    private void OnDrawGizmosSelected()
    {
        //~ draw backgrounds area
        Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
        Vector3 backgroundSize = this.backgroundSprites[0].texture.bounds.size;
        Gizmos.DrawCube(
            this.transform.position,
            new Vector3(
                backgroundSize.x * this.backgroundCount,
                backgroundSize.y,
                1f
            )
        );
        //~ draw start of ground in image
        Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
        Gizmos.DrawLine(
            this.transform.position + Vector3.left + Vector3.up * this.groundYstart,
            this.transform.position + Vector3.right + Vector3.up * this.groundYstart
        );
    }

    private void Start()
    {
        backgroundSpriteWeightRangeStart = new float[backgroundSprites.Length];
        backgroundSpriteWeightRangeEnd = new float[backgroundSprites.Length];
        PercentageCalculation();
    }
    private void PercentageCalculation()
    {
        float totalWeight = 0;
        foreach (SpriteSpawn backgroundSprite in backgroundSprites)
        {
            totalWeight += backgroundSprite.randomWeigth;
            Debug.Log (totalWeight);
        }
        if (totalWeight > 1)
        {
            Debug.LogException(new System.Exception("randomWeigth Total over 100%"));
        }
        else
        {
            int index = 0;
            float lastMax = 0f;
            foreach (SpriteSpawn backgroundSprite in backgroundSprites)
            {
                Debug.Log(index);
                backgroundSpriteWeightRangeStart[index] = lastMax;
                backgroundSpriteWeightRangeEnd[index] = lastMax + backgroundSprite.randomWeigth;
                lastMax = backgroundSprite.randomWeigth;
                Debug.Log(backgroundSpriteWeightRangeStart[index] + " ; " + backgroundSpriteWeightRangeEnd[index]);
                index += 1;
            }
        }
    }
}
