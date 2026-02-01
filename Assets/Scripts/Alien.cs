using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Shooting")] public GameObject projectilePrefab;
    [Header("Death")] public AudioClip deathSFX;
    [Header("Value")] public int points = 10;

    [Header("Restore Initial Rotation")]
    public float restoreStrength = 2f;
    public float restoreDamping = 1f;

    [Header("Death Fragments")]
    public GameObject[] fragmentPrefabs;     // assign in Inspector
    public int fragmentsPerPrefab = 1;       // how many of each to spawn
    public Vector3 fragmentSpawnOffset;      // optional offset

    AliensManager manager;
    AudioSource audioSource;
    Rigidbody rb;
    Quaternion initialRotation;

    void Start()
    {
        manager = GetComponentInParent<AliensManager>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Quaternion deltaRot = initialRotation * Quaternion.Inverse(transform.rotation);
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 torque = axis * (angle * Mathf.Deg2Rad) * restoreStrength;
        torque -= rb.angularVelocity * restoreDamping;

        rb.AddTorque(torque, ForceMode.VelocityChange);
    }

    public void Shoot()
    {
        if (projectilePrefab == null) return;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }

    public void Kill()
    {
        // spawn fragments at alien position
        SpawnFragments();

        PlaySFX(deathSFX);
        if (manager != null)
            manager.ReportAlienDeath();
        GameManager.Instance.AddScore(points);
        gameObject.SetActive(false);
    }

    void SpawnFragments()
    {
        if (fragmentPrefabs == null || fragmentPrefabs.Length == 0) return;

        Vector3 basePos = transform.position + fragmentSpawnOffset;
        basePos.y = 0f; // force Y to zero

        foreach (var prefab in fragmentPrefabs)
        {
            if (prefab == null) continue;

            for (int i = 0; i < fragmentsPerPrefab; i++)
            {
                Vector3 randOffset = Random.insideUnitSphere * 0.2f;
                randOffset.y = 0f; // keep random offset flat on XZ plane

                Instantiate(prefab, basePos + randOffset, transform.rotation);
            }
        }
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
