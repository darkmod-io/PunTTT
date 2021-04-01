using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance;
    public float preperationTime;
    Role[] roles;
    ArrayList availableRoles;
    private void Awake() {
        Instance = this;
        roles = GetComponentsInChildren<Role>();
    }
    private void Start() {
        InitRoles();
        StartCoroutine(StartRound(preperationTime));
    }

    IEnumerator StartRound(float prepTime) {
        yield return new WaitForSeconds(prepTime);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        for (int i = 0; i < players.Length; i++) {
            players[i].GetComponent<PlayerController>().role = GetRoleByName((string) availableRoles.ToArray()[i]);
            // players[i].GetComponent<PlayerController>().PV.RPC("RPC_InitializeRole", RpcTarget.All, players[i].GetComponent<PlayerController>().role.roleName);
        }
        players[0].GetComponent<PlayerController>().InitializeRole(availableRoles.ToArray());

    }  

    void InitRoles() {
        availableRoles = new ArrayList();
        availableRoles.Add("Innocent");
        availableRoles.Add("Traitor");
        availableRoles.Add("Innocent");
        availableRoles.Add("Innocent");
        availableRoles.Add("Traitor");
        availableRoles.Add("Detective");
        availableRoles.Add("Traitor");
        availableRoles.Add("Innocent");
        availableRoles.Add("Traitor");
        availableRoles.Add("Innocent");
        availableRoles.Add("Innocent");
        availableRoles.Add("Traitor"); 
    }

    public Role GetRoleByName(string _roleName) {
        if (_roleName == "Innocent")  return roles[0];
        if (_roleName == "Traitor")   return roles[1];
        if (_roleName == "Detective") return roles[2];
        return new Role();
    }
}
