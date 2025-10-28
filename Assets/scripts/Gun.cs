using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Gun : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject bullet;


    [Header("GunProperty")]
    [SerializeField] float shootForce;              // 発射の威力
    [SerializeField] float timeBetweenShooting;     // 発射レート（連射武器用）
    [SerializeField] float timeBetweenShots;        // 発射レート（ショットガン用）
    [SerializeField] float spread;                  // 散弾具合（ショットガン用）
    [SerializeField] float reloadTime;              // リロード時間
    [SerializeField] int magazineSize;              // マガジンのサイズ
    [SerializeField] int bulletsPerTap;             // 一発当たりの弾の数
    [SerializeField] bool allowButtonHold;          //連射武器か単発武器かのフラグ


    [Header("Reference")]
    //public GameObject bulletPrefab;         // 弾プレハブ
    //public Transform muzzle;                // 銃口
    public Player1 player;                  // プレイヤー参照
    [SerializeField] LayerMask ignoreLayer;         // 無視していいレイヤー

    GameObject playerCam;
    int bulletsShot, bulletsLeft;
    bool shooting, readyToShoot;
    public bool reloading;
    public bool allowInvoke = true;
    public bool isEquipped = false; // この銃が現在装備中なら true


    void Start()
    {
        playerCam = GameObject.Find("Main Camera");
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEquipped) return;  // 持っていない銃は動かさない
        InputHandler();
    }

    private void InputHandler()
    {
        if (player != null && !player.isAiming)
        {
            return;
        }
        // 長押しできるかできないかで入力処理を切り替える
        if (allowButtonHold)
            shooting = Input.GetMouseButton(0);
        else
            shooting = Input.GetMouseButtonDown(0);

        // 打てる状態なのかをチェック
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }

        // リロードする
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // 弾がなくなったら自動的にリロード
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            Reload();
        }
    }

    void Shoot()
    {
        readyToShoot = false;

        // 画面の中央にレイを飛ばす
        Ray ray = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, 1000f, ~ignoreLayer))// レイが何かに当たったかをがチェック
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(10);// 何にも当たらなかったらレイの長さを強制決定

        // 銃口から見たターゲットの方向を取得
        Vector3 directionWithoutSpread = targetPoint - shootPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // 散弾する銃の銃口から見たターゲットの方向を取得
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // 弾を生成
        GameObject currentBullet = Instantiate(bullet, shootPoint.position, Quaternion.identity);

        // 弾を前方に向かせる
        currentBullet.transform.forward = directionWithSpread.normalized;

        // 弾に力を加える
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        bulletsLeft--;
        bulletsShot++;

        // 弾と弾に間隔を開ける
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
        // 一度に出す弾
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    // 撃てる状態にする
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    // リロード処理
    private void Reload()
    {
        reloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    // リロード状態を終了
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    
}
