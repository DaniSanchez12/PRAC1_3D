using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int m_Score;
    public static PlayerManager instance;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        FPSController l_Player = GameController.GetGameController().GetPlayer();

        scoreText.text = "Score: " + m_Score;
        timerText.text = "Timer: " +  l_Player.GetTime().ToString("0.0");

        if(scoreText.IsActive() && timerText.IsActive())
        {
            scoreText.outlineWidth = 0.3f;
            scoreText.outlineColor = Color.black;

            timerText.outlineWidth = 0.3f;
            timerText.outlineColor = Color.black;
        }
    }
}
