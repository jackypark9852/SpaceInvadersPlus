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
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;
        
        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        PlaySFX(shootSFX);
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
}
