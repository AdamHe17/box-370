using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButton : MonoBehaviour {

    void Start() {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(startGame);
    }

    void startGame() {
        SceneManager.LoadScene("1-0");
    }
}
