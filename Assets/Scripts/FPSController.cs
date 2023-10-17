using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using System;

public class FPSController : MonoBehaviour
{


    // Start is called before the first frame update el mio

    //CONTROLES DE MOVIMIENTO
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

    public TMP_Text lifeText;
    public TMP_Text shieldText;
    public TMP_Text ammoText;

    //CARACTERISTICAS DEL PLAYER
    public CharacterController m_CharacterController;
    public float m_Speed;
    float m_SprintSpeed;
    public float m_FastSpeedMultiplier = 1.5f;
    public float m_JumpSpeed;
    float m_VerticalSpeed = 0.0f;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    public Camera m_Camera;
    public float m_NormalMovementFOV = 60;
    public float m_RunMovementFOV = 70;
    public bool m_Dead;

    //BLOQUEAR CAMARA
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    private ShootingGallery m_Gallery;
    private float m_Time;
    private float m_MaxTime = 30;
    private bool m_TimerActive = false;


    [Header("Inputs")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyKeyCode = KeyCode.D;
    public KeyCode m_UpKeyKeyCode = KeyCode.W;
    public KeyCode m_DownKeyKeyCode = KeyCode.S;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public KeyCode m_ShootingGalleryCode = KeyCode.L;


    [Header("Shoot")]
    public float m_MaxShootDistance = 50.0f;
    public LayerMask m_ShootingLayerMask;
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
        m_Life = GameController.GetGameController().GetPlayerLife();
        GameController.GetGameController().SetPlayer(this);
        m_MaxLife = m_Life;
        m_MaxShield = m_Shield;
        Cursor.lockState = CursorLockMode.Locked;
        m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        m_Dead = false;
        m_Time = m_MaxTime;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_DecalsPool = new TCObjectPool(5, m_DecalPrefab);
        m_CurrentHealth = m_MaxLife;
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
        float l_YawInverted=m_YawInverted ? -1.0f : 1.0f;
        float l_PitchInverted=m_PitchInverted ? -1.0f : 1.0f;

        float l_YawInRadians = m_Yaw*Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f)*Mathf.Deg2Rad;

        //calculos de vectores de movimiento
        Vector3 l_Forward = new Vector3 (Mathf.Sin(l_YawInRadians),0.0f, Mathf.Cos(l_YawInRadians));    
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
        if (Input.GetKeyDown(m_JumpKeyCode) && m_OnGround)
            m_VerticalSpeed = m_JumpSpeed;
        float l_FOV = m_NormalMovementFOV;

        if(Input.GetKey(m_SprintKeyCode))
        {
            l_Speed = m_Speed * m_FastSpeedMultiplier;
            l_FOV = m_RunMovementFOV;
        }
        m_Camera.fieldOfView = l_FOV;
            
        UpdateUI();
        if(Input.GetKeyDown(m_ShootingGalleryCode))
        {
            ShootingGallery.GetShootingGallery().ActivateShootingGallery();
            m_TimerActive = true;
        }

        if(m_Time <= 0)
        {
            m_TimerActive=false;
        }

        if(m_TimerActive)
        {
            m_Time -= Time.deltaTime;
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
            
        }
        else if (l_RaycastHit.collider.tag == "SmallTarget")
        {
           
        }
        
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
    void UpdateUI()
{
    if (lifeText != null)
        lifeText.text = "Life: " + m_Life.ToString("0");

    if (shieldText != null)
        shieldText.text = "Shield: " + m_Shield.ToString("0");

    if (ammoText != null)
        ammoText.text = "Ammo: " + m_CurrentAmmo + " / " + m_MaxAmmo;
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
    public float GetTime()
    {
        return m_Time;
    }
    public float GetShield()
    {
        return m_Shield;
    }
    public void AddLife(float Life)
    {
        m_Life = Mathf.Clamp(m_Life + Life, 0.0f, m_MaxLife);
    }
    public void AddShield(float Shield)
    {
        if(m_CurrentShield < 100)
        {
            m_CurrentShield += Shield;
            if(m_CurrentShield > 100) { m_CurrentShield = m_MaxShield; }
        }
    }
    public void RecieveDamage(float Damage)
    {
        if (m_CurrentShield >= 0)
        {
            m_Life -= Damage * 0.25f;
            m_CurrentShield -= Damage * 0.75f;
        }
        else
        {
            m_CurrentShield = 0;
            m_Life -= Damage;
        }

        if (m_Life <= 0)
        {
            Kill();
        }
    }
     void Kill()
    {
        m_Life = 0;
        GameController.GetGameController().RestartGame();
    }
        public void ResetTime()
    {
        m_Time = m_MaxTime;
    }
    public void RestartGame()
    {
        m_Life = 100;
        m_CurrentShield = 100;
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
        m_Dead = false;
    }
    IEnumerator EndShoot()
    {
        yield return new WaitForSeconds(m_ShootAnimationClip.length);
    }
}
