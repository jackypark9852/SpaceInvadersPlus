using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Shooting")] public GameObject projectilePrefab;
    [Header("Death")] public AudioClip deathSFX;
    [Header("Value")] public int points = 10;

    AliensManager manager;
    AudioSource audioSource;

    void Start()
    {
        manager = GetComponentInParent<AliensManager>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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