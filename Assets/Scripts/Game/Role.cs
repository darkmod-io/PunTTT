using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    public string roleName;
    [SerializeField] Color roleColor;
    public Color GetColor() {
        return this.roleColor;
    }
}
