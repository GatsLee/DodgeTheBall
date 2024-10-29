using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeUIController : MonoBehaviour
{
    [SerializeField] private GameObject PlayerLife1;
	[SerializeField] private GameObject PlayerLife2;
    [SerializeField] private GameObject PlayerLife3;
	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
        switch(GameManager.Instance.PlayerLife)
		{
			case 3:
				PlayerLife1.SetActive(true);
				PlayerLife2.SetActive(true);
				PlayerLife3.SetActive(true);
				break;
			case 2:
				PlayerLife1.SetActive(true);
				PlayerLife2.SetActive(true);
				PlayerLife3.SetActive(false);
				break;
			case 1:
				PlayerLife1.SetActive(true);
				PlayerLife2.SetActive(false);
				PlayerLife3.SetActive(false);
				break;
			case 0:
				PlayerLife1.SetActive(false);
				PlayerLife2.SetActive(false);
				PlayerLife3.SetActive(false);
				break;
		}
	}
}
