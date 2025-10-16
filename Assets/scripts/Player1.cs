using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player1 : MonoBehaviour
{
    public float PlayerSpeed; //プレイヤーの移動速度を管理する変数
    public float CrouchSpeed; //プレイヤーのしゃがみ歩き時の移動速度を管理する変数
    public float RotationSpeed; //プレイヤーの視界速度を管理する変数
    public Transform Camera; //MainCamera
    public Transform AimCamera; //構えカメラの位置
    public float camSmooth = 10f; //通常カメラから構えカメラまでの補完速度
    Vector3 speed = Vector3.zero;
    Vector3 rot = Vector3.zero;
    public Animator PlayerAnimator;
    bool isRun; //走り状態を管理する変数
    bool isCrouch; //しゃがみ状態を管理する変数
    bool isTakingItem; // 武器取得中フラグ
    bool ishas_Weapon_pistol; //ハンドガンを所持中を管理する変数
    public bool isAiming; //銃を構える状態を管理する変数
    bool isPistol_crouch; //銃をしゃがみ状態時に構える状態を管理する変数
    bool isPistol_crouch_Aiming; //しゃがみ歩き時、銃を構える状態を管理する変数
    private GameObject weaponInRange;  // プレイヤーが触れている武器
    private GameObject equippedWeapon; // 装着済みの武器

    private List<GameObject> weapons = new List<GameObject>(); //武器を格納するリスト
    private int currentWeaponIndex = -1; //現在の武器（-1は未装備）
    [SerializeField] private float mouseSensitivity = 1000f; 
    //[SerializeField] private float keyRotationSpeed = 100f;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Move();
        Rotation();
        Crouch();
        CrouchAim();
        GetWeapon();
        Pistol_crouch_Aiming();



        // ↓ここに入れる
        AnimatorStateInfo state = PlayerAnimator.GetCurrentAnimatorStateInfo(0);
        if (isTakingItem && state.IsName("TakingItem") && state.normalizedTime >= 1f)
        {
            isTakingItem = false;
            //ishas_Weapon_pistol = true;
            PlayerAnimator.SetBool("TakingItem", false);
            //PlayerAnimator.SetBool("has_Weapon_pistol", ishas_Weapon_pistol);
        }

        HandleAim();

        // --- カメラ補間 ---
        //銃を構えている間
        if (isAiming)
        {
            // 肩越しカメラに滑らかに移動
            //Vector3.Lerp(a, b, t) は a から b に t の割合で補間する
            Camera.position = Vector3.Lerp(Camera.position, AimCamera.position, camSmooth * Time.deltaTime);
            //回転も同じように、現在の回転から AimCamera の回転まで補間
            Camera.rotation = Quaternion.Lerp(Camera.rotation, AimCamera.rotation, camSmooth * Time.deltaTime);
        }
        else
        {
            // 通常カメラに戻す
            Camera.position = Vector3.Lerp(Camera.position, transform.position, camSmooth * Time.deltaTime);
            Camera.rotation = Quaternion.Lerp(Camera.rotation, Quaternion.Euler(0, Camera.eulerAngles.y, 0), camSmooth * Time.deltaTime);
        }

        //武器の切り替え（キー 1, 2, 3）
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }


    }

    void Move()
    {
        speed = Vector3.zero;
        rot = Vector3.zero;
        isRun = false;


        if (Input.GetKey(KeyCode.W))
        {
            rot.y = 0;
            MoveSet();
        }
        if (Input.GetKey(KeyCode.S))
        {
            rot.y = 180;
            MoveSet();
        }
        if (Input.GetKey(KeyCode.D))
        {
            rot.y = 90;
            MoveSet();

        }
        if (Input.GetKey(KeyCode.A))
        {
            rot.y = -90;
            MoveSet();
        }



        PlayerAnimator.SetBool("run", isRun && !isCrouch);
        PlayerAnimator.SetBool("crouchwalk", isCrouch && isRun); // しゃがみ歩き
        PlayerAnimator.SetBool("TakingItem", isTakingItem);

        // 新規: 構え状態での走り
        if (ishas_Weapon_pistol && isAiming && isRun)
        {
            PlayerAnimator.SetBool("pistol_run", true);
        }
        else
        {
            PlayerAnimator.SetBool("pistol_run", false);
        }
    }

    void MoveSet()
    {

        isRun = true;

        float moveSpeed = isCrouch ? CrouchSpeed : PlayerSpeed;

        //isAimingがTrue(銃を構えている時)
        if (isAiming)
        {
            //移動方向を格納するための変数を初期化，最初は(0, 0, 0)
            Vector3 moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                //Wキーが押されたらカメラの前方向を移動方向に加える
                //つまりプレイヤーが「カメラが向いている方向に前進」
                moveDir += Camera.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir -= Camera.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDir += Camera.right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDir -= Camera.right;
            }
            //平面上だけの移動に制限
            moveDir.y = 0;
            //移動方向ベクトルを「長さ１」にする。
            moveDir.Normalize();
            //実際にキャラを移動させる
            //moveDir（方向）* movespeed（移動速度) * Time.deltaTime(フレーム)
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            // 向きはカメラに合わせて固定
            //Quaternion.Euler(0, Camera.eulerAngles.y, 0) = Y軸だけカメラと同じ角度にする
            transform.rotation = Quaternion.Euler(0, Camera.eulerAngles.y, 0);
        }
        else
        {
            // --- 通常時は今まで通りキャラの向きも変える ---
            transform.Translate(0f, 0f, moveSpeed * Time.deltaTime);
            transform.eulerAngles = Camera.transform.eulerAngles + rot;
        }

    }

    void Rotation()
    {
        if (isAiming)
        {
            // 🎯 エイム中 → マウス操作
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


            // 横回転（キャラのY軸回転）
            transform.Rotate(Vector3.up * mouseX);

            // 縦回転（カメラの上下のみ）
            Vector3 camEuler = Camera.eulerAngles;
            camEuler.x -= mouseY;
            // 上下の回転制限（例: -30°〜60°）
            camEuler.x = Mathf.Clamp((camEuler.x > 180) ? camEuler.x - 360 : camEuler.x, -90f, 90f);
            camEuler.x = (camEuler.x < 0) ? camEuler.x + 360 : camEuler.x; // Clamp調整
            Camera.eulerAngles = new Vector3(camEuler.x, Camera.eulerAngles.y, 0f);
        }
        else
        {
            var speed = Vector3.zero;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                speed.y = -RotationSpeed;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                speed.y = RotationSpeed;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                speed.x = RotationSpeed;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                speed.x = -RotationSpeed;
            }


            Camera.transform.eulerAngles += speed; //自分の回転にspeedを加算
        }

    }


    // 武器を手に装着する処理は
    // 「拾うモーション → 立ち姿」への遷移直後に呼び出したいので
    // StateMachineBehaviour を使うのがきれい

    void TryPickupWeapon()
    {
        //シーン内に存在する WeaponPickUp コンポーネントを全部探して配列に格納します。
        WeaponPickUp[] weaponPickups = FindObjectsOfType<WeaponPickUp>();
        //for文で一つずつ確認
        foreach (var pickup in weaponPickups)
        {
            //デバッグ用
            Debug.Log(pickup.name + " inRange: " + pickup.playerInRange);
            //プレイヤーが武器の当たり判定内にいるかつまだ武器を拾っていない状態
            if (pickup.playerInRange && !isTakingItem)
            {
                //Trueにしてアイテムを拾うモーションを行う
                isTakingItem = true;
                PlayerAnimator.SetBool("TakingItem", true);
                //プレイヤーの手の位置に配置しているオブジェクト指（WeaponSocket)を探してそのTransformを取得
                Transform socket = GameObject.Find("WeaponSocket").transform;
                GameObject weaponObj = pickup.gameObject;
                weaponObj.transform.SetParent(socket);
                weaponObj.transform.localPosition = Vector3.zero;
                weaponObj.transform.localRotation = Quaternion.identity;
                weaponObj.GetComponent<Collider>().enabled = false;

                equippedWeapon = weaponObj;
                AddWeapon(weaponObj);
                Debug.Log("武器取得成功: " + pickup.name);
                Debug.Log("拾ったオブジェクトのシーン状態: " + weaponObj.scene.name);
            }
        }
        
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouch = !isCrouch; // 押すたびに切り替え
            PlayerAnimator.SetBool("crouch", isCrouch);
        }
    }

    void GetWeapon()
    {
        // 武器取得
        if (Input.GetKeyDown(KeyCode.E) && !isTakingItem)
        {
            TryPickupWeapon();
            ishas_Weapon_pistol = true; // 武器持ち状態にする
            isAiming = false;         // 最初は構えていない
        }

    }

    void HandleAim()
    {
        isAiming = false;
        // 武器を持っていて、かつ拾い中じゃないときだけ構えられる
        if (ishas_Weapon_pistol && !isTakingItem && Input.GetMouseButton(1))
        {

            isAiming = true;
        }

        PlayerAnimator.SetBool("has_Weapon_pistol", isAiming);

    }

    void CrouchAim()
    {
        isPistol_crouch = false;
        //ピストルをもっているかつマウスの右クリックをおしている間
        if (ishas_Weapon_pistol == true && Input.GetMouseButton(1))
        {
            isPistol_crouch = true;
        }

        PlayerAnimator.SetBool("pistol_crouch", isPistol_crouch);
    }

    void Pistol_crouch_Aiming()
    {
        isPistol_crouch_Aiming = false;

        if (ishas_Weapon_pistol == true && Input.GetMouseButton(1) && isRun == true)
        {
            isPistol_crouch_Aiming = true;
        }

        PlayerAnimator.SetBool("pistol_crouch_run", isPistol_crouch_Aiming);
    }

    void AddWeapon(GameObject weaponObj)
    {
        // すでに持っているなら無視
        if (weapons.Contains(weaponObj))
        {
            return;
        }

        // 上限は2個（例）
        if (weapons.Count >= 2)
        {
            GameObject removed = weapons[0];
            weapons.RemoveAt(0);
            Destroy(removed);
        }

        weapons.Add(weaponObj);
        // ← ここで Gun.cs に Player1 をセット
        Gun gunComp = weaponObj.GetComponent<Gun>();
        if (gunComp != null)
        {
            gunComp.player = this;
            Debug.Log("Gun に player をセット: " + weaponObj.name);
        }

        currentWeaponIndex = weapons.Count - 1;
        EquipWeapon(currentWeaponIndex);

        Debug.Log("武器追加: " + weaponObj.name);
    }

    void SwitchWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
        {
            return;
        }

        currentWeaponIndex = index;
        EquipWeapon(currentWeaponIndex);
    }

    void EquipWeapon(int index)
    {
        Debug.Log("EquipWeapon呼ばれた index=" + index + " weapons.Count=" + weapons.Count);
        for (int i = 0; i < weapons.Count; i++)
        {
            bool active = (i == index);
            weapons[i].SetActive(active);
            Debug.Log("EquipWeapon: " + weapons[i].name + " active=" + active);
        }
    }
}
