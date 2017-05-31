using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class warning : MonoBehaviour {
    Quaternion rotation;
    Vector3 position;

    GameObject[] guards;
    void Start() {
        rotation = transform.rotation;
        guards = GameObject.FindGameObjectsWithTag("guard");
    }

    private void FixedUpdate() {
        foreach (GameObject guard in guards) {
            if (guard.GetComponent<Patrol>().canSeePlayer()) {
                this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                return;
            }
        }
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
    }

    void LateUpdate() {
        transform.rotation = rotation;
        position = GameObject.FindGameObjectWithTag("Player").transform.position;
        position.y += 0.6f;
        transform.position = position;
    }
}
