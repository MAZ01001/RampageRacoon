using UnityEngine;

public class BackgroundGenerator : MonoBehaviour {
    //~ custom datatype (private)
    [System.Serializable]
    private struct SpriteSpawn{
        [Tooltip("The sprite texture")]
        public Sprite texture;

        [Tooltip("The sprite material for blending")]
        public Material sharedMaterial;

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
            this.transform.position + Vector3.left  * backgroundSize.x + Vector3.up * this.groundYstart,
            this.transform.position + Vector3.right * backgroundSize.x + Vector3.up * this.groundYstart
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
            this.SetSpriteRandomImgMaterial(ref spriteRenderer, this.backgroundSprites);
            spriteRenderer.sortingOrder = -10;
            pos += Vector2.right * this.backgroundSize.x;
        }
        //~ generate colliders
        //~ >> top border
        BoxCollider2D box = this.gameObject.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.up * (this.groundYstart + 0.5f);
        box.size = new Vector2(this.backgroundSize.x * (float)(this.backgroundCount - 1) * 1.1f, 1f);
        //~ >> bottom border
        box = this.gameObject.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.down * (this.backgroundSize.y * 0.5f + 0.5f);
        box.size = new Vector2(this.backgroundSize.x * (float)(this.backgroundCount - 1) * 1.1f, 1f);
        //~ >> left border
        box = this.gameObject.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.left * this.backgroundSize.x * (float)(this.backgroundCount - 1) * 0.5f;
        box.size = new Vector2(1f, this.backgroundSize.y * 1.1f);
        //~ >> right border
        box = this.gameObject.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.right * this.backgroundSize.x * (float)(this.backgroundCount - 1) * 0.5f;
        box.size = new Vector2(1f, this.backgroundSize.y * 1.1f);
        //~ generate props
        // TODO
        Random.state = rState;
    }


    //~ private methods
    /// <summary> Set the given <paramref name="spriteRenderer"/> a random background texture from <see cref="spriteList"/> and the corresponding material </summary>
    /// <param name="spriteRenderer"> The sprite to set <see cref="Sprite"/> and <see cref="Material"/> </param>
    /// <param name="spriteList"> The list of different <see cref="SpriteSpawn"/> </param>
    private void SetSpriteRandomImgMaterial(ref SpriteRenderer spriteRenderer, in SpriteSpawn[] spriteList){
        if(spriteList.Length == 0) return;
        int index = 0;
        float rValue = Random.value;
        for(int i = spriteList.Length - 1; i > 0; i--){
            if(rValue <= spriteList[i].randomWeigth){
                index = i;
                break;
            }
        }
        spriteRenderer.sprite = spriteList[index].texture;
        spriteRenderer.sharedMaterial = spriteList[index].sharedMaterial;
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
        this.SetSpriteRandomImgMaterial(ref spriteRenderer, this.foregroundSprites);
    }

#if UNITY_EDITOR
    /// <summary> [Inspector] Generate Environment once </summary>
    [ContextMenu(">> Run once")]
    private void InspectorRunOnce(){
        this.InspectorDestroy2DBoxColliders();
        this.InspectorDestroyChildElements();
        this.Start();
    }

    /// <summary> [Inspector] Destroy all child objects </summary>
    [ContextMenu(">> Remove child elements")]
    private void InspectorDestroyChildElements(){
        for(int i = this.transform.childCount - 1; i >= 0; i--)
            GameObject.DestroyImmediate(this.transform.GetChild(i).gameObject);
    }

    /// <summary> [Inspector] Destroy all 2D box collider components </summary>
    [ContextMenu(">> Remove 2D box colliders")]
    private void InspectorDestroy2DBoxColliders(){
        foreach(Component boxCollider2DComponent in this.gameObject.GetComponents<BoxCollider2D>())
            GameObject.DestroyImmediate(boxCollider2DComponent);
    }
#endif
}
