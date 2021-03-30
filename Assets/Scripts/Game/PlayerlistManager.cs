using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerlistManager : MonoBehaviour
{
    public Transform playerlistObject;
    public GameObject playerlistItemGamePrefab;

    private void Awake() {
        playerlistObject.gameObject.SetActive(false);
    }
    private void Update() {
        if (GetComponent<PhotonView>().IsMine && Input.GetKeyDown(KeyCode.Tab)) {
            playerlistObject.gameObject.SetActive(true);
        }
        if (GetComponent<PhotonView>().IsMine && Input.GetKeyUp(KeyCode.Tab)) {
            playerlistObject.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void UpdatePlayerlist() {
        if (GetComponent<PhotonView>().IsMine) {
            foreach (Transform child in playerlistObject) {
                if (child.gameObject.tag != "Playerlist")
                    Destroy(child.gameObject);
            }
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = -1; i < players.Length; i++) {
                GameObject newListitem = Instantiate(playerlistItemGamePrefab, playerlistObject.transform);
                if (i == -1) newListitem.GetComponent<Text>().text = "<b><i>Playerlist:</i></b>";
                else newListitem.GetComponent<Text>().text = players[i].GetComponent<PlayerController>().username;
            }   
        }
    }
}
