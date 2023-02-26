using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField, Min(2), InspectorName("Win Condition Points")]
    int pointsToWin = 3; 

    [SerializeField]
    Ball ball;

    [SerializeField]
    Paddle CPUPaddle, PlayerPaddle;

    [SerializeField]
    Scoreboard scoreboard;

    [SerializeField]
    GameObject pausePanel;

    private bool isPlayable = false;
    private bool isPaused = false;


    void Awake() {
        this.StartNewGame();
    }
    void StartNewGame() {
        StartNewMatch();
        scoreboard.Reset();
    }

    void StartNewMatch() {
        this.isPlayable = false;
        this.isPaused = false;
        this.ball.Reset();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            this.isPaused = !this.isPaused;
            this.pausePanel.SetActive(this.isPaused);
        }

        float dt = Time.deltaTime;
        if(this.isPaused) {
            dt = 0;
        }
        PlayerPaddle.Move(ball, dt);
        CPUPaddle.Move(ball, dt);
        if(isPlayable && !isPaused) {
            if(scoreboard.countdown > -1f) {
                scoreboard.countdown -= dt;
            }
            else if(!scoreboard.isScoreActiveAndEnabled()) {
                scoreboard.ShowScore();
            } else {
                ball.Move(dt);
                if(!ball.CollisionCheck(PlayerPaddle, dt)) {
                    scoreboard.cpuscore++;
                    isPaused = true;
                    StartCoroutine(CO_RefreshPause(0.75f, StartNewMatch));
                }
                if(!ball.CollisionCheck(CPUPaddle, dt)) {
                    scoreboard.playerscore++;
                    isPaused = true;
                    StartCoroutine(CO_RefreshPause(0.75f, StartNewMatch));
                }
            }
        } else if(Input.anyKeyDown) {
            isPlayable = true;
        }
    }
    private IEnumerator CO_RefreshPause(float delay, Action callback) {
        yield return new WaitForSeconds(delay);
        callback();
    }
}
