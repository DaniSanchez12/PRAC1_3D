using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class FPSController : MonoBehaviour
{


    // Start is called before the first frame update el mio


    float m_Yaw;
    float m_Pitch;

    public Transform m_pitchController;
    public float m_YawRotationSpeed;
    public float m_PitchRotationSpeed;

    public float m_MinPitch;
    public float m_MaxPitch;

    public bool m_YawInverted;
    public bool m_PitchInverted;
    bool m_OnGround = true;

    public CharacterController m_CharacterController;

    public float m_Speed;
    float m_SprintSpeed;
    public float m_FastSpeedMultiplier = 1.5f;
    public float m_JumpSpeed;
    float m_VerticalSpeed = 0.0f;

    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    
    public Camera m_Camera;
    public float m_NormalMovementFOV = 60;
    public float m_RunMovementFOV = 70;

    public Text scoreText;
    public Text lifeText;
    public Text shieldText;


    [Header("Inputs")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyKeyCode = KeyCode.D;
    public KeyCode m_UpKeyKeyCode = KeyCode.W;
    public KeyCode m_DownKeyKeyCode = KeyCode.S;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_ReloadKeyCode = KeyCode.R;

    [Header("Shoot")]
    public float m_MaxShootDistance = 50.0f;
    public LayerMask m_ShootingLayerMask;
    public int m_Score;
    bool CanShoot = true;
    public GameObject m_DecalPrefab;
    TCObjectPool m_DecalsPool;

    [Header("Life")]
    public float m_Life;
    float m_MaxLife = 1.0f;
    public float m_Shield;
    float m_MaxShield = 1.0f;
    public float m_DroneDamage;
    public float m_CurrentHealth;
    public float m_CurrentShield;

    [Header("Ammo")]
    public int m_MaxAmmo = 30;
    public int m_CurrentAmmo = 30;
    public int m_TimesShot;

    [Header("Animations")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_ReloadAnimationClip;



    private void Awake()
    {
        m_CharacterController=GetComponent<CharacterController>();
        
    }

    void Start()
    {
        
        //m_Life = GameController.GetGameController().GetPlayerLife();
        m_MaxLife = m_Life;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_DecalsPool = new TCObjectPool(5, m_DecalPrefab);
        SetIdleWeaponAnimation();
    }
#if UNITY_EDITOR
    void UpdateInputDebug()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }
#endif

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        UpdateInputDebug();
#endif
        float l_HorizontalMovement = Input.GetAxis("Mouse X");
        float l_VerticalMovement = Input.GetAxis("Mouse Y");
        float l_Speed =m_Speed;

        //scoreText.text = "Score: " + m_Score.ToString();
        //lifeText.text = "Life: " + m_Life.ToString("F1"); // Muestra un decimal en la vida
        //shieldText.text = "Shield: " + m_Shield.ToString("F1"); // Muestra un decimal en el escudo


        if (Input.GetKeyDown(m_JumpKeyCode) && m_OnGround)
            m_VerticalSpeed = m_JumpSpeed;
        float l_FOV = m_NormalMovementFOV;

        if(Input.GetKey(m_SprintKeyCode))
        {
            l_Speed = m_Speed * m_FastSpeedMultiplier;
            l_FOV = m_RunMovementFOV;
        }
        m_Camera.fieldOfView = l_FOV;

        float l_YawInverted=m_YawInverted ? -1.0f : 1.0f;
        float l_PitchInverted=m_PitchInverted ? -1.0f : 1.0f;


        float l_YawInRadians = m_Yaw*Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f)*Mathf.Deg2Rad;

        Vector3 l_Forward = new Vector3 (Mathf.Sin(l_YawInRadians),0.0f, Mathf.Cos(l_YawInRadians));    //calculos de vectores de movimiento
        Vector3 l_Right = new Vector3 (Mathf.Sin(l_Yaw90InRadians),0.0f, Mathf.Cos(l_Yaw90InRadians));

        Vector3 l_Movement = Vector3.zero; // inicializacion

        if(Input.GetKey(m_LeftKeyCode))
            l_Movement =-l_Right;
        else if(Input.GetKey(m_RightKeyKeyCode))
            l_Movement = l_Right;

        if(Input.GetKey(m_UpKeyKeyCode))
            l_Movement+=l_Forward;
        else if(Input.GetKey(m_DownKeyKeyCode))
        {
        l_Movement-=l_Forward;
        }
            

        l_Movement.Normalize();
        l_Movement*= l_Speed+Time.deltaTime;
        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;


        m_Yaw += m_YawRotationSpeed * l_HorizontalMovement * Time.deltaTime*l_YawInverted;
        m_Pitch += m_PitchRotationSpeed * l_VerticalMovement * Time.deltaTime*l_PitchInverted;
        m_Pitch=Mathf.Clamp(m_Pitch,m_MinPitch,m_MaxPitch);
       

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_pitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y*Time.deltaTime;
        l_Movement.y = m_VerticalSpeed*Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if((l_CollisionFlags & CollisionFlags.CollidedBelow)!=0)
        {
            m_VerticalSpeed=0.0f;
        }


        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        if ((l_CollisionFlags & CollisionFlags.Below)!=0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
        }
        else
        {
            m_OnGround = false;
        }

        if(Input.GetMouseButtonDown(0) && CanShoot)
        {
            Shoot();
        }
        if (Input.GetKey(m_ReloadKeyCode) && m_CurrentAmmo < 30 && m_MaxAmmo > 0)
        {
            SetReloadAnimation();
            Reload();
        }
    }
    void Shoot()
    {
        Ray l_ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(l_ray, out l_RaycastHit, m_MaxShootDistance, m_ShootingLayerMask.value))
        {
            CreateShootHitParticles(l_RaycastHit.collider, l_RaycastHit.point, l_RaycastHit.normal);
            if (l_RaycastHit.collider.tag == "Target")
        {
            m_Score += 25; // Aumenta el puntaje en 25 al impactar en "Target"
        }
        else if (l_RaycastHit.collider.tag == "SmallTarget")
        {
            m_Score += 50; // Aumenta el puntaje en 50 al impactar en "SmallTarget"
        }
        
        Debug.Log("Puntaje actual: " + m_Score);
        }
        SetShootWeaponAnimation();
        m_TimesShot++;
        m_CurrentAmmo--;
        if(m_TimesShot >= 30)
        {
            CanShoot = false;
        }
    }

    void Reload()
    {
        m_TimesShot = 0;
        m_CurrentAmmo = 30;
        if(m_MaxAmmo != 0)
            m_MaxAmmo -= 30;
        StartCoroutine(ShootAfterReload());
    }
    IEnumerator ShootAfterReload()
    {
        yield return new WaitForSeconds(m_ReloadAnimationClip.length);
        CanShoot = true;
    }

    private void CreateShootHitParticles(Collider _Collider, Vector3 Position, Vector3 Normal)
    {
        //Debug.DrawRay(Position, Normal * 5.0f, Color.red, 2.0f);
        GameObject.Instantiate(m_DecalPrefab, Position, Quaternion.LookRotation(Normal));
        //GameObject l_Decal = m_DecalsPool.GetNextElement();
        //l_Decal.SetActive(true);
        //l_Decal.transform.position = Position;
        //l_Decal.transform.rotation = Quaternion.LookRotation(Normal);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            other.GetComponent<Item>().Pick(this);
            Debug.Log("a");
        }
    }

    void SetIdleWeaponAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimationClip.name);
    }
    void SetShootWeaponAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimationClip.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.1f);
        StartCoroutine(EndShoot());
    }
    void SetReloadAnimation()
    {
        m_Animation.CrossFade(m_ReloadAnimationClip.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.5f);
    }
    public int GetAmmo()
    {
        return m_MaxAmmo;
    }
    public void AddAmmo(int Ammo)
    {
        m_MaxAmmo += Ammo;
    }
    public float GetLife()
    {
        return m_Life;
    }
    public float GetShield()
    {
        return m_Shield;
    }
    public void AddLife(float Life)
    {
        m_Life = Mathf.Clamp(m_Life + Life, 0.0f, m_MaxLife);
    }

    IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(m_ShootAnimationClip.length);
    }

}
