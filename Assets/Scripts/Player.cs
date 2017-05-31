using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;

    Animator anim;

    private int treasures;

    private void Start() {
        rb = this.GetComponent<Rigidbody2D>();
        anim = FindObjectOfType<Animator>();
        treasures = GameObject.FindGameObjectsWithTag("treasure").Length;
    }

    private void FixedUpdate() {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        checkInput();
    }

    private void checkInput() {
        if (Input.GetKey(KeyCode.W)) {
            rb.AddForce(Vector2.up * speed);
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.AddForce(Vector2.down * speed);
        }

        if (Input.GetKey(KeyCode.A)) {
            rb.AddForce(Vector2.left * speed);
        }
        else if (Input.GetKey(KeyCode.D)) {
            rb.AddForce(Vector2.right * speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "guard") {
            anim.SetTrigger("gameover");

            Time.timeScale = 0;
        } else if (collision.gameObject.tag == "exit") {
            if (treasures <= 0) {
                if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings) {
                    anim.SetTrigger("gameover");

                    Time.timeScale = 0;
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        } else if (collision.gameObject.tag == "treasure") {
            FindObjectOfType<ScoreManager>().incScore();
            treasures--;
            Destroy(collision.gameObject);
        }
    }
}
