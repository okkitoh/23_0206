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
        gameObject.SetActive(true);
        Reset();
    }

    public void Reset()
    {
        this.transform.localPosition = Vector3.zero;
        int x_dir = Random.Range(2,51) % 2 == 0 ? 1 : -1;
        this.v = new Vector2(x_dir * Random.Range(6f, 8f),10f); // [-8,-6] <-> [6, 8]
    }

    public void Move(float dt) {
        float nextX = this.transform.localPosition.x + (this.v.x * dt);
        float nextY = this.transform.localPosition.z + (this.v.y * dt);
        if(nextX < -10f || nextX > 10f) {
            nextX = Mathf.Clamp(nextX, -10f, 10f);
            this.v.x *= -1;
        }
        if(nextY < -10f || nextY > 10f)
        {
            nextY = Mathf.Clamp(nextY,-10f,10f);
            this.v.y *= -1;
        }

        this.transform.localPosition = new Vector3(nextX, 0, nextY);
    }

    public bool CollisionCheck(Paddle p, float dt) {
        Vector3 nextPstn = this.transform.localPosition;
        float stepY = this.transform.localPosition.z + (this.v.y * dt);
        if(Mathf.Abs(p.position.y - stepY) <= (this.radius + p.radius)) {
            nextPstn.y = p.position.y - (p.radius + this.radius + 0.01f);
            this.v.y *= -1;
            float x_col = XCollisionCheck(p);
            if(x_col >= 0 && x_col < 1) {
                this.v.x = (this.v.x / Mathf.Abs(this.v.x)) * Mathf.Max(vx_min, vx_max * x_col);
                nextPstn.x = Mathf.Clamp(this.transform.localPosition.x + (this.v.x * dt), -10f, 10f);
            } else {
                return false;
            }
        }
        this.transform.localPosition = nextPstn;
        return true;
    }

    
    private float XCollisionCheck(Paddle p) {
        float colx = Mathf.Abs(this.position.x - p.position.x)/(this.radius + p.halfLength);
        return colx >= 0 && colx < 1f ? colx : -1;
    }










    //public void UpdateVisualization() {
    //    this.transform.localPosition = new Vector3(position.x, 0f, position.y);
    //}
    //public void Move() {
    //    position += velocity * Time.deltaTime;
    //}
    //public void Reset() {
    //    position = Vector2.zero;
    //    UpdateVisualization();
    //    velocity = new Vector2(startXSpeed, -constantYSpeed);
    //    gameObject.SetActive(true);
    //}
    //   public void EndGame ()
    //{
    //	position.x = 0f;
    //	gameObject.SetActive(false);
    //}

    //   public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime) {
    //       velocity.x = maxXSpeed * speedFactor; // where in range [0, maxXSpeed]
    //       position.x = start + velocity.x * deltaTime;
    //   }

    //   // Ball manages its own state given relevant information
    //   public void BounceX(float boundary) {
    //       float durationAfterBounce = (position.x - boundary) / velocity.x;
    //       position.x = 2f * boundary - position.x;
    //       velocity.x = -velocity.x;
    //       EmitBounceParticles(
    //           boundary,
    //           position.y - velocity.y * durationAfterBounce,
    //           boundary < 0f ? 90f : 270f
    //       );
    //   }
    //   public void BounceY(float boundary) {
    //       float durationAfterBounce = 
    //       position.y = 2f * boundary - position.y;
    //       velocity.y = -velocity.y;
    //       EmitBounceParticles(
    //           boundary,
    //           position.x - velocity.x * durationAfterBounce,
    //           boundary < 0f ? 0f : 180f
    //       );
    //   }


    void EmitBounceParticles(float x, float z, float rotation) {
        ParticleSystem.ShapeModule shape = bounceParticleSystem.shape;
        shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0f, rotation, 0f);
        bounceParticleSystem.Emit(bounceParticleEmission);
    }
}
