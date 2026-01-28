using UnityEngine;

public class AlienProjectile : MonoBehaviour
{
    public float speed = 10f; 
    public float stepInterval = 0.1f; 
    public float lifeTime = 5f;   

    float stepTimer;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        stepTimer = stepInterval;
    }

    void Update()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            stepTimer = stepInterval;
            
            transform.Translate(Vector3.back * speed * stepInterval, Space.World);
        }
    }
}