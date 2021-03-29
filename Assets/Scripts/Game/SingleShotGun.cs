using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotGun : Gun
{
    PhotonView PV;
    [SerializeField] Camera cam;
    [SerializeField] Animator animator;
    public PhotonAnimatorView PAV;

    void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public override void Use() {
        if (((GunInfo) itemInfo).currentAmmo > 0) {
            if (((GunInfo) itemInfo).currentAmmo == 1) ((GunInfo) itemInfo).nextTimeToFire = 1f;
            Shoot();   
        }
        else {
            Reload();
        }
    }

    void Shoot() {
        ((GunInfo) itemInfo).currentAmmo--;
        RefreshAmmoDisplay(((GunInfo) itemInfo).currentAmmo, ((GunInfo) itemInfo).maxAmmo);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) itemInfo).damage);
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }
    public override void Reload() {
        StartCoroutine(Reloading(((GunInfo) itemInfo).reloadTime));       
    }
    IEnumerator Reloading(float reloadTime)
    {
        ((GunInfo) itemInfo).isReloading = true;

        animator.SetBool("isReloading", true);
        Debug.Log("Started reloading");

        yield return new WaitForSeconds(reloadTime - 0.5f);
        animator.SetBool("isReloading", false);
        
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Finished reloading");
        
        ((GunInfo) itemInfo).currentAmmo = ((GunInfo) itemInfo).maxAmmo; 
        ((GunInfo) itemInfo).isReloading = false;
        RefreshAmmoDisplay(((GunInfo) itemInfo).currentAmmo, ((GunInfo) itemInfo).maxAmmo);
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal) {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        GameObject bulletImpact;
        if (colliders.Length > 0) {

            if (colliders[0].tag != "Player")
                bulletImpact = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            else 
                bulletImpact = Instantiate(bulletImpactPrefabPlayer, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefabPlayer.transform.rotation);

            bulletImpact.transform.SetParent(colliders[0].transform);
            Destroy(bulletImpact, 3f);  
        }
    }
}
