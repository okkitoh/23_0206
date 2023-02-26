using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    [SerializeField]
    Button ExitButton;


    public void Awake() {
        ExitButton.onClick.AddListener(onExitButtonClick);
    }

    public void onExitButtonClick() {
        SceneManager.LoadScene(0);
    }
}
