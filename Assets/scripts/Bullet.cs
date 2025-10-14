using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit: " + collision.collider.name);

        // 当たったら消す（敵いなくても壁とかに当たれば消える）
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
