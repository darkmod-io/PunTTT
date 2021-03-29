using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance;
    Role[] roles;
    private void Awake() {
        Instance = this;
        roles = GetComponentsInChildren<Role>();
    }

    public void SetRoles(GameObject[] players) {
        int playerCount = players.Length;
        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerController>().roleName == "No Role") {
                player.GetComponent<PlayerController>().roleName = roles[Random.Range(0,3)].roleName;   
            }
        }
    }
    
    public Color GetColor(string roleName) {
        if (roleName == "Innocent")  return roles[0].roleColor;
        if (roleName == "Traitor")   return roles[1].roleColor;
        if (roleName == "Detective") return roles[2].roleColor;
        return new Color(0, 0, 0);
    }
}
