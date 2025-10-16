using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public GameObject enemy;  
    public float moveSpeed; //移動速度
    public float moveTime; //移動する時間
    public float waitTime;
    public Animator EnemyAnimator;

    bool isWalk; //歩くアニメーション


    // Start is called before the first frame update
    void Start()
    {
        isWalk = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ptorol();
    }
    
    void Ptorol()
    {
        isWalk = true;
        transform.Translate(0f, 0f, moveSpeed);
        EnemyAnimator.SetBool("walk", isWalk);
    }

}
