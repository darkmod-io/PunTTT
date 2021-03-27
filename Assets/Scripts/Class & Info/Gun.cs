using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public abstract override void Use();
    public GameObject bulletImpactPrefab;
    public GameObject bulletImpactPrefabPlayer;
    public Animator _animator;
    public void Reload() {
        StartCoroutine(Reloading(_animator, ((GunInfo) itemInfo).reloadTime));
        ((GunInfo) itemInfo).currentAmmo = ((GunInfo) itemInfo).maxAmmo;        
    }
    IEnumerator Reloading(Animator animator, float reloadTime)
    {
        ((GunInfo) itemInfo).isReloading = true;
        Debug.Log("Reloading");

        animator.SetBool("isReloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("isReloading", false);
        yield return new WaitForSeconds(.25f);
        
        ((GunInfo) itemInfo).isReloading = false;
    }

}
