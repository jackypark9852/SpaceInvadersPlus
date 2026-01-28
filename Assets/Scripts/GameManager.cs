using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int lives = 3;
    public Spaceship player;
    public Vector3 respawnPos = new Vector3(0, 0, -5f);
    public float respawnDelay = 1.5f;

    float respawnTimer;
    bool isRespawning;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isRespawning)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                RespawnComplete();
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
    }

    public void LoseLife()
    {
        lives--;
        Debug.Log($"Lives: {lives}");
        
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            StartRespawn();
        }
    }

    void StartRespawn()
    {
        isRespawning = true;
        respawnTimer = respawnDelay;
        player.gameObject.SetActive(false);
    }

    void RespawnComplete()
    {
        player.transform.position = respawnPos;
        player.gameObject.SetActive(true);
        isRespawning = false;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0f;
    }

    public void ResetGame()
    {
        score = 0;
        lives = 3;
        Time.timeScale = 1f;
    }
}