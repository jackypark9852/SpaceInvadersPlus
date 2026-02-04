using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

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
    public float ammoPerShot = 5f;   // how much gauge each shot costs

    [Header("Bomb Powerup")]
    public GameObject bombPrefab;
    public int bombCharges = 0;

    [Header("Suck")]
    public KeyCode suckKey = KeyCode.LeftShift;
    public float suckRadius = 5f;
    public Vector3 suckOffset = Vector3.zero;
    public float absorbRadius = 0.5f;
    public Vector3 absorbOffset = Vector3.zero;
    public float suckForce = 30f;
    public float suckUpwardForce = 5f;
    [FormerlySerializedAs("fragmentLayer")] public LayerMask pickupLayer;

    [Header("Power Gauge / Ammo")]
    public float maxGauge = 100f;
    public float gaugePerFragment = 5f;
    public float currentGauge;

    [Header("Ammo UI")]
    public Slider ammoSlider;

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

        // Init ammo (full) and slider
        currentGauge = maxGauge;
        if (ammoSlider != null)
        {
            ammoSlider.minValue = 0f;
            ammoSlider.maxValue = maxGauge;
            ammoSlider.value = currentGauge;
        }
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

        // Require ammo to shoot
        if (Input.GetButton("Fire1") && fireTimer <= 0f && currentGauge >= ammoPerShot)
        {
            Shoot();
            fireTimer = fireInterval;
        }

        fireTimer -= Time.deltaTime;

        if (Input.GetKey(suckKey))
            SuckFragments();
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;
        if (bombCharges > 0)
        {
            // Launch bomb instead of normal shot
            LaunchBomb();
            bombCharges--;
            return;
        }

        // Consume ammo
        currentGauge = Mathf.Max(0f, currentGauge - ammoPerShot);

        // Update ammo slider
        if (ammoSlider != null)
        {
            ammoSlider.value = currentGauge;
        }

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        PlaySFX(shootSFX);
    }

    void LaunchBomb()
    {
        if (bombPrefab == null) return;

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.LookRotation(transform.forward));
        BombProjectile bombScript = bomb.GetComponent<BombProjectile>();
    }

    void SuckFragments()
    {
        Vector3 suckCenter = transform.position + suckOffset;
        Vector3 absorbCenter = transform.position + absorbOffset;

        Collider[] hits = Physics.OverlapSphere(suckCenter, suckRadius, pickupLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Fragment") && !hit.CompareTag("Powerup")) continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb == null) continue;

            // Where do we absorb to? (spaceship center + absorbOffset)
            Vector3 toAbsorb = absorbCenter - rb.position;
            float distToAbsorb = toAbsorb.magnitude;

            if (distToAbsorb < absorbRadius)
            {
                if (hit.CompareTag("Fragment"))
                {
                    AbsorbFragment(hit.gameObject);
                }
                else if (hit.CompareTag("Powerup"))
                {
                    AbsorbPowerup(hit.gameObject);
                }

                continue;
            }

            // Always pull toward the ship (or absorb center), not the suckCenter
            Vector3 dir = toAbsorb.normalized;

            Vector3 force = dir * suckForce;
            force += new Vector3(0f, 0f, -suckUpwardForce);

            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void AbsorbFragment(GameObject fragment)
    {
        currentGauge = Mathf.Min(maxGauge, currentGauge + gaugePerFragment);

        if (ammoSlider != null)
        {
            ammoSlider.value = currentGauge;
        }

        Destroy(fragment);
    }

    void AbsorbPowerup(GameObject powerup)
    {
        bombCharges = Mathf.Min(bombCharges + 1, 1); // cap at 1 if “next shot” only

        Destroy(powerup);
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

    void OnDrawGizmosSelected()
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

        Vector3 center = transform.position + suckOffset;

        Vector3 suckCenter = transform.position + suckOffset;
        Vector3 absorbCenter = transform.position + absorbOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(suckCenter, suckRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(absorbCenter, absorbRadius);
    }
}
