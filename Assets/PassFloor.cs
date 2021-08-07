using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassFloor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
           transform.parent.GetComponent<Collider2D>().enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            transform.parent.GetComponent<Collider2D>().enabled = true;
        }
    }
}
