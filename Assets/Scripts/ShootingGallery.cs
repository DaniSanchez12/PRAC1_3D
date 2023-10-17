using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShootingGallery : MonoBehaviour
{
    // Start is called before the first frame update
    //rango y posicion
    public Transform m_DetectionPoint;
    private float m_DetectionDistance = 20;

    [Space(0.5f)]
    [Header("Targets")]
    [Space(1f)]

    public List<GameObject> m_TargetList;
    //canvas para mostrar
    [Space(0.5f)]
    [Header("HUD")]
    [Space(1f)]
    public Canvas m_Message;
    public TMP_Text m_TextMessage;
    public TMP_Text m_Score;
    public TMP_Text m_TimerText;
    public Canvas m_NewMessageCanvas;
    public TMP_Text m_NextMessage;

    //si se entra en el ShootingGallery 
    static ShootingGallery m_ShootingGallery;
    private bool m_Entered = false;
    private bool m_Scored = false;


    void Start()
    {
        m_ShootingGallery = GetComponent<ShootingGallery>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Message.isActiveAndEnabled)
        {
            //m_TextMessage.outlineColor = new Color32(255, 145, 0, 255);
            m_TextMessage.outlineColor = Color.black;
           // m_TextMessage.outlineWidth = 0.4f;
        }

        if (DetectCollision() && !m_Entered)
        {
            m_Entered = true;

            if(m_Entered)
            {
                ShowMessage();
            }
        }   
        if(PlayerManager.instance.m_Score >= 1000 && !m_Scored)
        {
            m_Scored = true;
            if (m_Scored)
            {
                m_NewMessageCanvas.gameObject.SetActive(true);
                StartCoroutine(HideMessage());
            }
        }
    }
    public void ActivateShootingGallery()
    {
        GameController.GetGameController().GetPlayer().ResetTime();
        m_Message.gameObject.SetActive(false);

        for (int i = 0; i < m_TargetList.Count; i++)
        {
            m_TargetList[i].GetComponent<Animation>().Play();
        }
    }
    void TimeUp()
    {
        if(GameController.GetGameController().GetPlayer().GetTime() <= 0)
        {
            for (int i = 0; i < m_TargetList.Count; i++)
            {
                m_TargetList[i].GetComponent<Animation>().Stop();
            }
        }
    }
    bool DetectCollision()
    {
        return (m_DetectionPoint.transform.position - GameController.GetGameController().GetPlayer().transform.position).magnitude <= m_DetectionDistance;
    }
    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(5);
        m_NewMessageCanvas.gameObject.SetActive(false);
    }
    public static ShootingGallery GetShootingGallery()
    {
        return m_ShootingGallery;
    }
    void ShowMessage()
    {
        m_TimerText.gameObject.SetActive(true);
        m_Message.gameObject.SetActive(true);
    }
}
