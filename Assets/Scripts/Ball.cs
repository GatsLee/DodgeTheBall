using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Ball : MonoBehaviour
{
	private IObjectPool<Ball> ballPool;
	private float ballRemainTime;
	public string ballDirection;

	// Start is called before the first frame update
	void Start()
    {
		ballRemainTime = 0f;
		ballDirection = "none";
    }

    // Update is called once per frame
    void Update()
    {
		ballRemainTime += Time.deltaTime;
		// if the ball is out of scope of window: back to object pool
		if (GameManager.Instance.GameStatus != 1)
		{
			ballPool.Release(this);
		}
		else
		{
			if (IsOutOfScope())
			{
				ballPool.Release(this);
				GameManager.Instance.ActiveBallCount--;
			}
		}
	}

	private bool IsOutOfScope()
	{
		Vector3 minPlayableBounds = GameManager.Instance.minPlayableBounds;
		Vector3 maxPlayableBounds = GameManager.Instance.maxPlayableBounds;
		if (minPlayableBounds == null || maxPlayableBounds == null)
		{
			return false;
		}
		float curBallX = this.transform.position.x;
		float curBallY = this.transform.position.y;

		if (minPlayableBounds.x + 0.2f > curBallX
			|| minPlayableBounds.y + 0.2f > curBallY
			|| maxPlayableBounds.x - 0.2f < curBallX
			|| maxPlayableBounds.y - 0.2f < curBallY)
		{
			return true;
		}
		return false;
	}

	public void SetPool(IObjectPool<Ball> ballPool)
	{
		// set the ball pool
		this.ballPool = ballPool;
	}
}
