using System.Collections;
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
    public PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;
    PlayerlistManager plm;

    public GameObject nameTag;
    public GameObject playerUI;
    public Text healthText;
    public Text roleText;
    public Image roleImage;
    public Role role;
    public string username = "No Name";
    Player[] _players;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        plm = GetComponent<PlayerlistManager>();

        playerManager = PhotonView.Find((int) PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {        
        if(PV.IsMine)
        {
            EquipItem(0);
            InitializeName(PhotonNetwork.NickName);
            plm.UpdatePlayerlist();
            Cursor.lockState = CursorLockMode.Locked;
            
            foreach (Item item in items) {
                ((GunInfo) item.itemInfo).currentAmmo = ((GunInfo) item.itemInfo).maxAmmo;
                ((GunInfo) item.itemInfo).nextTimeToFire = Time.time + 1f;
            }
            healthText.text = "100/100 Health";
            ((Gun) items[itemIndex]).RefreshAmmoDisplay(((GunInfo) items[itemIndex].itemInfo).currentAmmo, ((GunInfo) items[itemIndex].itemInfo).maxAmmo);
        }
        else {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(playerUI);
        }
        PV.RPC(nameof(plm.UpdatePlayerlist), RpcTarget.Others);
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
            else                EquipItem(itemIndex - 1);
        }

        if (Input.GetMouseButtonDown(0)) {
            if (Time.time >= ((GunInfo) items[itemIndex].itemInfo).nextTimeToFire) {
                if (!((GunInfo) items[itemIndex].itemInfo).isReloading) {
                    ((GunInfo) items[itemIndex].itemInfo).nextTimeToFire = Time.time + 1f / ((GunInfo) items[itemIndex].itemInfo).fireRate;
                    items[itemIndex].Use();
                }
                else {
                    Debug.Log("You are currently reloading and therefor cannot shoot right now!");
                }
            }
            else {
                Debug.Log("You must wait " + (((GunInfo) items[itemIndex].itemInfo).nextTimeToFire - Time.time).ToString() + "s before shooting again!");
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !((GunInfo) items[itemIndex].itemInfo).isReloading && ((GunInfo) items[itemIndex].itemInfo).currentAmmo < ((GunInfo) items[itemIndex].itemInfo).maxAmmo) {
            ((Gun) items[itemIndex]).Reload();
            ((Gun) items[itemIndex]).RefreshAmmoDisplay(((GunInfo) items[itemIndex].itemInfo).currentAmmo, ((GunInfo) items[itemIndex].itemInfo).maxAmmo);
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
            ((Gun) items[itemIndex]).RefreshAmmoDisplay(((GunInfo) items[itemIndex].itemInfo).currentAmmo, ((GunInfo) items[itemIndex].itemInfo).maxAmmo);
        }

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner) {
            if (changedProps["itemIndex"] != null) EquipItem((int) changedProps["itemIndex"]);
            if (changedProps["nameTag"]   != null) InitializeName(((string) changedProps["nameTag"]));
            if (changedProps["roleName"]  != null) PV.RPC("RPC_InitializeRole", RpcTarget.All, (((string) changedProps["roleName"])));
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

        healthText.text = currentHealth.ToString()  + "/100 Health";

        if (currentHealth <= 0) {
            Die();
        }
    }

    
    void Die() {
        playerManager.Die();
    }

    public void InitializeName(string _username) {
        username = _username;
        nameTag.GetComponent<TextMeshPro>().text = username;
        name = username;
        
        if (PV.IsMine) {
            Hashtable hash = new Hashtable();
            hash.Add("nameTag", _username);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            nameTag.GetComponent<TextMeshPro>().text = PhotonNetwork.NickName;
        }
    }

    public void InitializeRole(object[] availableRoles) {
        Player[] _players = PhotonNetwork.PlayerList;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            PV.RPC("RPC_InitializeRole", _players[i], availableRoles[i]);
        }
    }

    [PunRPC]
    public void RPC_InitializeRole(string _roleName) {
        role = RoleManager.Instance.GetRoleByName(_roleName);
        nameTag.GetComponent<TextMeshPro>().color = role.GetColor();
        if (PV.IsMine) {
            roleText.text = role.roleName;
            roleImage.color = role.GetColor();
            Hashtable hash = new Hashtable();
            hash.Add("roleName", role.roleName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

    }
}