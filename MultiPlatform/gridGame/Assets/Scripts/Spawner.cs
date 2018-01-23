using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public event System.Action<int> OnNewWave;
    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;

    private LivingEntity playerEntity;
    private Transform playerT;

    private Wave currentWave;
    private int currentWaveNumber;

    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;

    private MapGenerator map;

    private float timeBetweenCampingChecks = 2;
    private float campThreshholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private bool isCamping;

    private bool isDis;
    
    [System.Serializable]
    public class Wave{
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;
        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColour;
    }

	void Start () {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.deltaTime;
        campPositionOld = playerT.position;
        playerEntity.OnDeadth += OnPlayerDead;
        
        map = FindObjectOfType<MapGenerator>();
        NextWave();
	}

    void Update(){
        if (!isDis){
            if (Time.time > nextCampCheckTime){
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThreshholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime){
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }
        if (devMode){
            if (Input.GetKeyDown(KeyCode.Return)){
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>()){
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy(){
        float spawnDelay = 1;
        float tileFlashspeed = 4;
        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping){
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = Color.white;
        Color flashColour = Color.red;
        float spawnTime = 0;

        while(spawnTime < spawnDelay){
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTime * tileFlashspeed, 1));

            spawnTime += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeadth += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColour);
    }

    void OnEnemyDeath(){
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0){
            NextWave();
        }
    }

    void OnPlayerDead(){
        isDis = true;
    }

    void ResetPlayerPosition(){
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave(){
        if (currentWaveNumber > 0){
            AudioManager.instance.PlaySound2D("Level Complete");
        }
        currentWaveNumber++;

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if (OnNewWave != null){
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

}
