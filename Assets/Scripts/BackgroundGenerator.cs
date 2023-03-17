using UnityEngine;

public class BackgroundGenerator : MonoBehaviour {
    //~ custom datatype (private)
    [System.Serializable]
    private class WeightedRandom{
        [Range(0f, 1f)][Tooltip("The weight of this sprite for random generation")]
        public float randomWeigth = 1f;

        [Tooltip("[Inspector] Only to show what the actual percentage is")]
        public float actualPercent = 0f;

        /// <summary> Sort a <see cref="WeightedRandom"/> list (ascending) </summary>
        /// <param name="weightList"> The <see cref="WeightedRandom"/> list to sort (ascending) </param>
        public static void SortWeightList(in WeightedRandom[] weightList){
            System.Array.Sort<WeightedRandom>(
                weightList,
                (WeightedRandom a, WeightedRandom b) => a.randomWeigth.CompareTo(b.randomWeigth)
            );
        }
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
    [Header("-- General --")]
    [SerializeField][Tooltip("The random seed for the environment generation")]
    private int randomSeed;

    [Header("-- Background --")]
    [SerializeField][Tooltip("The start of the ground (from the top) in the background image")]
    private float groundYstart;

    [SerializeField][Tooltip("The amount of backgrounds for the environment to generate")]
    private int backgroundCount;

    [SerializeField][Tooltip("The prefab for spawning background sprites")]
    private GameObject backgroundSpritePrefab;

    [SerializeField][Tooltip("The different backgrounds for spawning")]
    [ContextMenuItem(">> Sort background sprite list", "InspectorSortBackgroundSprites")]
    private BackgroundSpriteSpawn[] backgroundSprites;

    [Header("-- Foreground --")]
    [SerializeField][Tooltip("The amount of foreground props to generate")]
    private int foregroundCount;

    [SerializeField][Tooltip("The different foregrounds for spawning")]
    [ContextMenuItem(">> Sort foreground sprite list", "InspectorSortForegroundSprites")]
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
        //~ save random state
        Random.State rState = Random.state;
        Random.InitState(this.randomSeed);
        //~ generate backgrounds
        GameObject emptyParent = new GameObject("Background");
        emptyParent.transform.SetParent(this.transform);
        emptyParent.transform.localPosition = Vector3.zero;
        WeightedRandom.SortWeightList(this.backgroundSprites);
        Vector2 pos = Vector2.left
            * this.backgroundSize.x
            * (this.backgroundCount - 1)
            * 0.5f;
        for(int i = 0; i < this.backgroundCount; i++){
            //~ spawn (empty) background sprite prefab
            GameObject background = Object.Instantiate<GameObject>(this.backgroundSpritePrefab, emptyParent.transform);
            background.transform.position = pos;
            SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
            //~ get random texture and material
            if(this.backgroundSprites.Length > 0){
                //~ get random index
                float rValue = Random.value;
                int index = System.Array.FindIndex<WeightedRandom>(
                    this.backgroundSprites,
                    backgroundSprite => rValue <= backgroundSprite.randomWeigth
                );
                if(index == -1) index = this.backgroundSprites.Length - 1;
                //~ set texture and material
                spriteRenderer.sprite = this.backgroundSprites[index].texture;
                spriteRenderer.sharedMaterial = this.backgroundSprites[index].sharedMaterial;
            }
            spriteRenderer.sortingOrder = -10;
            pos += Vector2.right * this.backgroundSize.x;
        }
        //~ calculate spawn area and generate colliders
        this.spawnAreaMin = (Vector2)this.transform.position
            + Vector2.down * (this.backgroundSize.y * 0.5f)
            + Vector2.left * (this.backgroundSize.x * (float)(this.backgroundCount - 1) * 0.5f - 0.5f);
        this.spawnAreaMax = (Vector2)this.transform.position
            + Vector2.up * this.groundYstart
            + Vector2.right * (this.backgroundSize.x * (float)(this.backgroundCount - 1) * 0.5f - 0.5f);
        //~ >> top border
        BoxCollider2D box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = Vector2.up * (this.spawnAreaMax.y + 0.5f);
        box.size = new Vector2(this.backgroundSize.x * (float)(this.backgroundCount - 1) * 1.1f, 1f);
        //~ >> bottom border
        box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = Vector2.up * (this.spawnAreaMin.y - 0.5f);
        box.size = new Vector2(this.backgroundSize.x * (float)(this.backgroundCount - 1) * 1.1f, 1f);
        //~ >> left border
        box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = Vector2.right * (this.spawnAreaMin.x - 0.5f);
        box.size = new Vector2(1f, this.backgroundSize.y * 1.1f);
        //~ >> right border
        box = emptyParent.AddComponent<BoxCollider2D>();
        box.offset = Vector2.right * (this.spawnAreaMax.x + 0.5f);
        box.size = new Vector2(1f, this.backgroundSize.y * 1.1f);
        //~ generate foreground props
        if(this.foregroundSprites.Length > 0){
            emptyParent = new GameObject("Foreground");
            emptyParent.transform.SetParent(this.transform);
            emptyParent.transform.localPosition = Vector3.zero;
            WeightedRandom.SortWeightList(this.foregroundSprites);
            for(int i = 0; i < this.foregroundCount; i++){
                pos = new Vector2(
                    Random.Range(this.spawnAreaMin.x, this.spawnAreaMax.x),
                    Random.Range(this.spawnAreaMin.y, this.spawnAreaMax.y)
                );
                //~ get random index
                float rValue = Random.value;
                int index = System.Array.FindIndex<WeightedRandom>(
                    this.foregroundSprites,
                    foregroundSprite => rValue <= foregroundSprite.randomWeigth
                );
                if(index == -1) index = this.foregroundSprites.Length - 1;
                //~ spawn random prefab
                GameObject sprite = Object.Instantiate<GameObject>(this.foregroundSprites[index].prefab, emptyParent.transform);
                sprite.transform.position = pos;
            }
        }
        //~ restore random state
        Random.state = rState;
        //~ start spawning enemies
        this.GetComponent<EnemySpawner>()?.StartSpawning(this.spawnAreaMin, this.spawnAreaMax);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        //~ calculate the actual percentage (backgroundSprites)
        float largestWeight = 0f;
        for(int i = 0; i < this.backgroundSprites.Length; i++){
            this.backgroundSprites[i].actualPercent = Mathf.Clamp01(this.backgroundSprites[i].randomWeigth - largestWeight) * 100f;
            if(this.backgroundSprites[i].randomWeigth > largestWeight) largestWeight = this.backgroundSprites[i].randomWeigth;
        }
        //~ calculate the actual percentage (foregroundSprites)
        largestWeight = 0f;
        for(int i = 0; i < this.foregroundSprites.Length; i++){
            this.foregroundSprites[i].actualPercent = Mathf.Clamp01(this.foregroundSprites[i].randomWeigth - largestWeight) * 100f;
            if(this.foregroundSprites[i].randomWeigth > largestWeight) largestWeight = this.foregroundSprites[i].randomWeigth;
        }
    }

    /// <summary> [Inspector] Sort the <see cref="backgroundSprites"/> Array </summary>
    private void InspectorSortBackgroundSprites() => WeightedRandom.SortWeightList(this.backgroundSprites);

    /// <summary> [Inspector] Sort the <see cref="foregroundSprites"/> Array </summary>
    private void InspectorSortForegroundSprites() => WeightedRandom.SortWeightList(this.foregroundSprites);

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
