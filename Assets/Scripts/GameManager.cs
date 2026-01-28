using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject gameOverText;
    public GameObject winPanel;
    public GameObject winText;
    public GameObject restartButton;
    public int score = 0;
    public int lives = 3;
    public Spaceship player;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI livesText;
    public Vector3 respawnPos = new Vector3(0, 0, -5f);
    public float respawnDelay = 1.5f;

    float respawnTimer;
    bool isRespawning;
    bool gameEnded;

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

    void Start()
    {
        HideAllPanels();
        UpdateUI();
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
        if (gameEnded) return;
        score += points;
        UpdateUI();
    }

    public void LoseLife()
    {
        if (gameEnded) return;
        lives--;
        UpdateUI();
        
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            StartRespawn();
        }
    }

    public void Win()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null)
                winText.SetActive(true);
            restartButton.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.SetActive(true);
            restartButton.SetActive(true);
        }
    }

    void HideAllPanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        if (livesText != null)
            livesText.text = $"Lives: {lives}";
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

    public void ResetGame()
    {
        score = 0;
        lives = 3;
        Time.timeScale = 1f;
        gameEnded = false;
        HideAllPanels();
        UpdateUI();
    }
}
