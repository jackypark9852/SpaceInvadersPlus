using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Shooting")] public GameObject projectilePrefab;
    [Header("Death")] public AudioClip deathSFX;
    [Header("Value")] public int points = 10;
    
    [Header("Restore Initial Rotation")]
    public float restoreStrength = 2f;
    public float restoreDamping = 1f;

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
        PlaySFX(deathSFX);
        manager.ReportAlienDeath();
        GameManager.Instance.AddScore(points);
        gameObject.SetActive(false);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}