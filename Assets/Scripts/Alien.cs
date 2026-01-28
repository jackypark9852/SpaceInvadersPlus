using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootOrigin;
    
    public void Shoot()
    {
        if (projectilePrefab == null) return;
        
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }
}