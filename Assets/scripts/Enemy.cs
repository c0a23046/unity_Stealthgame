using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed; //移動速度
    public float moveTime; //移動する時間
    public float waitTime; //待機時間
    public Animator EnemyAnimator;

    GameObject Target; //プレイヤーをターゲットとする
    //bool isWalk; //歩くアニメーション
    bool isMoving = true; //今動いているかどうか

    float timer; //時間を図るタイマー

    // Start is called before the first frame update
    void Start()
    {
        //isWalk = false;
        isMoving = true;
        timer = moveTime; //初めは「移動状態」から開始
    }

    // Update is called once per frame
    void Update()
    {
        //ターゲットが見当たらない時（パトロール中）
        if (Target == null)
        {
            Ptorol();
        }
        else //ターゲットを見つけた時
        {
            ChaseTarget();
        }
    }

    void Ptorol()
    {
        timer -= Time.deltaTime;
        if (isMoving)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            EnemyAnimator.SetBool("walk", true);

            if (timer <= 0f)
            {
                isMoving = false;
                timer = waitTime;
            }
        }
        else
        {
            EnemyAnimator.SetBool("walk", false);

            if (timer <= 0f)
            {
                transform.Rotate(0f, Random.Range(0f, 360f), 0f);
                isMoving = true;
                timer = moveTime;
            }
        }
    }
    
    void ChaseTarget()
    {
        if (Target)
        {
            float chaseSpeed = moveSpeed * 1.5f;
            transform.LookAt(Target.transform);
            transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
            EnemyAnimator.SetBool("walk", true);
        }
    }

    //プレイヤーがエネミーの範囲内に入ったら追跡を行う
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Target = other.gameObject;
        }
    }
    
    //プレイヤーがエネミーの範囲外に出たら追跡をやめる
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Target = null;
        }
    }
}
