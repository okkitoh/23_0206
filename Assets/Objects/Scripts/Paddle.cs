using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    public bool isCPU = false;
    [SerializeField] 
    public Vector2 v = new Vector2(10f,0f);

    public float halfLength;
    public float radius;

    public Vector2 position {
        get {
            Vector3 p = this.transform.localPosition;
            return new Vector2(p.x, p.z);
        }
        set {
            this.transform.localPosition = new Vector3(value.x, 0f, value.y);
        }
    }

    private void Awake() {
        halfLength = this.transform.GetChild(0).localScale.x / 2;
        radius = this.transform.GetChild(0).localScale.z / 2;
    }

    public void Move(Ball ball, float dt) {
        if(isCPU) {
            this.transform.localPosition = ComputerMoveInput(ball, dt);
        } else {
            this.transform.localPosition = PlayerMoveInput(dt);
        }
    }
    // quick n dirty state machine
    internal const float DEVIATION_MAX = 6f;
    internal const float DEVIATION_MIN = 2f;
    internal static float bobtarget = 0f;
    internal static float delay_ms = 0f;
    private Vector3 ComputerMoveInput(Ball ball, float dt) {
        Vector3 nextPstn = this.transform.localPosition;
        float target = ball.position.x;
        float dx = this.v.x * dt;
        // If ball is moving away from CPU,
        // set target to origin and bob and weave around
        // the bob and weave part will be a target that flip flops from negative to positive
        if(ball.v.y < 0) {
            float displacement = bobtarget - nextPstn.x;
            if(delay_ms <= 0) { //bobtarget always leading in direction paddle should go
                bobtarget = UnityEngine.Random.Range(DEVIATION_MIN, DEVIATION_MAX) * (bobtarget > 0 ? -1 : 1);
                delay_ms = UnityEngine.Random.Range(0.150f, 0.500f); // best and worst realistic human response time
            } else if(Mathf.Abs(displacement) <= dx) {
                delay_ms -= dt;
            }
            target = bobtarget;
        } else {
            bobtarget = 0f;
            delay_ms = 0f;
        }

        if(Mathf.Abs(target - nextPstn.x) >= dx) {
            if(target > nextPstn.x) {
                nextPstn.x = nextPstn.x + dx;
            } else {
                nextPstn.x = nextPstn.x - dx;
            }
            float bounds = 10f - halfLength;
            nextPstn.x = Mathf.Clamp(nextPstn.x, -bounds, bounds);
        }
        return nextPstn;
    }

    private Vector3 PlayerMoveInput(float dt) {
        Vector3 nextPstn = this.transform.localPosition;
        nextPstn.x -= (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? v.x * dt : 0f;
        nextPstn.x += (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? v.x * dt : 0f;
        if(Mathf.Abs(nextPstn.x) >= (10.5f - halfLength)) {
            nextPstn.x = nextPstn.x/Mathf.Abs(nextPstn.x) * (10.5f - halfLength);
        }
        return nextPstn;
    }

    private void OnCollision(Action callback){
        callback();
    }

}
