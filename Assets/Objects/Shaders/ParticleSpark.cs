using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;
using static UnityEngine.ParticleSystem;

public class ParticleSpark : MonoBehaviour
{
    [SerializeField]
    ComputeShader Shader;
    ComputeBuffer result;
    Vector4[] output;       /* Intermediary buffer to get compute shader results */
    Transform[] particles;  /* Reference to particles drawn, position set via compute */

    int kernel;
    uint thGroupSize;

    [SerializeField, Range(1,1000)]
    int particleCount = 1;  /* number of particles to generate */

    [SerializeField, Min(0.01f)]
    float coneHeight = 0.3f;   /* tan( opp/adj ) = tan( r/h ) = max angle */
    
    [SerializeField, Min(0.01f)]
    float radius = 2;       /* explosions to shotgun spreads */

    [SerializeField, Min(0.01f)]
    float minVelocity = 2;
    [SerializeField, Min(0.01f)]
    float maxVelocity = 10;

    [SerializeField]
    float scale = 1f;



    /* debug stuff */
    [SerializeField]
    Transform Pathtester;

    void OnEnable() {
        kernel = Shader.FindKernel("RandomConeRays");
        Shader.GetKernelThreadGroupSizes(kernel, out thGroupSize, out _, out _);
        result = new ComputeBuffer(particleCount, sizeof(float) * 4);
        output = new Vector4[particleCount];

        particles = new Transform[particleCount];
        Vector4[] initData = new Vector4[particleCount];
        for(int i = 0; i < particleCount; ++i) {
            particles[i] = Instantiate(Pathtester, Vector3.zero, Quaternion.identity);
            particles[i].transform.localScale = Vector3.one * scale;
            particles[i].transform.SetParent(this.transform);
            initData[i] = Vector4.zero;
        }
        result.SetData(initData);    
    }

    void Update() {
        Shader.SetBuffer(kernel, "Result", result);
        int thGroups = (int) ((particleCount + (thGroupSize - 1)) / thGroupSize);
        Shader.SetFloat("dt", Time.deltaTime);
        Shader.SetFloat("h", coneHeight);
        Shader.SetFloat("r_max", radius);
        Shader.SetFloat("v_min", minVelocity);
        Shader.SetFloat("v_max", maxVelocity);
        
        Shader.Dispatch(kernel, thGroups, 1, 1);
        result.GetData(output);

        for(int i = 0; i < particleCount; ++i) {
            Vector4 o = output[i];
            particles[i].transform.localPosition = new Vector3(o.x, o.y, o.z);
            particles[i].transform.localScale = Vector3.one * scale * o.w;
            Debug.Log(output[i]);
        }
    }

    float scaleByDistance(float distance) {
        return (30 - distance)/30;
    }

    void OnDestroy() {
        result.Release();
        result = null;
    }
}
