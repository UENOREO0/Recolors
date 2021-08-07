using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 色を切り替えたりするクラス、UIとのやり取りもします
/// </summary>
public class ControllColor : MonoBehaviour
{
    ColorManager color_manager;

    // キャンバス
    GameObject canvas;

    // UIサークル用の情報
    GameObject[] Circles;
    GameObject[] MonoCircles;
    Vector3[] c_SettingPos;

    Vector3 Vanish_Pos;

    // 選択時のフォーカス画像
    GameObject Focus_Circle;
    Vector3 Pos_modify_focus;

    // サークル表示時間
    float Time_circlesShow;
    bool isShowing = false;

    // 色を持っているか持ってないか
    bool[] isHaving;

    // 現在の能力の指標
    int index_CurPow = -1;

    public void SetInputActions(RecolorsInputAction inp)
    {
        inp.Player.SwitchAbility.started += SwitchAbilityStarted;
    }

    // Start is called before the first frame update
    void Start()
    {
        // マネージャーを取得
        color_manager = GameObject.FindObjectOfType<ColorManager>();

        // ブール初期化
        isHaving = new bool[(int)ColorManager.Color_Type.c_Max];

        // サークル初期化
        Vanish_Pos = new Vector3(1000f, 1000f, 0);

        Circles = new GameObject[(int)ColorManager.Color_Type.c_Max];
        MonoCircles = new GameObject[(int)ColorManager.Color_Type.c_Max];
        c_SettingPos = new Vector3[(int)ColorManager.Color_Type.c_Max];

        GameObject temp_g = Resources.Load<GameObject>("Sprites/Monochrome_g");

        canvas = GameObject.Find("Canvas");

        for (var i = 0; i < (int)ColorManager.Color_Type.c_Max; ++i)
        {
            isHaving[i] = false;

            ColorManager.Color_Type temp = (ColorManager.Color_Type)i;
            Circles[i] = canvas.transform.Find(temp.ToString() + "Circle").gameObject;

            // サークルがある場合
            if (Circles[i] != null)
            {
                c_SettingPos[i] = new Vector3(Circles[i].transform.position.x - canvas.transform.position.x,
                    Circles[i].transform.position.y, Circles[i].transform.position.z);

                // 最初は白黒を配置する
                MonoCircles[i] = Instantiate<GameObject>(temp_g);
                MonoCircles[i].transform.parent = canvas.transform;

                MonoCircles[i].transform.position = Vanish_Pos;
                Circles[i].transform.position = Vanish_Pos;
            }
            else
            {
                MonoCircles[i] = Instantiate<GameObject>(temp_g, Vanish_Pos, Quaternion.identity);
            }
        }

        Focus_Circle = canvas.transform.Find("Focus").gameObject;

        Pos_modify_focus = new Vector3(Focus_Circle.transform.position.x - canvas.transform.position.x
            - c_SettingPos[0].x, Focus_Circle.transform.position.y - c_SettingPos[0].y,
            Focus_Circle.transform.position.z);

        // 遠くへ飛ばす
        Focus_Circle.transform.position = Vanish_Pos;
    }

    // Update is called once per frame
    void Update()
    {
        VanishCircles();

    }

    private void VanishCircles()
    {
        if (isShowing)
        {
            // 表示を消す処理
            Time_circlesShow -= Time.deltaTime;

            if (Time_circlesShow <= 0f)
            {
                isShowing = false;

                Time_circlesShow = 0f;

                // フォーカスサークルを消す
                Focus_Circle.transform.position = Vanish_Pos;

                // 各サークルを消す
                for (var k = 0; k < (int)ColorManager.Color_Type.c_Max; ++k)
                {
                    if (Circles[k] != null)
                    {
                        if (isHaving[k])
                        {
                            Circles[k].transform.position = Vanish_Pos;
                        }
                        else
                        {
                            MonoCircles[k].transform.position = Vanish_Pos;

                        }
                    }
                }
            }
        }
    }


    public void SetColorActiveState(ColorManager.Color_Type col,bool state)
    {
        isHaving[(int)col] = state;
    }
    
    private void SwitchAbilityStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // 関数が呼ばれているかチェック
        //Debug.Log("SwtichAbility Working");
        
        // 能力を持っているか判定
        var fal_num = 0;

        for (var i = 0; i < (int)ColorManager.Color_Type.c_Max; ++i)
        {
            fal_num++;

            if (isHaving[i])
            {
                break;
            }
        }

        // 能力を一つも持ってない場合
        if (fal_num == (int)ColorManager.Color_Type.c_Max)
        {
            return;
        }
        else
        {
            // サークルの表示処理
            isShowing = true;

            Time_circlesShow = 2f;

            for (var k = 0; k < (int)ColorManager.Color_Type.c_Max; ++k)
            {
                if (Circles[k] != null)
                {
                    if (isHaving[k])
                    {
                        Circles[k].transform.position = new Vector3(c_SettingPos[k].x + canvas.transform.position.x,
                            c_SettingPos[k].y, c_SettingPos[k].z);

                        MonoCircles[k].transform.position = Vanish_Pos;
                    }
                    else
                    {
                        Circles[k].transform.position = Vanish_Pos;

                        MonoCircles[k].transform.position = new Vector3(c_SettingPos[k].x + canvas.transform.position.x,
                            c_SettingPos[k].y, c_SettingPos[k].z);
                    }
                }
            }

            // 色の切り替え処理
            for (var j = 0; ; j++)
            {
                // 能力の切り替え処理
                index_CurPow++;

                if (index_CurPow >= (int)ColorManager.Color_Type.c_Max)
                {
                    index_CurPow = 0;
                }

                // インデックスに対応する色を持っていたらブレーク
                if (isHaving[index_CurPow])
                {
                    GetComponent<Player>().SetPlayerColor((ColorManager.Color_Type)index_CurPow);

                    // フォーカスオブジェクトを移動させる
                    Focus_Circle.transform.position = new Vector3(c_SettingPos[index_CurPow].x + canvas.transform.position.x
                        + Pos_modify_focus.x, c_SettingPos[index_CurPow].y + Pos_modify_focus.y, Pos_modify_focus.z);

                    break;
                }

                // 無限ループ対策
                if (j > (int)ColorManager.Color_Type.c_Max)
                {
                    break;
                }
            }

        }
    }
}
