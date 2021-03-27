using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    PhotonView PV;
    [SerializeField] Camera cam;

    void Awake() {
        PV = GetComponent<PhotonView>();    
    }
    public override void Use() {
        Shoot();   
    }

    void Shoot() {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) itemInfo).damage);
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal) {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.5f);
        GameObject bulletImpact = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
        if (colliders.Length > 0) bulletImpact.transform.SetParent(colliders[0].transform);
        Destroy(bulletImpact, 3f);  
    }
}
