using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] Item[] items;
    int itemIndex;
    int previousItemIndex = -1;
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;
    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    float nextTimeToFire = 1f;

    PlayerManager playerManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int) PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            EquipItem(0);
            Cursor.lockState = CursorLockMode.Locked;
            ((GunInfo) items[itemIndex].itemInfo).currentAmmo = ((GunInfo) items[itemIndex].itemInfo).maxAmmo;
            playerManager.RefreshAmmoDisplay(((GunInfo) items[itemIndex].itemInfo).currentAmmo, ((GunInfo) items[itemIndex].itemInfo).maxAmmo);
        }
        else {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;
        Look();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString())) {
                EquipItem(i);
                break;
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0) {
            if (itemIndex >= items.Length - 1) EquipItem(0);
            else                               EquipItem(itemIndex + 1);
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0) {
            if (itemIndex <= 0) EquipItem(items.Length - 1);
            else EquipItem(itemIndex - 1);
        }
        
        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire && !((GunInfo) items[itemIndex].itemInfo).isReloading) {
            nextTimeToFire = Time.time + 1f / ((GunInfo) items[itemIndex].itemInfo).fireRate;
            items[itemIndex].Use();
            playerManager.RefreshAmmoDisplay(((GunInfo) items[itemIndex].itemInfo).currentAmmo, ((GunInfo) items[itemIndex].itemInfo).maxAmmo);
            // Hallo Eddy
        }
    }



    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void EquipItem(int _itemIndex) {

        if(_itemIndex == previousItemIndex)
            return;

        itemIndex = _itemIndex;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1) {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;

        if (PV.IsMine) {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        playerManager.RefreshAmmoDisplay(((GunInfo) items[itemIndex].itemInfo).currentAmmo, ((GunInfo) items[itemIndex].itemInfo).maxAmmo);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner) {
            EquipItem((int) changedProps["itemIndex"]);
        }
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
            
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage) {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage) {
        if (!PV.IsMine)
            return;
        
        currentHealth -= damage;

        playerManager.RefreshHealthDisplay(currentHealth);

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        playerManager.Die();
    }
}