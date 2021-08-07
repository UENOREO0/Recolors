using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour {
    Transform player;
    [SerializeField]
    float range = 1;
    [SerializeField]
    float respawnTime = 3;

    Rigidbody2D rig;
    Vector3 spawnPos;
    void Start() {
        player = GameObject.FindObjectOfType<Player>().transform;

        rig = GetComponent<Rigidbody2D>();
        spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (Mathf.Abs(player.position.x - transform.position.x) < range) {
            rig.simulated = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision) {
        collision.gameObject.GetComponent<Player>()?.Death(GetComponent<ColorObject>().GetColorType());
        gameObject.SetActive(false);
        rig.simulated = false;
        player.GetComponent<MonoBehaviour>().StartCoroutine(Respawn());
    }
    IEnumerator Respawn() {
        yield return new WaitForSeconds(respawnTime);
        gameObject.SetActive(true);
        transform.position = spawnPos;
    }
}
