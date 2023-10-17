using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameController : MonoBehaviour
{
//el mio
    static GameController m_GameController=null;
    public FPSController m_Player;
    float m_PlayerLife = 1.0f;
    public TMP_Text lifeText;
    public TMP_Text shieldText;
    public TMP_Text ammoText;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static GameController GetGameController()
    {
        if(m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
            GameControllerData l_GameControllerData = Resources.Load<GameControllerData>("GameControllerData");
            m_GameController.m_PlayerLife = 3;
        }
        return m_GameController;
    }

    public FPSController GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayer(FPSController Player)
    {
        m_Player = Player;
    }

    public static void DestroySingleton()
    {
        if(m_GameController != null)
            GameObject.Destroy(m_GameController.gameObject);
        m_GameController = null;
    }

    public void SetLife(float PlayerLife)
    {
        m_PlayerLife = PlayerLife;
    }

    public float GetPlayerLife()
    {
        return m_PlayerLife;
    }

    public float GetShield()
    {
        return m_Player.GetShield();
    }

    public float GetAmmo()
    {
        return m_Player.GetAmmo();
    }

    public void RestartGame()
    {
        m_Player.RestartGame();
    }
}
