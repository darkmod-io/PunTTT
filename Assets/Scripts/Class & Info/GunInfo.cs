using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage;
    public float maxAmmo;
    public float currentAmmo;
    public float reloadTime;
    public float fireRate;
    public bool isReloading = false;
}
