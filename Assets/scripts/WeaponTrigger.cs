using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    private WeaponPickUp parentWeapon;

    private void Awake()
    {
        // 親オブジェクトに WeaponPickUp がついているか確認
        parentWeapon = GetComponentInParent<WeaponPickUp>();
        if(parentWeapon == null)
        {
            Debug.LogError("WeaponTrigger: 親に WeaponPickUp が見つかりません");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && parentWeapon != null)
        {
            parentWeapon.playerInRange = true;
            Debug.Log("プレイヤーが武器範囲に入りました");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && parentWeapon != null)
        {
            parentWeapon.playerInRange = false;
            Debug.Log("プレイヤーが武器範囲から出ました");
        }
    }
}
