using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    public Slider slider;    

    [SerializeField]
    public Button backButton;

    void Start() {
        backButton.onClick.AddListener(onBackButton);
        slider.value = AudioListener.volume;
        slider.onValueChanged.AddListener( delegate{ onSliderValueChanged(); } );
    }

    public void onSliderValueChanged() {
        AudioListener.volume = slider.value;
    }

    public void onBackButton() {
        this.gameObject.SetActive(false);
    }

}
