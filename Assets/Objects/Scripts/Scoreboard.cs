using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static partial class GameConstants {
   public const float COUNTDOWN_START_VAL = 3f;
}


public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    public TextMeshPro CountdownText;

    [SerializeField]
    public TextMeshPro CPUScoreLabel;
    [SerializeField]
    public TextMeshPro PlayerScoreLabel;
    
    [SerializeField]
    public TextMeshPro CPUScoreText;
    [SerializeField]
    public TextMeshPro PlayerScoreText;

    private float _countdown = 0f;
    public float countdown {
        get { return _countdown; }
        set {
            _countdown = value;
            if(_countdown <= 0) {
                CountdownText.SetText("Start!"); 
            } else {
                CountdownText.SetText("{0}", Mathf.Ceil(_countdown));
            }
        }
    }
    private int _cpuscore = 0;
    public int cpuscore {
        get { return _cpuscore; }
        set {
            _cpuscore = value;
            CPUScoreText.SetText("{0}", _cpuscore);
        }
    }
    private int _playerscore = 0;
    public int playerscore {
        get { return _playerscore; }
        set {
            _playerscore = value;
            PlayerScoreText.SetText("{0}", _playerscore);
        }
    }


    private void Awake() {
        this.Reset();
    }
    public void Reset() {
        _countdown = GameConstants.COUNTDOWN_START_VAL;
        CountdownText.gameObject.SetActive(true);
        CountdownText.SetText("Get Ready");
        CPUScoreLabel.gameObject.SetActive(false);
        PlayerScoreLabel.gameObject.SetActive(false);
        CPUScoreText.gameObject.SetActive(false);
        PlayerScoreText.gameObject.SetActive(false);
    }
    public void ShowScore() {
        CountdownText.gameObject.SetActive(false);    
        CPUScoreLabel.gameObject.SetActive(true);
        PlayerScoreLabel.gameObject.SetActive(true);
        PlayerScoreText.gameObject.SetActive(true);
        CPUScoreText.gameObject.SetActive(true);
    }
    public bool isScoreActiveAndEnabled() {
        return !CountdownText.isActiveAndEnabled;
    }


}
