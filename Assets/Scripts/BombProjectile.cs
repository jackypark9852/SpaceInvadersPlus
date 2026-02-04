using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float explosionRadius = 8f;
    public GameObject explosionPrefab;  // Assign explosion VFX prefab
    public AudioClip explosionSound;    // Assign explosion sound clip
    public LayerMask damageLayer;      // Enemies/fragments layer
    private Rigidbody rb;
    private AudioSource audioSource;   // Reference to AudioSource

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();  // Get AudioSource component
        rb.linearVelocity = -transform.forward * speed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Detonate();
        }
    }

    void Detonate()
    {
        // Play explosion sound
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Spawn explosion VFX
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));

        // Damage nearby enemies/fragments
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
            {
                continue;
            }

            Alien alien = hit.GetComponent<Alien>();
            if (alien != null)
            {
                alien.Kill();
            }
            else
            {
                UFO ufo = hit.GetComponent<UFO>();
                ufo.Kill();
            }
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
    void OnBecameInvisible() { Detonate(); }
}
