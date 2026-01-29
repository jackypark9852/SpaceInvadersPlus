using UnityEngine;

public class AlienProjectile : MonoBehaviour
{
    public float speed = 10f; 
    public float lifeTime = 5f;   

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Spaceship player = other.GetComponent<Spaceship>();
            if (player != null)
            {
                player.Kill();
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("Barrier"))
        {
            Barrier barrier = other.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.Hit();
            }
            Destroy(gameObject);
        }
    }
}