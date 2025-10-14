using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform WeaponSocket; //手に置く位置
    private GameObject currentWeapon; //現在装備している武器
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    // 武器を拾ったときに呼ばれる処理
    public void PickUpWeapon(GameObject weapon)
    {
        // Rigidbody無効化
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Collider無効化
        Collider col = weapon.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // WeaponSocket に配置
        weapon.transform.SetParent(WeaponSocket);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        currentWeapon = weapon;
    }
}
