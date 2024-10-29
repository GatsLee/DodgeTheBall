using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    //GameManager Instance
    private static GameManager instance;

    public static GameManager Instance
    {
	get 
		{ 
			if (instance == null)
				{
					Debug.LogError("GameManager is null");
			}
				return instance;
			
		} 
	}

    // core variables
    public int PlayerLife;
    public int GameStatus; // 0:start 1:play 2:pause 3:game over 
    public float PlayerElapsedTime;

	// playable rectangle coor array
	public RectTransform PlayableRectangle;
	public Vector3 minPlayableBounds;
	public Vector3 maxPlayableBounds;

	// ball-related variables
	public float ballSpawnPadding;
	private float ballSpawnTime;
	private float ballSpeed;
	private float ballMaxSpeed;
	private float ballMaxSize;

	// ball object pool & related variables
	public BallPoolManager BallPoolManager;
	public int ActiveBallCount;
	public int CurMaxActiveBallCount;
	public int MaxActiveBallCount;
	private float ballSpawnTimer = 0f;
	private float speedIncreaseTimer = 0f;

	// UI activate/inactivate
	public Transform UICanvas;
    private GameObject StartUI;
	private GameObject PlayUI;
	private GameObject PauseUI;
    private GameObject EndUI;
	public TextMeshProUGUI finalElapsedTime;

	// player unit & related variables
	public GameObject PlayerPrefab;
	public GameObject PlayerUnit;
	private bool isGamePlayed;

	// Start is called before the first frame update
	private float playerSpeed = 5f;
	private Rigidbody2D playerRb;
	private Vector2 movementVector;
	private Vector2 prevMovementVector;

	void Start()
    {
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
		StartUI = UICanvas.Find("StartUI").gameObject;
		PlayUI = UICanvas.Find("PlayUI").gameObject;
		PauseUI = UICanvas.Find("PauseUI").gameObject;
        EndUI = UICanvas.Find("EndUI").gameObject;

		Vector3[] worldCorners = new Vector3[4];
		PlayableRectangle.GetWorldCorners(worldCorners);

		minPlayableBounds = new Vector3(worldCorners[0].x, worldCorners[0].y, 0);
		maxPlayableBounds = new Vector3(worldCorners[2].x, worldCorners[2].y, 0);

		ballSpeed = 1f;
		ballSpawnPadding = 0.25f;
		isGamePlayed = false;
		ActiveBallCount = 0;
		CurMaxActiveBallCount = 30;
		MaxActiveBallCount = 100;

		PlayerUnit = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		playerRb = PlayerUnit.GetComponent<Rigidbody2D>();
		PlayerUnit.SetActive(false);
		movementVector = Vector2.zero;
	}

	// Update is called once per frame
	void Update()
    {
		// check the game status
		switch (GameStatus)
		{
			case 0:
				StartGame();
				break;
			case 1:
				// capture input for player movement
				movementVector.x = Input.GetAxis("Horizontal");
				movementVector.y = Input.GetAxis("Vertical");
				PlayGame();
				break;
			case 2:
				PauseGame();
				break;
			case 3:
				EndGame();
				break;
		}
	}

	private void FixedUpdate()
	{
		
	}


	public void StartGame()
    {
        // init the variables
		GameStatus = 0;
		PlayerElapsedTime = 0f;
		ResetVariables();

		//UI
		PlayUI.SetActive(false);
		StartUI.SetActive(true);
		PauseUI.SetActive(false);
		EndUI.SetActive(false);
	}

    public void PauseGame()
    {
		GameStatus = 2;
        // stop time
        // stop moving balls
        // disable the player to move the player unit
    }

    public void EndGame()
    {
		GameStatus = 3;
		ResetVariables();
		// set active for end game sprites & buttons
		PlayUI.SetActive(false);
		StartUI.SetActive(false);
		PauseUI.SetActive(false);
		EndUI.SetActive(true);
	}

	public void PlayGame()
	{
		GameStatus = 1;
		PlayUI.SetActive(true);
		StartUI.SetActive(false);
		PauseUI.SetActive(false);
		EndUI.SetActive(false);

		if (isGamePlayed == false)
		{
			PlayerElapsedTime = 0f;
			PlayerUnit.SetActive(true);
			isGamePlayed = true;
		}

		if (PlayerLife == 0)
		{
			GameStatus = 3;
			return;
		}
		PlayerElapsedTime += Time.deltaTime;

		// Spawning balls + Game Logic
		SpawnBallOnCommand();

		// move the player unit
		if (movementVector.magnitude != 0)
		{
			prevMovementVector = movementVector;
		}
		else
		{
			movementVector = prevMovementVector;
		}
		Vector2 tmpVector = playerRb.position + movementVector * playerSpeed * Time.fixedDeltaTime;
		Vector2 finalMovementVector = new Vector2(Mathf.Clamp(tmpVector.x, minPlayableBounds.x + ballSpawnPadding, maxPlayableBounds.x - ballSpawnPadding),
													Mathf.Clamp(tmpVector.y, minPlayableBounds.y + ballSpawnPadding, maxPlayableBounds.y - ballSpawnPadding));
		playerRb.MovePosition(finalMovementVector);
		playerRb.MoveRotation(Mathf.Atan2(movementVector.y, movementVector.x) * Mathf.Rad2Deg -90f);
	}

	private void SpawnBallOnCommand()
	{
		// if ball count hits the limits, stop the ball spawning
		if (ActiveBallCount > MaxActiveBallCount)
		{
			return;
		}
		ballSpawnTimer += Time.deltaTime;
		speedIncreaseTimer += Time.deltaTime;
		// every 60 seconds, 10 balls are added
		if (ballSpawnTimer >= 15f)
		{
			CurMaxActiveBallCount += 10;
			ballSpawnTimer = 0f;
		}
		// every 5mins, ball speed increases
		if (speedIncreaseTimer >= 300f)
		{
			ballSpeed += 0.1f;
			speedIncreaseTimer = 0f;
		}
		if (ActiveBallCount <= CurMaxActiveBallCount &&
			MaxActiveBallCount >= CurMaxActiveBallCount)
		{
			SpawnBallOnSide();
			ActiveBallCount++;
		}
	}

	private void SpawnBallOnSide()
	{
		Ball ball = BallPoolManager.BallPool.Get();
		if (ball == null)
		{
			return;
		}
		int spawnSide = Random.Range(0, 4);
		float spawnX = Random.Range(minPlayableBounds.x + ballSpawnPadding, maxPlayableBounds.x - ballSpawnPadding);
		float spawnY = Random.Range(minPlayableBounds.y + ballSpawnPadding, maxPlayableBounds.y - ballSpawnPadding);
		Vector3 ballDir = Vector3.zero;
		switch(spawnSide)
		{
			case 0: // top
				ballDir = Vector3.up;
				ball.ballDirection = "up";
				ball.transform.position = new Vector3(spawnX, minPlayableBounds.y + ballSpawnPadding, 0);
				break;
			case 1: // right
				ballDir = Vector3.left;
				ball.ballDirection = "right";
				ball.transform.position = new Vector3(maxPlayableBounds.x - ballSpawnPadding, spawnY, 0);
				break;
			case 2: // bottom
				ballDir = Vector3.down;
				ball.ballDirection = "down";
				ball.transform.position = new Vector3(spawnX, maxPlayableBounds.y - ballSpawnPadding, 0);
				break;
			case 3: // left
				ballDir = Vector3.right;
				ball.ballDirection = "left";
				ball.transform.position = new Vector3(minPlayableBounds.x + ballSpawnPadding, spawnY, 0);
				break;
			default:
				break;
		}
		
		float randomAngle = Random.Range(-70f, 70f);
		float angleInRadians = randomAngle * Mathf.Deg2Rad;
		Vector2 rotatedVector = new Vector2(
			ballDir.x * Mathf.Cos(angleInRadians) - ballDir.y * Mathf.Sin(angleInRadians),
			ballDir.x * Mathf.Sin(angleInRadians) + ballDir.y * Mathf.Cos(angleInRadians)
		);
		Rigidbody2D tmp = ball.GetComponent<Rigidbody2D>();
		// init the ball
		tmp.velocity = Vector2.zero;
		tmp.angularVelocity = 0f;
		tmp.AddForce(rotatedVector * ballSpeed, ForceMode2D.Impulse);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ResetVariables()
	{
		PlayerLife = 3;
		isGamePlayed = false;
		PlayerUnit.transform.position = new Vector3(0, 0, 0);
		PlayerUnit.SetActive(false);
		playerRb.velocity = Vector2.zero;
		playerRb.angularVelocity = 0f;

		ballSpawnTimer = 0f;
		speedIncreaseTimer = 0f;

		ActiveBallCount = 0;
		CurMaxActiveBallCount = 30;
	}
}
