using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        collision.gameObject.GetComponent<Player>()?.Death(GetComponent<ColorObject>().GetColorType());

    }
}
