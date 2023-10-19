using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargets : MonoBehaviour
{
    [Header("Target")]
    [Space(10)]
    public Rigidbody m_RigidBody;
    public float m_Speed;
    public Transform[] m_WayPoints;
    int m_CurrentWayPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Movement();
    }

    void Movement()
    {
        if(Vector3.Distance(transform.position, m_WayPoints[m_CurrentWayPoint].position) < 0.25f)
        {
            m_CurrentWayPoint += 1;
            m_CurrentWayPoint = m_CurrentWayPoint % m_WayPoints.Length;
        }

        Vector3 dir = (m_WayPoints[m_CurrentWayPoint].position - transform.position).normalized;
        m_RigidBody.MovePosition(transform.position + dir*m_Speed*Time.deltaTime);
    }
}
