using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // For UI handling
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject startingScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseScreen;
    
    // For background transition mechanism
    [SerializeField] private GameObject mainBackground;
    [SerializeField] private GameObject powerBackground;
    [SerializeField] private float bgTransitionSpeed;
    private BoxCollider powerBgCollider; // FOr triggering player color change
    private float backgroundPos = 3.5f;

    // Outside script communication
    private PlayerController playerControllerScript;
    private SoundControl soundControlScript;

    // Wall spawn mechanism starts
    private float spawnPosX = 21.5f;
    private float spawnPosZ = -0.5f;

    // Top wall spawn
    private float topYLowerBound = 9f;
    private float topYUpperBound = 10.5f;

    // Bottom wall spawn
    private float bottomYLowerBound = -2.3f;
    private float bottomYUpperBound = .35f;

    private float wallSpawnStart = 0.5f;
    private float wallRepeat = 0.30f;

    // Wall spawn mechanism ends

    // Collider and Powerup spawn mechanism
    private float colliderSpawnPosY = 3.0f;
    private float powerupSpawnPosY = 4.8f;
    private float powerupStart = 10;
    private float powerupRepeat = 25;

    // Game state handlers
    public bool gameStarted = false;
    public bool isGameOver = false;
    public bool isGamePaused = false;

    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        soundControlScript = GameObject.Find("Game Sound").GetComponent<SoundControl>();

        powerBgCollider = powerBackground.GetComponent<BoxCollider>();

        // Game is paused at the start, game gets going in the player
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (gameStarted && !isGameOver && Input.GetKeyDown(KeyCode.F))
        {
            PauseGame();
        }

        // Picking up power changes the background and player color
        if (playerControllerScript.hasPower)
        {
            if (powerBackground.transform.position.y > backgroundPos)
            {
                powerBackground.transform.Translate(Vector3.down * Time.deltaTime * bgTransitionSpeed, Space.World);
                mainBackground.transform.Translate(Vector3.down * Time.deltaTime * bgTransitionSpeed, Space.World);

                // Triggers player color change exactly when the background surpasses the player
                if (powerBackground.transform.position.y - (powerBgCollider.size.x / 2) < playerControllerScript.transform.position.y)
                {
                    playerControllerScript.playerRenderer.material.color = new Color(18f / 255f, 0, 166f / 255f);
                }
            }
        }
        // Main background appears and player color reverts
        else if (!playerControllerScript.hasPower)
        {
            if (mainBackground.transform.position.y < backgroundPos)
            {
                powerBackground.transform.Translate(Vector3.up * Time.deltaTime * bgTransitionSpeed, Space.World);
                mainBackground.transform.Translate(Vector3.up * Time.deltaTime * bgTransitionSpeed, Space.World);
                
                // Triggers player color change exactly when the background surpasses the player
                if (powerBackground.transform.position.y - (powerBgCollider.size.x / 2) > playerControllerScript.transform.position.y)
                {
                    playerControllerScript.playerRenderer.material.color = Color.white;
                }
            }
        }
    }

    // Pressing space at the start triggers this function
    private void StartGame()
    {
        gameStarted = true;

        startingScreen.SetActive(false);
        scoreText.gameObject.SetActive(true);

        InvokeRepeating("SpawnWall", wallSpawnStart, wallRepeat);
        InvokeRepeating("SpawnPower", powerupStart, powerupRepeat);

        playerControllerScript.StartingGame();
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PauseGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            soundControlScript.music.Pause();
            pauseScreen.SetActive(true);
        }
        else if (isGamePaused)
        {
            Time.timeScale = 1;
            isGamePaused = false;
            soundControlScript.music.Play();
            pauseScreen.SetActive(false);
        }
    }

    private void SpawnWall()
    {
        if (!isGameOver)
        {   
            // Top wall and collider
            Vector3 topWallPosition = new Vector3(spawnPosX, Random.Range(topYLowerBound, topYUpperBound), spawnPosZ);
            Vector3 topColliderPosition = new Vector3(spawnPosX, colliderSpawnPosY, spawnPosZ);

            GameObject topWall = ObjectPool.SharedInstance.GetPooledWall();
            
            if (topWall != null)
            {
                topWall.transform.position = topWallPosition;
                topWall.SetActive(true);
            }

            SpawnCollider(topColliderPosition);

            // Bottom wall and collider
            Vector3 bottomWallPosition = new Vector3(spawnPosX + 3, Random.Range(bottomYLowerBound, bottomYUpperBound), spawnPosZ);
            Vector3 bottomColliderPosition = new Vector3(spawnPosX + 3, colliderSpawnPosY, spawnPosZ);

            GameObject bottomWall = ObjectPool.SharedInstance.GetPooledWall();

            if (bottomWall != null)
            {
                bottomWall.transform.position = bottomWallPosition;
                bottomWall.SetActive(true);
            }

            SpawnCollider(bottomColliderPosition);
        }
    }

    private void SpawnCollider(Vector3 position)
    {
        GameObject topCollider = ObjectPool.SharedInstance.GetPooledCollider();

        if (topCollider != null)
        {
            topCollider.transform.position = position;
            topCollider.SetActive(true);
        }
    }

    private void SpawnPower()
    {
        if (!isGameOver)
        {
            Vector3 powerupSpawnPos = new Vector3(spawnPosX, powerupSpawnPosY, spawnPosZ);

            GameObject powerup = ObjectPool.SharedInstance.GetPooledPowerup();

            if (powerup != null)
            {
                powerup.transform.position = powerupSpawnPos;
                powerup.SetActive(true);
            }
        }
    }

    public void UpdateScore()
    {
        if (playerControllerScript.hasPower)
        {
            score += 3;
        }
        else
        {
            score += 1;
        }
        scoreText.text = "Score: " + score;
    }
}
