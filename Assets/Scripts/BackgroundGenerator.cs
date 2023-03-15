using UnityEngine;

public class BackgroundGenerator : MonoBehaviour {
    //~ custom datatype (private)
    [System.Serializable]
    private struct SpriteSpawn{
        [Tooltip("The sprite texture")]
        public Sprite texture;

        [Range(0f, 1f)][Tooltip("The weight of this sprite for random generation")]
        public float randomWeigth;
    }

    //~ inspector (private)
    [Header("General")]
    [SerializeField][Tooltip("The random seed for the environment generation")]
    private int randomSeed;

    [SerializeField][Tooltip("The amount of backgrounds for the environment to generate")]
    private int backgroundCount;

    [SerializeField][Tooltip("The start of the ground (from the top) in the background image")]
    private float groundYstart;

    [Header("# Sprite generation")]
    [SerializeField][Tooltip("The prefab for spawning sprites")]
    private GameObject spritePrefab;

    [SerializeField][Tooltip("The different backgrounds for spawning")]
    private SpriteSpawn[] backgroundSprites;

    [SerializeField][Tooltip("The different foregrounds for spawning")]
    private SpriteSpawn[] foregroundSprites;

    //~ private
    private Vector2 backgroundSize;

    //~ unity methods (private)
    private void OnDrawGizmosSelected() {
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
    private void Start() {
        if(this.backgroundSprites.Length == 0) return;
        this.backgroundSize = (Vector2)this.backgroundSprites[0].texture.bounds.size;
        Random.State rState = Random.state;
        Random.InitState(this.randomSeed);
        //~ generate backgrounds
        Vector2 pos = Vector2.left
            * this.backgroundSize.x
            * (this.backgroundCount - 1)
            * 0.5f;
        for(int i = 0; i < this.backgroundCount; i++){
            GameObject background = Object.Instantiate<GameObject>(this.spritePrefab, this.transform);
            background.transform.position = pos;
            SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = this.GetRandomSpriteImage(this.backgroundSprites);
            spriteRenderer.sortingOrder = -10;
            pos += Vector2.right * this.backgroundSize.x;
        }
        //~ generate props
        // TODO
        Random.state = rState;
    }


    //~ private methods
    /// <summary> Get a random background texture from <see cref="spriteList"/> </summary>
    /// <param name="spriteList"> The list of different <see cref="SpriteSpawn"/> </param>
    /// <returns> A random 2D sprite from <see cref="spriteList"/> </returns>
    private Sprite GetRandomSpriteImage(in SpriteSpawn[] spriteList){
        if(spriteList.Length == 0) return null;
        for(int i = spriteList.Length - 1; i > 0; i--){
            if(Random.value <= spriteList[i].randomWeigth) return spriteList[i].texture;
        }
        return spriteList[0].texture;
    }

    /// <summary> Spawn a random foreground sprite at <paramref name="position2D"/> </summary>
    /// <param name="position2D"> The position for the sprite </param>
    private void SpawnRandomForegroundSprite(Vector2 position2D){
        GameObject sprite = Object.Instantiate<GameObject>(
            this.spritePrefab,
            (Vector3)position2D,
            Quaternion.identity,
            this.transform
        );
        SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.GetRandomSpriteImage(this.foregroundSprites);
    }

#if UNITY_EDITOR
    /// <summary> [Inspector] Generate Environment once </summary>
    [ContextMenu(">> Run once")]
    private void InspectorRunOnce(){
        this.InspectorDestroyChildElements();
        this.Start();
    }

    /// <summary> [Inspector] Destroy all child objects </summary>
    [ContextMenu(">> Remove child elements")]
    private void InspectorDestroyChildElements(){
        for(int i = this.transform.childCount - 1; i >= 0; i--)
            GameObject.DestroyImmediate(this.transform.GetChild(i).gameObject);
    }
#endif
}
