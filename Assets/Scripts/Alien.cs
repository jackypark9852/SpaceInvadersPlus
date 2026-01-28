using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Shooting")] public GameObject projectilePrefab;
    AliensManager manager;

    [Header("Value")] public int points = 10;

    void Start()
    {
        manager = GetComponentInParent<AliensManager>();
    }

    public void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }

    public void Kill()
    {
        GameManager.Instance.AddScore(points);
        gameObject.SetActive(false);
    }
}