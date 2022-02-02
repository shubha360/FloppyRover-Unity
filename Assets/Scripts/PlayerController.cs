using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public Renderer playerRenderer;

    private GameManager gameManagerScript;
    private SoundControl soundControlScript;

    public float jumpForce;
    private float gravityModifier = 2;
    private float startForce = 15; // force of the starting jump
    public bool hasPower; 
    private bool canJump = false; // Player can't jump for 1 second after starting the game
    private float playerYPosition; // Stores Y position for the power-lock

    // For controlling player gravity
    // Gravity is static. So it gets increased everytime the game restarts.
    private static Vector3 normalGravity; // Stores normal gravity at start
    private static bool isGravitySet = false; // Determines if gravity is set

    // Start is called before the first frame update
    void Start()
    {   
        // Sets normal gravity if not set
        if (!isGravitySet)
        {
            normalGravity = Physics.gravity;
            isGravitySet = true;
        }

        playerRb = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>();

        // Set the gameplay gravity
        Physics.gravity = normalGravity;
        Physics.gravity *= gravityModifier;

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        soundControlScript = GameObject.Find("Game Sound").GetComponent<SoundControl>();
    }

    // Starts the game with a jump
    public void StartingGame()
    {
        playerRb.AddForce(Vector3.up * startForce, ForceMode.Impulse);

        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(1);

        canJump = true; // Now the player can jump using space
    }

    // Update is called once per frame
    void Update()
    {   
        if (transform.position.y < -3)
        {
            gameManagerScript.GameOver();
        }

        if (hasPower)
        {
            MoveLeft.currentSpeed += 0.15f;
            soundControlScript.music.pitch += 0.01f;

            // Locks position
            if (transform.position.y < playerYPosition || transform.position.y > playerYPosition)
            {
                transform.position = new Vector3(transform.position.x, playerYPosition, transform.position.z);
            }
        }
        else if (!gameManagerScript.isGameOver &&
            !gameManagerScript.isGamePaused &&
            canJump &&
            Input.GetKeyDown(KeyCode.Space) &&
            transform.position.y < 8)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            gameManagerScript.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.CompareTag("Collider"))
        {
            gameManagerScript.UpdateScore();
        }

        if (other.gameObject.CompareTag("Powerup"))
        {
            other.gameObject.SetActive(false);

            hasPower = true;
            playerYPosition = transform.position.y;
            playerRb.useGravity = false;

            StartCoroutine(Power());
        }
    }

    IEnumerator Power()
    {
        yield return new WaitForSeconds(6);

        hasPower = false;
        playerRb.useGravity = true;
        MoveLeft.currentSpeed = 13;
        soundControlScript.music.pitch = 1;
    }
}