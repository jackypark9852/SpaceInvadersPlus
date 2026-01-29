using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Speed Curve")]
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 12f, 1f, 30f);
    public float maxCurveTime = 1f;
    public float lifeTime = 5f;

    float curveTimer;
    float baseSpeed;

    void Start()
    {
        baseSpeed = speedCurve.Evaluate(0f);
        curveTimer = 0f;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        curveTimer = Mathf.Min(curveTimer + Time.deltaTime, maxCurveTime);
        float curveSpeed = speedCurve.Evaluate(curveTimer / maxCurveTime);
        float currentSpeed = curveSpeed * Time.deltaTime;
        
        transform.Translate(Vector3.back * currentSpeed, Space.World);
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
                UFO ufo = other.GetComponent<UFO>();
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