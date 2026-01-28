using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AliensManager : MonoBehaviour
{
    [Header("Alien Prefabs")]
    public GameObject alien1Prefab;
    public GameObject alien2Prefab;
    public GameObject alien3Prefab;

    [Min(1)] public int rows = 5;
    [Min(1)] public int aliensPerRow = 11;
    public float minX = -5f, maxX = 5f;
    public float minZ = 0f, maxZ = 10f;
    public Vector3 originOffset = Vector3.zero;

    [Header("Stats")]
    [SerializeField] int totalAliens;
    public int aliveAliens;

    [Header("Movement")]
    [Min(0.1f)] public float baseStepSpeed = 4f;
    [Min(0.1f)] public float speedMultiplierPerAlien = 0.02f;
    public float stepDown = 0.5f;
    public float leftLimit = -8f;
    public float rightLimit = 8f;

    public float baseMoveInterval = 0.2f;
    float currentMoveInterval;
    float timer;

    [Header("Group Shooting")]
    public float minShootInterval = 0.8f;
    public float maxShootInterval = 2.5f;
    float groupShootTimer;

    [Header("UFO")]
    public GameObject ufoPrefab;
    [Min(1f)] public float minUfoDelay = 10f;
    [Min(5f)] public float minUfoInterval = 20f;
    [Min(30f)] public float maxUfoInterval = 60f;
    float ufoSpawnTimer;
    bool ufoEnabled = true;
    GameObject currentUfo;

    int direction = 1;

    [HideInInspector] public bool dirty = true;

    void Start()
    {
        ResetMovementSpeed();
        ResetGroupShootTimer();
        ufoSpawnTimer = minUfoDelay;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && dirty)
        {
            dirty = false;
            RebuildAliens();
        }
#endif

        if (!Application.isPlaying)
            return;

        groupShootTimer -= Time.deltaTime;
        if (groupShootTimer <= 0f)
        {
            ShootRandomAlien();
            ResetGroupShootTimer();
        }

        ufoSpawnTimer -= Time.deltaTime;
        if (ufoSpawnTimer <= 0f && ufoEnabled && currentUfo == null)
        {
            SpawnUFO();
            ResetUFOSpawnTimer();
        }
        
        timer += Time.deltaTime;
        if (timer < currentMoveInterval)
            return;

        timer = 0f;
        StepAliens();
    }

    void ResetUFOSpawnTimer()
    {
        ufoSpawnTimer = Random.Range(minUfoInterval, maxUfoInterval);
    }

    void SpawnUFO()
    {
        if (ufoPrefab == null)
            return;

        float x = leftLimit - 50;
        Vector3 spawnPos = new Vector3(x, 0.0f, -110.0f);

        currentUfo = Instantiate(ufoPrefab, spawnPos, Quaternion.identity, transform);
        var ufo = currentUfo.GetComponent<UFO>();
        if (ufo != null)
            ufo.SetManager(this);
    }

    public void UFODestroyed()
    {
        currentUfo = null;
        ResetUFOSpawnTimer(); 
        ufoEnabled = true;
    }

    void ResetMovementSpeed()
    {
        float speedMult = 1f + (speedMultiplierPerAlien * (totalAliens - aliveAliens));
        currentMoveInterval = baseMoveInterval / speedMult;
    }

    public void ReportAlienDeath()
    {
        aliveAliens--;
        ResetMovementSpeed();
        if (aliveAliens <= 0)
        {
            GameManager.Instance.Win();
        }
    }

    void RebuildAliens()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        if (rows <= 0 || aliensPerRow <= 0) return;

        totalAliens = rows * aliensPerRow;
        aliveAliens = totalAliens;

        float width = maxX - minX;
        float depth = maxZ - minZ;
        float xStep = (aliensPerRow > 1) ? width / (aliensPerRow - 1) : 0f;
        float zStep = (rows > 1) ? depth / (rows - 1) : 0f;

        for (int row = 0; row < rows; row++)
        {
            GameObject prefab = GetPrefabForRow(row);
            if (prefab == null) continue;

            for (int col = 0; col < aliensPerRow; col++)
            {
                float x = minX + col * xStep;
                float z = minZ + row * zStep;
                var pos = originOffset + new Vector3(x, 0f, z);

#if UNITY_EDITOR
                var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
                go.transform.position = pos;
#else
                Instantiate(prefab, pos, Quaternion.identity, transform);
#endif
            }
        }

        Vector3 oldRootPos = transform.position;
        Vector3 center = ComputeWorldCenter();
        transform.position = center;

        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            c.position += oldRootPos - center;
        }

        ResetMovementSpeed();
    }

    GameObject GetPrefabForRow(int row)
    {
        return row switch
        {
            0 => alien1Prefab,
            1 or 2 => alien2Prefab,
            _ => alien3Prefab
        };
    }

    Vector3 ComputeWorldCenter()
    {
        if (transform.childCount == 0)
            return transform.position;

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            sum += transform.GetChild(i).position;
        }
        return sum / transform.childCount;
    }

    void StepAliens()
    {
        if (HitsEdge())
        {
            Vector3 pos = transform.position;
            pos.z -= stepDown;
            transform.position = pos;
            direction *= -1;
            return;
        }

        Vector3 p = transform.position;
        float currentSpeed = baseStepSpeed * (1f + (speedMultiplierPerAlien * (totalAliens - aliveAliens)));
        p.x += direction * currentSpeed * currentMoveInterval;
        transform.position = p;
    }

    bool HitsEdge()
    {
        if (transform.childCount == 0)
            return false;

        float minWorldX = float.PositiveInfinity;
        float maxWorldX = float.NegativeInfinity;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.gameObject.activeInHierarchy) continue;

            float x = child.position.x;
            if (x < minWorldX) minWorldX = x;
            if (x > maxWorldX) maxWorldX = x;
        }

        if (direction > 0 && maxWorldX >= rightLimit)
            return true;
        if (direction < 0 && minWorldX <= leftLimit)
            return true;

        return false;
    }

    void OnDrawGizmos()
    {
        if (transform.childCount > 0)
        {
            float minXg = float.PositiveInfinity;
            float maxXg = float.NegativeInfinity;
            float minZg = float.PositiveInfinity;
            float maxZg = float.NegativeInfinity;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (!child.gameObject.activeInHierarchy) continue;

                Vector3 p = child.position;
                if (p.x < minXg) minXg = p.x;
                if (p.x > maxXg) maxXg = p.x;
                if (p.z < minZg) minZg = p.z;
                if (p.z > maxZg) maxZg = p.z;
            }

            if (minXg != float.PositiveInfinity)
            {
                float width = maxXg - minXg;
                float depth = maxZg - minZg;
                Vector3 center = new Vector3(minXg + width * 0.5f, transform.position.y, minZg + depth * 0.5f);
                Vector3 size = new Vector3(width, 0.0f, depth);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, size);
            }
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            new Vector3(leftLimit, transform.position.y, -100f),
            new Vector3(leftLimit, transform.position.y, 100f)
        );

        Gizmos.DrawLine(
            new Vector3(rightLimit, transform.position.y, -100f),
            new Vector3(rightLimit, transform.position.y, 100f)
        );
    }

    void ResetGroupShootTimer()
    {
        groupShootTimer = Random.Range(minShootInterval, maxShootInterval);
    }

    void ShootRandomAlien()
    {
        List<Alien> livingAliens = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            var alienComp = transform.GetChild(i).GetComponent<Alien>();
            if (alienComp != null && transform.GetChild(i).gameObject.activeInHierarchy)
                livingAliens.Add(alienComp);
        }

        if (livingAliens.Count > 0)
        {
            int randIdx = Random.Range(0, livingAliens.Count);
            livingAliens[randIdx].Shoot();
        }
    }
}
