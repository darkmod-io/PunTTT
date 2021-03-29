using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Gun : Item
{
    public abstract override void Use();
    public abstract void Reload();
    public GameObject bulletImpactPrefab;
    public GameObject bulletImpactPrefabPlayer;
    public Text ammoText;

    public void RefreshAmmoDisplay(float currentAmmoAmount, float maxAmmoAmount) {
        ammoText.text = currentAmmoAmount.ToString() + "/" + maxAmmoAmount.ToString() + " Ammo";
    }
}
