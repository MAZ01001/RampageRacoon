using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    //~ insector (private)
    [SerializeField][Tooltip("All enemy prefabs for spawning")]
    private GameObject[] enemyPrefabs;

    [SerializeField][Min(0f)][Tooltip("Time interval for spawning (delay in seconds)")]
    private float spawnTimeInteval = 5f;

    [SerializeField][Min(0)][Tooltip("The maximum amount of enemies")]
    private int maxEnemyCount = 100;

    [SerializeField][Min(0f)][Tooltip("The width of the camera view in scene (not spawnable area)")]
    private float cameraWidth = 27f;

    //~ private
    private Coroutine spawnRoutine;
    private GameObject emptyParent = null;
    private Vector2 spawnAreaPos = Vector2.zero;
    private Vector2 spawnAreaExtends = Vector2.zero;
    private float spriteMaxWidth = 0f;
    private int enemyCounter = 0;

    //~ unity methods (private)
    private void OnDrawGizmosSelected() {
        if(this.spriteMaxWidth == 0f) GetMaxSpriteWidth();
        //~ draw camera size
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)Camera.main.transform.position, new Vector3(this.cameraWidth, 2f, 1f));
        //~ draw calculated max sprite width
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(this.transform.position, new Vector3(this.spriteMaxWidth, 1f, 1f));
    }
    private void Awake() {
        this.enemyCounter = 0;
        this.GetMaxSpriteWidth();
    }

    //~ private methods
    /// <summary> Calculates the max width of all <see cref="enemyPrefabs"/> in world space </summary>
    private void GetMaxSpriteWidth(){
        this.spriteMaxWidth = 0f;
        foreach(GameObject sprite in this.enemyPrefabs){
            SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
            if(spriteRenderer.bounds.size.x > this.spriteMaxWidth) this.spriteMaxWidth = spriteRenderer.bounds.size.x;
        }
    }
    /// <summary> [Coroutine] Spawning Enemies indefinitely </summary>
    private IEnumerator EnemySpawning(){
        Vector2 pos = Vector2.zero;
        float cameraXLeft, cameraXRight;
        bool spawnLeft, spawnRight;
        Enemy enemy;
        //~ make empty parent
        if(this.emptyParent is null){
            this.emptyParent = new GameObject("Enemies");
            this.emptyParent.transform.SetParent(this.transform);
        }
        while(true){
            yield return new WaitForSeconds(this.spawnTimeInteval);
            yield return new WaitUntil(() => this.enemyCounter < this.maxEnemyCount);
            //~ set random position (outside camera view)
            cameraXRight = Camera.main.transform.position.x + this.cameraWidth * 0.5f;
            spawnLeft = Mathf.Abs((this.spawnAreaPos.x + this.spawnAreaExtends.x) - cameraXRight) > this.spriteMaxWidth;
            cameraXLeft = Camera.main.transform.position.x - this.cameraWidth * 0.5f;
            spawnRight = Mathf.Abs((this.spawnAreaPos.x - this.spawnAreaExtends.x) - cameraXLeft) > this.spriteMaxWidth;
            if(spawnLeft){
                if(spawnRight) //~ random left or right
                    pos = new Vector2(
                        Random.value > 0.5f
                            ? Random.Range(cameraXRight, this.spawnAreaPos.x + this.spawnAreaExtends.x)
                            : Random.Range(this.spawnAreaPos.x - this.spawnAreaExtends.x, cameraXLeft),
                        this.spawnAreaPos.y + Random.Range(-this.spawnAreaExtends.y, this.spawnAreaExtends.y)
                    );
                else //~ left
                    pos = new Vector2(
                        Random.Range(this.spawnAreaPos.x - this.spawnAreaExtends.x, cameraXLeft),
                        this.spawnAreaPos.y + Random.Range(-this.spawnAreaExtends.y, this.spawnAreaExtends.y)
                    );
            }else if(spawnRight) //~ right
                pos = new Vector2(
                    Random.Range(cameraXRight, this.spawnAreaPos.x + this.spawnAreaExtends.x),
                    this.spawnAreaPos.y + Random.Range(-this.spawnAreaExtends.y, this.spawnAreaExtends.y)
                );
            else continue;
            enemy = Object.Instantiate<GameObject>(
                this.enemyPrefabs[Random.Range(0, this.enemyPrefabs.Length)],
                pos,
                Quaternion.identity,
                this.emptyParent.transform
            ).GetComponent<Enemy>();
            enemy.OnDie.AddListener(() => this.enemyCounter--);
            this.enemyCounter++;
        }
    }

    //~ public methods
    /// <summary> Start spawning </summary>
    /// <param name="spawnAreaPos"> world position of spawn area </param>
    /// <param name="spawnAreaExtends"> world space extends of spawn area (extend is half the size) </param>
    public void StartSpawning(in Vector2 spawnAreaPos, in Vector2 spawnAreaExtends){
        this.spawnAreaPos = spawnAreaPos;
        this.spawnAreaExtends = spawnAreaExtends;
        this.StopSpawning();
        this.spawnRoutine = StartCoroutine(this.EnemySpawning());
    }
    /// <summary> Stops spawning </summary>
    public void StopSpawning(){
        if(this.spawnRoutine is not null) StopCoroutine(this.spawnRoutine);
        this.spawnRoutine = null;
    }
    /// <summary> Set the spawn rate (delay in seconds) </summary>
    /// <param name="newSpawnRate"> The new spawn rate (delay in seconds) </param>
    public void SetSpawnRate(float newSpawnRate){
        if(newSpawnRate < 0f) newSpawnRate = 0f;
        this.spawnTimeInteval = newSpawnRate;
    }
}
