using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;
    public Text healthText;
    public Text ammoText;
    public RawImage Death;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Death.gameObject.SetActive(false);
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
        if (PV.IsMine) RefreshHealthDisplay(100f);
    }

    public void RefreshHealthDisplay(float healthAmount) {
        healthText.text = healthAmount.ToString() + "/100 Health";
    }

    public void RefreshAmmoDisplay(float currentAmmoAmount, float maxAmmoAmount) {
        ammoText.text = currentAmmoAmount.ToString() + "/" + maxAmmoAmount.ToString() + " Ammo";
    }

    public void Die() {
        Death.gameObject.SetActive(true);
        PhotonNetwork.Destroy(controller);
        
        Invoke("CreateController", 2f);
    }
}
