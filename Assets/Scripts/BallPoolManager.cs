using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BallPoolManager : MonoBehaviour
{
    public GameObject BallPrefab;
    public IObjectPool<Ball> BallPool { get; private set; }

	private void Start()
	{
		InitPool();
	}


	private void InitPool()
    {
        BallPool = new ObjectPool<Ball>(CreateBall, OnGetBall, OnReleaseBall, OnDestroyBall, true, 100);
	}

    private Ball CreateBall()
	{
		GameObject ballObject = Instantiate(BallPrefab);
		Ball ball = ballObject.GetComponent<Ball>();
        ball.SetPool(BallPool);
		return ball;
	}

    private void OnGetBall(Ball ball)
    {
		ball.gameObject.SetActive(true);
	}

    private void OnReleaseBall(Ball ball)
    {
        ball.gameObject.SetActive(false);
	}

	private void OnDestroyBall(Ball ball)
	{
		Destroy(ball.gameObject);
	}
}
