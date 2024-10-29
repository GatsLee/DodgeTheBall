using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision != null) {
			if (collision.gameObject.tag == "Ball" && GameManager.Instance.PlayerLife > 0)
			{
				GameManager.Instance.PlayerLife -= 1;
			}
		}
	}
}
