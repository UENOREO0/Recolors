using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBox : MonoBehaviour
{
    ColorObject obj;
    void Start()
    {
        obj = GetComponent<ColorObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (obj.GethavingColor()) {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else {
             gameObject.layer = LayerMask.NameToLayer("PlayerNone");
        }
    }
}
