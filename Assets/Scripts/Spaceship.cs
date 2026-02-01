using UnityEngine;
using System.Collections;

public class Spaceship : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float stepInterval = 0.1f;
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public AudioClip shootSFX;
    public float fireInterval = 0.2f;

    [Header("Suck")]
    public KeyCode suckKey = KeyCode.LeftShift;
    public float suckRadius = 5f;
    public float suckForce = 30f;
    public float suckUpwardForce = 5f;
    public LayerMask fragmentLayer;

    [Header("Power Gauge")]
    public float maxGauge = 100f;
    public float gaugePerFragment = 5f;
    public float gaugeDecayPerSecond = 5f;

    float currentGauge;

    [Header("Respawn")]
    public Vector3 respawnPos = new Vector3(0, 0, -5f);
    public float respawnDelay = 1.5f;
    public AudioClip deathSFX;

    [Header("Camera")]
    public GameCamera gameCamera;

    [Header("Audio Source")]
    public AudioSource audioSource;

    TPSCamera tpsCam;

    float moveTimer;
    float fireTimer;
    float horizInput;

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (gameCamera == null)
            gameCamera = FindObjectOfType<GameCamera>();

        tpsCam = FindObjectOfType<TPSCamera>();
        if (tpsCam != null)
            tpsCam.SetTarget(transform);

        fireTimer = fireInterval;
        moveTimer = stepInterval;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool toTPS = !gameCamera.IsFirstPersonMode();
            gameCamera.SetThirdPersonMode(toTPS);
        }

        horizInput = Input.GetAxis("Horizontal");

        Vector3 pos = transform.position;
        pos.x += -horizInput * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        if (Input.GetButton("Fire1") && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireInterval;
        }

        fireTimer -= Time.deltaTime;

        if (Input.GetKey(suckKey))
            SuckFragments();

        if (currentGauge > 0f)
            currentGauge = Mathf.Max(0f, currentGauge - gaugeDecayPerSecond * Time.deltaTime);
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        PlaySFX(shootSFX);
    }

    void SuckFragments()
    {
        Debug.Log("Sucking");
        Vector3 center = transform.position;
        Collider[] hits = Physics.OverlapSphere(center, suckRadius, fragmentLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Fragment")) continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb == null) continue;

            Vector3 dir = (center - rb.position);
            float dist = dir.magnitude;
            if (dist < 0.1f)
            {
                AbsorbFragment(hit.gameObject);
                continue;
            }

            dir /= dist;

            Vector3 force = dir * suckForce;
            force += Vector3.up * suckUpwardForce;

            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void AbsorbFragment(GameObject fragment)
    {
        currentGauge = Mathf.Min(maxGauge, currentGauge + gaugePerFragment);
        Destroy(fragment);
        // Optional: play absorb SFX/VFX here
    }

    public void Kill()
    {
        PlaySFX(deathSFX);
        GameManager.Instance.LoseLife();
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            new Vector3(minX, transform.position.y - 10f, transform.position.z),
            new Vector3(minX, transform.position.y + 10f, transform.position.z)
        );

        Gizmos.DrawLine(
            new Vector3(maxX, transform.position.y - 10f, transform.position.z),
            new Vector3(maxX, transform.position.y + 10f, transform.position.z)
        );

        Vector3 rectCenter = new Vector3((minX + maxX) * 0.5f,
                                         transform.position.y,
                                         transform.position.z);
        Vector3 rectSize = new Vector3(maxX - minX, 20f, 1f);
        Gizmos.DrawWireCube(rectCenter, rectSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, suckRadius);
    }

}
