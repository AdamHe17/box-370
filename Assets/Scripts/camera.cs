using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour {

    private void LateUpdate() {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0, -10f);
    }
}
