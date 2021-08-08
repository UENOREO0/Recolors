using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Player>()?.Death(GetComponent<ColorObject>().GetColorType());
    }
}
