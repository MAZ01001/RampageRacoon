using UnityEngine;

public class BackgroundGenerator : MonoBehaviour {
    //~ custom datatype (private)
    [System.Serializable]
    private class WeightedRandom{
        [Range(0f, 1f)][Tooltip("The weight of this sprite for random generation")]
        public float randomWeigth;
    }
    [System.Serializable]
    private class BackgroundSpriteSpawn : WeightedRandom{
        [Tooltip("The sprite texture")]
        public Sprite texture;

        [Tooltip("The sprite material for blending")]
        public Material sharedMaterial;
    }
    [System.Serializable]
    private class ForegroundSpriteSpawn : WeightedRandom{
        [Tooltip("The sprite prefab")]
        public GameObject prefab;
    }

    //~ inspector (private)
    [Header("General")]
    [SerializeField][Tooltip("The random seed for the environment generation")]
    private int randomSeed;

    [SerializeField][Tooltip("The amount of backgrounds for the environment to generate")]
    private int backgroundCount;

    [SerializeField][Tooltip("The start of the ground (from the top) in the background image")]
    private float groundYstart;

    [Header("Background")]
    [SerializeField][Tooltip("The prefab for spawning background sprites")]
    private GameObject backgroundSpritePrefab;

    [SerializeField][Tooltip("The different backgrounds for spawning")]
    private BackgroundSpriteSpawn[] backgroundSprites;

    [SerializeField][Tooltip("The different foregrounds for spawning")]
    private ForegroundSpriteSpawn[] foregroundSprites;

    //~ private
    private Vector2 backgroundSize = Vector2.zero;
    private Vector2 spawnAreaMin = Vector2.zero;
    private Vector2 spawnAreaMax = Vector2.zero;

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
        GameObject emptyParent = new GameObject("Background");
        emptyParent.transform.SetParent(this.transform);
        emptyParent.transform.localPosition = Vector3.zero;
        Vector2 pos = Vector2.left
            * this.backgroundSize.x
            * (this.backgroundCount - 1)
            * 0.5f;
        for(int i = 0; i < this.backgroundCount; i++){
            GameObject background = Object.Instantiate<GameObject>(this.backgroundSpritePrefab, emptyParent.transform);
            background.transform.position = pos;
            SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
            //~ get random texture and material
            if(this.backgroundSprites.Length > 0){
                int index = this.GetRandomIndexWeighted(this.backgroundSprites);
                spriteRenderer.sprite = this.backgroundSprites[index].texture;
                spriteRenderer.sharedMaterial = this.backgroundSprites[index].sharedMaterial;
            }
            spriteRenderer.sortingOrder = -10;
            pos += Vector2.right * this.backgroundSize.x;
        }
        //~ generate colliders
        // TODO create bounds for spawning area (vec2 min, vec2 max)
        // this.spawnAreaMin;
        // this.spawnAreaMax;
        //~ >> top border
        BoxCollider2D box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.up * (this.groundYstart + 0.5f);
        box.size = new Vector2(this.backgroundSize.x * (float)(this.backgroundCount - 1) * 1.1f, 1f);
        //~ >> bottom border
        box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.down * (this.backgroundSize.y * 0.5f + 0.5f);
        box.size = new Vector2(this.backgroundSize.x * (float)(this.backgroundCount - 1) * 1.1f, 1f);
        //~ >> left border
        box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.left * this.backgroundSize.x * (float)(this.backgroundCount - 1) * 0.5f;
        box.size = new Vector2(1f, this.backgroundSize.y * 1.1f);
        //~ >> right border
        box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = (Vector2)this.transform.position + Vector2.right * this.backgroundSize.x * (float)(this.backgroundCount - 1) * 0.5f;
        box.size = new Vector2(1f, this.backgroundSize.y * 1.1f);
        //~ generate props
        if(this.foregroundSprites.Length > 0){
            emptyParent = new GameObject("Foreground");
            emptyParent.transform.SetParent(this.transform);
            emptyParent.transform.localPosition = Vector3.zero;
            pos = Vector2.zero;
            // TODO loop
            //~ spawn random prefab
            int index = this.GetRandomIndexWeighted(this.foregroundSprites);
            GameObject sprite = Object.Instantiate<GameObject>(this.foregroundSprites[index].prefab, emptyParent.transform);
            sprite.transform.position = pos;
            // TODO
        }
        Random.state = rState;
    }


    //~ private methods
    /// <summary> Gives an index from the given <see cref="weightedList"/> </summary>
    /// <param name="weightedList"> The list of different <see cref="WeightedRandom"/> </param>
    /// <returns> A random index in the <see cref="weightedList"/> </returns>
    private int GetRandomIndexWeighted(in WeightedRandom[] weightedList){
        // TODO replace with new random system
        int index = 0;
        float rValue = Random.value;
        for(int i = weightedList.Length - 1; i > 0; i--){
            if(rValue <= weightedList[i].randomWeigth){
                index = i;
                break;
            }
        }
        return index;
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
