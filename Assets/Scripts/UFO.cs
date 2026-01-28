using UnityEngine;

public class UFO : MonoBehaviour
{
    [Header("Movement")]
    public float approachSpeed = 8f;
    public float weaveSpeed = 3f;
    public float minX = -6f;
    public float maxX = 6f;

    [Header("Direction Changes")]
    public float minWeaveTime = 1f;
    public float maxWeaveTime = 3f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float shootIntervalMin = 1f;
    public float shootIntervalMax = 3f;
    
    [Header("Points")]
    public int points = 100;

    AliensManager manager;

    enum State { Approaching, Weaving }
    State state = State.Approaching;
    
    float shootTimer;
    float weaveTimer;
    float weaveDirection = 1f;
    float targetX;

    void Start()
    {
        Vector3 pos = transform.position;
        shootTimer = Random.Range(shootIntervalMin, shootIntervalMax);
    }

    public void SetManager(AliensManager m)
    {
        manager = m;
    }

    void Update()
    {
        switch (state)
        {
            case State.Approaching:
                UpdateApproach();
                break;
            case State.Weaving:
                UpdateWeaving();
                break;
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = Random.Range(shootIntervalMin, shootIntervalMax);
        }
    }

    void UpdateApproach()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, 0f, approachSpeed * Time.deltaTime);
        transform.position = pos;
        
        if (Mathf.Abs(pos.x) < 0.1f)
        {
            state = State.Weaving;
            PickNewDirection();
        }
    }

    void UpdateWeaving()
    {
        weaveTimer -= Time.deltaTime;
        
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, weaveSpeed * Time.deltaTime);
        transform.position = pos;
        
        if (weaveTimer <= 0f)
        {
            PickNewDirection();
        }
    }

    void PickNewDirection()
    {
        weaveTimer = Random.Range(minWeaveTime, maxWeaveTime);
        
        if (weaveDirection > 0)
        {
            targetX = Random.Range(0f, maxX);
            weaveDirection = 1f;
        }
        else
        {
            targetX = Random.Range(minX, 0f);
            weaveDirection = -1f;
        }
        weaveDirection *= -1f;
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;
        
        GameObject proj = Instantiate(projectilePrefab, 
            transform.position + Vector3.up * 0.5f, 
            Quaternion.identity);
    }

    public void Kill()
    {
        GameManager.Instance.AddScore(points);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            Kill();
            Destroy(other.gameObject);
        }
    }

    void OnDestroy()
    {
        if (manager != null)
            manager.UFODestroyed();
    }
}
