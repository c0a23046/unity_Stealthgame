using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float HP;
    public float moveSpeed; //移動速度
    public float moveTime; //移動する時間
    public float waitTime; //待機時間
    public Animator EnemyAnimator;
    public float stopDistance;

    GameObject Target; //プレイヤーをターゲットとする
    //bool isWalk; //歩くアニメーション
    bool isMoving = true; //今動いているかどうか
    bool isRun = true; //追跡中の走り
    bool isShoot = true;//銃を撃っているかどうか

    float timer; //時間を図るタイマー

    // Start is called before the first frame update
    void Start()
    {
        isMoving = true;
        isRun = false;
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
        else
        {
            ChaseTarget();
        }

        if(HP <= 0)
        {
            Destroy(gameObject);
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
        if (Target == null) return;

        // エネミーの追跡速度を上げる
        float chaseSpeed = moveSpeed * 1.5f;

        // プレイヤーとの距離を取得
        float distance = Vector3.Distance(transform.position, Target.transform.position);

        // プレイヤーの方向を見る
        transform.LookAt(Target.transform);

        if (distance > stopDistance)
        {
            // stopDistance より遠いときだけ前進
            transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);
            EnemyAnimator.SetBool("run", true);
            EnemyAnimator.SetBool("shoot", false);
        }
        else
        {
            // 近すぎる場合は止まる
            EnemyAnimator.SetBool("run", false);
            EnemyAnimator.SetBool("shoot", true);
        }
    }


    //プレイヤーがエネミーの範囲内に入ったら追跡を行う
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Target = other.gameObject;
            EnemyAnimator.SetBool("run", true);
        }
    }

    //プレイヤーがエネミーの範囲外に出たら追跡をやめる
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Target = null;
            EnemyAnimator.SetBool("run", false); // ← 追跡終了時に走りを停止
            EnemyAnimator.SetBool("walk", true); // ← パトロール再開（任意）
        }
    }
    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet_handgun"))
        {
            HP--;
        }
        if(col.gameObject.CompareTag("Bullet_asarut"))
        {
            HP -= 2;
        }
    }
}
