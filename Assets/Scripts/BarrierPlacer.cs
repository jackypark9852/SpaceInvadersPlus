using UnityEngine;

[ExecuteInEditMode]
public class BarrierPlacer : MonoBehaviour
{
    public GameObject barrierPrefab;
    [Min(1)] public int barrierCount = 4;
    public float minX = -7f, maxX = 7f;
    public float zPosition = 5f;
    public float yPosition = 0f;

    [HideInInspector] public bool dirty = true;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        dirty = true;
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && dirty)
        {
            dirty = false;
            PlaceBarriers();
        }
#endif

        if (!Application.isPlaying) return;
    }

    void Start()
    {
        PlaceBarriers();
    }

    void PlaceBarriers()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        if (barrierPrefab == null || barrierCount <= 0) return;

        float width = maxX - minX;
        float xStep = (barrierCount > 1) ? width / (barrierCount - 1) : 0f;

        for (int i = 0; i < barrierCount; i++)
        {
            float x = minX + i * xStep;
            Vector3 pos = new Vector3(x, yPosition, zPosition);

#if UNITY_EDITOR
            var barrier = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(barrierPrefab, transform);
            barrier.transform.position = pos;
#else
            Instantiate(barrierPrefab, pos, Quaternion.identity, transform);
#endif
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(minX, transform.position.y, zPosition),
            new Vector3(maxX, transform.position.y, zPosition)
        );
    }
}