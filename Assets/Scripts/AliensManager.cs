using UnityEngine;

[ExecuteInEditMode]
public class AliensManager : MonoBehaviour
{
    public GameObject alienPrefab;
    [Min(1)] public int rows = 5;
    [Min(1)] public int aliensPerRow = 11;
    public float minX = -5f, maxX = 5f;
    public float minZ = 0f, maxZ = 10f;
    public Vector3 originOffset = Vector3.zero;
    
    public float stepSpeed = 4f;      // units per second along X
    public float stepDown = 0.5f;     // world units along Z
    public float leftLimit = -8f;     // world X
    public float rightLimit = 8f;     // world X
    
    public float moveInterval = 0.2f; // time between alien "steps"
    float timer;
    
    int direction = 1; // 1 = right, -1 = left

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
            RebuildAliens();
        }
#endif

        if (!Application.isPlaying)
            return;

        timer += Time.deltaTime;
        if (timer < moveInterval)
            return;

        timer = 0f;
        StepAliens();
    }

    void RebuildAliens()
    {
        // Clear existing children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }

        if (!alienPrefab || rows <= 0 || aliensPerRow <= 0) return;

        float width = maxX - minX;
        float depth = maxZ - minZ;
        float xStep = (aliensPerRow > 1) ? width / (aliensPerRow - 1) : 0f;
        float zStep = (rows > 1) ? depth / (rows - 1) : 0f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < aliensPerRow; col++)
            {
                float x = minX + col * xStep;
                float z = minZ + row * zStep;

                // World‑space spawn position
                var pos = originOffset + new Vector3(x, 0f, z);

#if UNITY_EDITOR
                var go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(alienPrefab, transform);
                go.transform.position = pos;
#else
                Instantiate(alienPrefab, pos, Quaternion.identity, transform);
#endif
            }
        }

        // Optionally, align the manager to the center of the formation in world‑space
        // and move children so their world positions are unchanged:
        // (Useful if you want to move the block via the parent.)
        Vector3 oldRootPos = transform.position;
        Vector3 center = ComputeWorldCenter();
        transform.position = center;

        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            c.position += oldRootPos - center;
        }
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
            pos.z -= stepDown;     // move down only
            transform.position = pos;

            direction *= -1;
            return;
        }

        // Normal horizontal step
        Vector3 p = transform.position;
        p.x += direction * stepSpeed * moveInterval;
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

            float x = child.position.x; // world X
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
        // Draw aliens extents (current formation bounds)
        if (transform.childCount > 0)
        {
            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minZ = float.PositiveInfinity;
            float maxZ = float.NegativeInfinity;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (!child.gameObject.activeInHierarchy) continue;

                Vector3 p = child.position; // world space
                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;
                if (p.z < minZ) minZ = p.z;
                if (p.z > maxZ) maxZ = p.z;
            }

            if (minX != float.PositiveInfinity)
            {
                float width = maxX - minX;
                float depth = maxZ - minZ;
                Vector3 center = new Vector3(minX + width * 0.5f, transform.position.y, minZ + depth * 0.5f);
                Vector3 size = new Vector3(width, 0.0f, depth);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, size); // aliens extents[web:82][web:86]
            }
        }

        // Draw movement limits in world space
        Gizmos.color = Color.yellow;

        // Left limit line
        Gizmos.DrawLine(
            new Vector3(leftLimit, transform.position.y, -100f),
            new Vector3(leftLimit, transform.position.y, 100f)
        );

        // Right limit line
        Gizmos.DrawLine(
            new Vector3(rightLimit, transform.position.y, -100f),
            new Vector3(rightLimit, transform.position.y, 100f)
        );
        
    }

}
