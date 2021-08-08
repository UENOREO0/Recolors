using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GroundChecker))]
[RequireComponent(typeof(ControllColor))]
public class Player : MonoBehaviour {
    [SerializeField]
    float speed = 10;

    [Tooltip("大きくするとキビキビ動く（大きすぎるとバグる）")]
    [SerializeField]
    float speedFollowing = 30;

    [SerializeField]
    float jump = 5;

    [SerializeField]
    float waterJump = 3;


    RecolorsInputAction inputActions;
    Rigidbody2D rigid;

    GroundChecker groundChecker;

    GameObject foot;

    bool isColor = false;
    ColorManager.Color_Type current;

    ColorManager manager;
    Vector3 respawnPos;
    Vector3 camera_respawnPos;

    Animator animator;

    // 服だけの色情報を管理するリスト
    List<Renderer> list_renderersForClothes;

    // 

    GrabedObject grabedObject;

    [SerializeField]
    float abilityDuration = 5;
    [SerializeField]
    float abilityCoolDown = 2;

    Coroutine abilityDurationCo = null;
    Coroutine abilityCoolDownCo = null;

    float yScale;

    bool lastIsGround = false;

    void Awake() {
        animator = GetComponentInChildren<Animator>();
        yScale = transform.localScale.y;

        respawnPos = transform.position;
        camera_respawnPos = Camera.main.transform.position;

        isColor = false;

        foot = transform.GetChild(0).gameObject;

        rigid = GetComponent<Rigidbody2D>();
        groundChecker = GetComponent<GroundChecker>();


        // リストに追加
        list_renderersForClothes = new List<Renderer>();

        list_renderersForClothes.Add(transform.Find("体").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("スカート部分").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("右腕").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("左腕").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("右袖").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("右足").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("三角布").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("左腕").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("左袖").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("左足").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("裏の髪").gameObject.GetComponent<Renderer>());
        list_renderersForClothes.Add(transform.Find("髪").gameObject.GetComponent<Renderer>());

        inputActions = new RecolorsInputAction();

        inputActions.Player.Jump.started += JumpStarted;
        inputActions.Player.UseAbility.started += UseAbilityStarted;
        inputActions.Player.UseAbility.canceled += UseAbilityCanceled;
        inputActions.Player.Grab.started += GrabStarted;
        inputActions.Player.Grab.canceled += GrabCanceled; ;

        con_color = GetComponent<ControllColor>();
        con_color.SetInputActions(inputActions);
    }

    private void GrabCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        grabedObject?.GrabEnd();
    }

    private void GrabStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        grabedObject?.GrabBegin(GetComponent<Rigidbody2D>());
    }

    private void Start() {
        manager = GameObject.Find("GameMaster").GetComponent<ColorManager>();
    }

    private void UseAbilityStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (!isColor|| abilityCoolDownCo!=null) {
            return;
        }
        switch (current) {
            case ColorManager.Color_Type.Blue:
                var scale = transform.localScale;
                scale.y = yScale/3;
                transform.localScale = scale;
                break;
            case ColorManager.Color_Type.Red:
                break;
            case ColorManager.Color_Type.Yellow:
                abilityCoolDownCo = StartCoroutine(AbilityCoolDown());
                return;

            case ColorManager.Color_Type.c_Max:
                break;
            default:
                break;
        }
        abilityDurationCo = StartCoroutine(AbilityDuration());
    }
    private void UseAbilityCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (!isColor || abilityCoolDownCo != null) {
            return;
        }
        switch (current) {
            case ColorManager.Color_Type.Blue:
                var scale = transform.localScale;
                scale.y = yScale;
                transform.localScale = scale;
                break;
            case ColorManager.Color_Type.Red:
                break;
            case ColorManager.Color_Type.Yellow:
                break;

            case ColorManager.Color_Type.c_Max:
                break;
            default:
                break;
        }
        if (abilityDurationCo!=null) {
            StopCoroutine(abilityDurationCo);
            abilityDurationCo = null;
        }

        abilityCoolDownCo = StartCoroutine(AbilityCoolDown());
    }

    IEnumerator AbilityDuration() {
        float startTime = Time.time;
        float currentTime = Time.time;

        while (currentTime - startTime < abilityDuration) {
            currentTime = Time.time;



            yield return null;
        }

        UseAbilityCanceled(new UnityEngine.InputSystem.InputAction.CallbackContext());
        abilityDurationCo = null;
    }
    IEnumerator AbilityCoolDown() {
        float startTime = Time.time;
        float currentTime = Time.time;

        while (currentTime - startTime < abilityCoolDown) {
            currentTime = Time.time;



            yield return null;
        }

        abilityCoolDownCo = null;
    }


    void OnDisable() => inputActions.Disable();
    void OnDestroy() => inputActions.Disable();

    void OnEnable() => inputActions.Enable();


    void Update() {
        //下にすり抜ける用
        var value = inputActions.Player.Move.ReadValue<Vector2>();
        var active = value.y > -0.8f;

        if (foot.activeSelf != active) {
            foot.SetActive(active);
        }

        if (!groundChecker.IsGround&& lastIsGround) {
            animator.SetBool("jump", true);
        }
        if (groundChecker.IsGround&& !lastIsGround) {
            animator.SetBool("jump", false);
        }

        lastIsGround = groundChecker.IsGround;
    }

    //ジャンプ
    private void JumpStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (groundChecker.IsGround) {
            var j = (current == ColorManager.Color_Type.Blue && abilityDurationCo != null) ?  waterJump : jump;

            rigid.AddForce(new Vector2(0, j), ForceMode2D.Impulse);

        }
    }


    void FixedUpdate() {
        //横移動
        var value = inputActions.Player.Move.ReadValue<Vector2>();

        animator.SetFloat("speed", Mathf.Abs(value.x));

        if (grabedObject!=null&& grabedObject.IsGrab) {
            grabedObject.GrabMove(value.x );
            return;
        }
        var move = new Vector2(value.x * speed, 0);

        var moveForce = speedFollowing * (move - rigid.velocity);
        moveForce.y = 0;

        rigid.AddForce(moveForce);

        // 体の向きを買える
        if (value.x > 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (value.x < -0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void Death(ColorManager.Color_Type type) {
        if (type == ColorManager.Color_Type.Blue && type == current && abilityDurationCo != null) {
            return;
        }

        transform.position = respawnPos;
        Camera.main.transform.position = camera_respawnPos;

        current = type;
        manager.TurnMonochrome(current);

        // 服の色を変更
        for (var i = 0; i < list_renderersForClothes.Count; ++i)
        {
            list_renderersForClothes[i].material.color = ColorManager.GetOriginalColor(current);
        }

        isColor = true;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        var colorObj = collision.GetComponent<ColorObject>();
        if (colorObj != null) {
            var type = colorObj.GetColorType();

            switch (type) {
                case ColorManager.Color_Type.Blue:

                    current = type;
                    manager.TurnMonochrome(current);

                    isColor = true;
                    con_color.SetColorActiveState(ColorManager.Color_Type.Blue, true);

                    collision.GetComponent<Collider2D>().isTrigger = false;
                    Death(ColorManager.Color_Type.Blue);

                    break;
                case ColorManager.Color_Type.Red:

                    if(current== ColorManager.Color_Type.Red)
                    {

                    }
                    else // 死ぬとき
                    {
                        current = type;
                        manager.TurnMonochrome(current);

                        isColor = true;
                        con_color.SetColorActiveState(current, true);

                        collision.GetComponent<Collider2D>().isTrigger = false;
                        Death(current);
                    }

                    break;
                case ColorManager.Color_Type.Yellow:

                    break;
            }
            
        }

        grabedObject ??= collision.GetComponent<GrabedObject>();
    }

    private void OnTriggerExit2D(Collider2D collision) {
            if (grabedObject == collision.GetComponent<GrabedObject>()) {
                grabedObject?.GrabEnd();
                grabedObject = null;
            }
        }
    

    // ControllColor　から呼び出し /////////////////////////////////////
    ControllColor con_color;

    public RecolorsInputAction GetInputAction()
    {
        return inputActions;
    }

    // 色を設定
    public void SetPlayerColor(ColorManager.Color_Type c_type)
    {
        current = c_type;

        // 服の色を変更
        for (var i = 0; i < list_renderersForClothes.Count; ++i)
        {
            list_renderersForClothes[i].material.color = ColorManager.GetOriginalColor(c_type);
        }
    }
}
