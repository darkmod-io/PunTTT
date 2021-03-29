using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;
    GameObject deathUI;
    GameObject deathScreen;
    GameObject deathText;
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


            deathUI = GameObject.FindGameObjectWithTag("DeathUI");
            deathScreen = deathUI.GetComponentsInChildren<CanvasRenderer>()[0].gameObject;
            deathText = deathUI.GetComponentsInChildren<CanvasRenderer>()[1].gameObject;
            
            toggleDeathScreen();
        }
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die() {
        toggleDeathScreen();
        PhotonNetwork.Destroy(controller);        
        Invoke("CreateController", 2f);
        Invoke("toggleDeathScreen", 2f);
    }
    private void toggleDeathScreen() {
        deathScreen.SetActive(!deathScreen.activeInHierarchy);
        deathText.SetActive(!deathText.activeInHierarchy);
    }
}
