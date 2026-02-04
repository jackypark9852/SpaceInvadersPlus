using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float explosionRadius = 8f;
    public GameObject explosionPrefab;  // Assign explosion VFX prefab
    public LayerMask damageLayer;      // Enemies/fragments layer

    private Rigidbody rb;
    private bool armed = false;
    private Spaceship parentShip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    void Update()
    {
        if (armed && Input.GetKeyDown(KeyCode.Space))
        {
            Detonate();
        }
    }

    void Detonate()
    {
        // Spawn explosion VFX
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Damage nearby enemies/fragments
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        foreach (var hit in hits)
        {
            Rigidbody hitRb = hit.attachedRigidbody;
            if (hitRb != null)
            {
                Vector3 forceDir = (hitRb.position - transform.position).normalized;
                hitRb.AddExplosionForce(500f, transform.position, explosionRadius, 3f);
            }
            // Optional: Call enemy.Kill() or fragment destroy
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // Optional: Alpha preview sphere
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }

    // Auto-detonate if flies offscreen (safety)
    void OnBecameInvisible() { if (armed) Detonate(); }
}