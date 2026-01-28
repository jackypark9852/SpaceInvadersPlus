using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Shooting")] public GameObject projectilePrefab;

    public void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }

    public void Kill()
    {
        GameManager.Instance.AddScore(10);
        gameObject.SetActive(false);
    }
}