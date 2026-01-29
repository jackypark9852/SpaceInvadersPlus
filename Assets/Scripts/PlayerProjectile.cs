using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 12f;
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
        Debug.Log("Trigger entered");
        if (other.CompareTag("Enemy"))
        {
            Alien alien = other.GetComponent<Alien>();
            if (alien != null)
            {
                alien.Kill();
            }
            else
            {
                UFO  ufo = other.GetComponent<UFO>();
                ufo.Kill();
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