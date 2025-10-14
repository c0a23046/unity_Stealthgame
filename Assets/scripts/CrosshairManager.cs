using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public GameObject crosshair; //CrosshairUI(Canvas内の親オブジェクト）
    public Player1 player; // プレイヤーをInspectorでドラッグして紐付け
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && crosshair != null)
        {
            crosshair.SetActive(player.isAiming);
            Debug.Log("isAiming: " + player.isAiming + " / Crosshair active: " + crosshair.activeSelf);
        }
        else
        {
            Debug.LogWarning("参照が設定されていません！");
        }
    }
}
