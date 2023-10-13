using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_GameController=null;
    public GameObject m_DestroyObject;
    public FPSController m_Player;
    float m_PlayerLife;
    float m_PlayerShield;


    static public GameController GetGameController()
    {
        if(m_GameController=null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
            GameControllerData l_GameControllerData = Resources.Load <GameControllerData>("GameControllerData");
            m_GameController.m_PlayerLife = l_GameControllerData.m_lifes;
            Debug.Log("Data loaded with life" + m_GameController.m_PlayerLife);
        }
        return m_GameController;    
    }
    public void SetPLayerLife(float PlayerLife)
    {
        m_PlayerLife = PlayerLife;
    }
    public float GetPlayerLife()
    {
        return m_PlayerLife;
    }
    public void SetPLayerShield(float PlayerShield)
    {
        m_PlayerShield = PlayerShield;
    }
    public float GetPlayerShield()
    {
        return m_PlayerShield;
    }
        public void SetPlayer(FPSController Player)
    {
        m_Player = Player;
    }
}
