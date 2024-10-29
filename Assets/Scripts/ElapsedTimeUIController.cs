using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElapsedTimeUIController : MonoBehaviour
{
    private TextMeshProUGUI TextMeshProUGUI;
	// Start is called before the first frame update
	void Start()
    {
        TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
	}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameStatus == 1)
        {
            ParseElapsedTimeToText();
        }
        else
        {
            TextMeshProUGUI.text = "00:00";
		}
    }

    private void ParseElapsedTimeToText()
    {
        int elapsedTime = (int)GameManager.Instance.PlayerElapsedTime;
	    int elapsedMinutes = elapsedTime / 60;
		int elapsedSeconds = elapsedTime % 60;

		string minutes = elapsedMinutes < 10 ? "0" + elapsedMinutes : elapsedMinutes.ToString();
		string seconds = elapsedSeconds < 10 ? "0" + elapsedSeconds : elapsedSeconds.ToString();

		TextMeshProUGUI.text = minutes + ":" + seconds;
	}
}
