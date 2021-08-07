using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // 各オブジェクトに変数を作成して設定
    public enum Color_Type
    {
        Blue = 0,
        Red,
        Yellow,
        c_Max
    };

    // 色の個数分リストを作成
    List<ColorObject>[] array_listColors;

    // それぞれの色情報
    static Color32[] array_MaterialColors = new Color32[(int)Color_Type.c_Max];
    static bool isInit = false;

    // 白色
    static Color32 white_MaterialColor;

    // Start is called before the first frame update
    void Start()
    {
        // それぞれの色のリストを初期化
        array_listColors = new List<ColorObject>[(int)Color_Type.c_Max];

        for (var i = 0; i < (int)Color_Type.c_Max; ++i)
        {
            array_listColors[i] = new List<ColorObject>();
        }

        // 色付きのオブジェクトを探して各リストに格納
        var list_GameObject = new List<ColorObject>();
        list_GameObject.AddRange(GameObject.FindObjectsOfType<ColorObject>());

        for (var i = 0; i < (int)Color_Type.c_Max; ++i)
        {
            for (var j = 0; j < list_GameObject.Count; ++j)
            {
                // 色のタイプを取得、比較
                if (i == (int)list_GameObject[j].GetColorType())
                {
                    // リストに追加
                    array_listColors[i].Add(list_GameObject[j]);
                }
            }

            Debug.Log("" + array_listColors[i].Count);
        }

    }

    static void ColorInit()
    {
        // 既定の色を読み込み
        for (var i = 0; i < (int)Color_Type.c_Max; ++i)
        {
            Color_Type temp = (Color_Type)i;
            array_MaterialColors[i] = Resources.Load<Material>("Materials/" + temp.ToString()).color;
        }

        white_MaterialColor = Resources.Load<Material>("Materials/White").color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 既定の色を取得
    static public Color32 GetOriginalColor(Color_Type color)
    {
        // 初期化済みかチェック
        if (isInit == false)
        {
            ColorInit();

            isInit=true;
        }

        // 取得したい色
        var c_num = (int)color;

        // iは色に対応する数字
        for (var i = 0; i < (int)Color_Type.c_Max; ++i)
        {
            if (c_num == i)
            {
                return array_MaterialColors[i];
            }
        }

        // 失敗時には0番を返却
        return array_MaterialColors[0];
    }

    static public Color32 GetWhite()
    {
        return white_MaterialColor;
    }
    static public Color32 GetOff()
    {
        return new Color(0.2f,0.2f,0.2f,1);
    }

    // 特定の色を白黒に変更する処理
    public void TurnMonochrome(Color_Type color)
    {
        // 白黒に変更する色
        var c_num = (int)color;

        // iは色に対応する数字
        for (var i = 0; i < (int)Color_Type.c_Max; ++i)
        {
            if (c_num == i)
            {
                for (var j = 0; j < array_listColors[i].Count; ++j)
                {
                    array_listColors[i][j].TurnOffColor();
                }

                break;
            }
        }
    }

    // 色を復活させる処理
    public void RecoverColor(Color_Type color)
    {
        // 復活させる色
        var c_num = (int)color;

        // iは色に対応する数字
        for (var i = 0; i < (int)Color_Type.c_Max; ++i)
        {
            if (c_num == i)
            {
                for (var j = 0; j < array_listColors[i].Count; ++j)
                {
                    array_listColors[i][j].TurnOnColor();
                }

                break;
            }
        }
    }
}
