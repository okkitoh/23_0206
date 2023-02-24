using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public float radius = 0.5f;
    [HideInInspector]
    public float vx_max = 20f;
    [HideInInspector]
    public float vx_min = 8f;
    [HideInInspector]
    public Vector2 v;
    [SerializeField]
    ParticleSystem bounceParticleSystem;
    [SerializeField]
    int bounceParticleEmission = 20;

    private AudioSource bonk;


 
    // ball is moving across x and z, transpose z as y on input/output
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
        radius = this.transform.localScale.x/2;
        bonk = gameObject.GetComponent<AudioSource>();
        if(bonk == null) {
            Debug.LogError("No Audio Source found ");
        }
        gameObject.SetActive(true);
        Reset();
    }
    public void PlayBonk(float pitch) {
        bonk.time = 0.14f;
        bonk.pitch = pitch;
        bonk.Play();
    }

    public void Reset()
    {
        this.transform.localPosition = Vector3.zero;
        int x_dir = Random.Range(2,51) % 2 == 0 ? 1 : -1;
        this.v = new Vector2(x_dir * Random.Range(6f, 8f),10f); // [-8,-6] <-> [6, 8]
    }

    public void Move(float dt) {
        if(Input.GetKeyDown(KeyCode.Space)) {
            this.PlayBonk(0.5f);
        }

        float nextX = this.transform.localPosition.x + (this.v.x * dt);
        float nextY = this.transform.localPosition.z + (this.v.y * dt);
        if(nextX < -10f || nextX > 10f) {
            nextX = Mathf.Clamp(nextX, -10f, 10f);
            this.v.x *= -1;
            this.PlayBonk(0.5f);
        }
        if(nextY < -10f || nextY > 10f)
        {
            nextY = Mathf.Clamp(nextY,-10f,10f);
            this.v.y *= -1;
            this.PlayBonk(0.5f);
        }

        this.transform.localPosition = new Vector3(nextX, 0, nextY);
    }

    public bool CollisionCheck(Paddle p, float dt) {
        Vector3 nextPstn = this.transform.localPosition;
        float stepY = this.transform.localPosition.z + (this.v.y * dt);
        if(Mathf.Abs(p.position.y - stepY) <= (this.radius + p.radius)) {
            nextPstn.y = p.position.y - (p.radius + this.radius + 0.2f);
            this.v.y *= -1;
            float x_col = XCollisionCheck(p);
            if(x_col >= 0 && x_col < 1) {
                //  1 -> maintain trajectory
                // -1 -> reverse trajectory
                float dir = 1;
                if(x_col > 0.35f && OPP_SIGN(this.v.x, p.v.x)) {
                    dir = -1;
                }
                this.v.x = dir * (this.v.x / Mathf.Abs(this.v.x)) * Mathf.Max(vx_min, vx_max * x_col);
                nextPstn.x = Mathf.Clamp(this.transform.localPosition.x + (this.v.x * dt), -10f, 10f);

                this.PlayBonk(0.5f + (1 * x_col/2));
            } else {
                return false;
            }
        }
        this.transform.localPosition = nextPstn;
        return true;
    }

    // 0 -> center
    // 1 -> edge
    private float XCollisionCheck(Paddle p) {
        float colx = Mathf.Abs(this.position.x - p.position.x)/(this.radius + p.halfLength);
        return colx >= 0 && colx < 1f ? colx : -1;
    }

    private bool OPP_SIGN(float a, float b) {
        return (a * b < 0);
    }


    void EmitBounceParticles(float x, float z, float rotation) {
        ParticleSystem.ShapeModule shape = bounceParticleSystem.shape;
        shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0f, rotation, 0f);
        bounceParticleSystem.Emit(bounceParticleEmission);
    }
}
