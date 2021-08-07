using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrabedObject : MonoBehaviour {

    Rigidbody2D rig;
    Rigidbody2D graber;

    GrabedObject nextGrabed;
    private void Awake() {
        rig = GetComponent<Rigidbody2D>();
    }

    public bool IsGrab { get; private set; }

    public void GrabMove(float move) {
        var g1 = rig.velocity;
        g1.x = move;

        if (graber != null) {
            var g2 = graber.velocity;
            g2.x = move;

            if (rig.position.x > graber.position.x && move < 0 ||
                rig.position.x < graber.position.x && move > 0) {
                g1.x *= 1.5f;
            }
            if (rig.position.x > graber.position.x && move > 0 ||
                rig.position.x < graber.position.x && move < 0) {
                nextGrabed?.GrabMove(move);
            }
            graber.velocity = g2;
        }

        rig.velocity = g1;
    }
    public void GrabBegin(Rigidbody2D transform) {
        GrabEnd();

        graber = transform;
        IsGrab = true;

        rig.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    public void GrabEnd() {
        IsGrab = false;
        graber = null;
        rig.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!IsGrab || graber == null) {
            return;
        }
        var g = collision.gameObject.GetComponent<GrabedObject>();
        if (g != null) {
            nextGrabed = g;
            nextGrabed.GrabBegin(null);
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
        var g = collision.gameObject.GetComponent<GrabedObject>();
        if (g != null && nextGrabed == g) {
            nextGrabed.GrabEnd();
            nextGrabed = null;
        }
    }
}
