using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    public Button playButton;

    [SerializeField]
    public Button settingsButton;

    [SerializeField]
    public Button exitButton;

    [SerializeField]
    public GameObject settingsPanel;


    public void Awake() {
        playButton.onClick.AddListener(OnPlayButton);
        settingsButton.onClick.AddListener(OnSettingsButton);
        exitButton.onClick.AddListener(OnExitButton);
        settingsPanel.SetActive(false);
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            settingsPanel.SetActive(false);
        }
    }

    public void OnPlayButton() {
        SceneManager.LoadScene(1);
    }
    public void OnSettingsButton() {
        settingsPanel.SetActive(true);
    }

    public void OnExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
