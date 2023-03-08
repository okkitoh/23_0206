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
    Vector3[] output;       /* Intermediary buffer to get compute shader results */
    Transform[] particles;  /* Reference to particles drawn, position set via compute */

    int kernel;
    uint thGroupSize;

    [SerializeField]
    int particleCount = 1;  /* number of particles to generate */

    [SerializeField]
    float coneHeight = 0.3f;   /* tan( opp/adj ) = tan( r/h ) = max angle */
    
    [SerializeField]
    float radius = 2;       /* explosions to shotgun spreads */

    [SerializeField]
    float constantVelocity = 10;

    /* debug stuff */
    [SerializeField]
    Transform Pathtester;

    void OnEnable() {
        kernel = Shader.FindKernel("RandomConeRays");
        Shader.GetKernelThreadGroupSizes(kernel, out thGroupSize, out _, out _);
        result = new ComputeBuffer(particleCount, sizeof(float) * 3);
        output = new Vector3[particleCount];

        particles = new Transform[particleCount];
        Vector3[] initData = new Vector3[particleCount];
        for(int i = 0;i < particleCount; ++i) {
            particles[i] = Instantiate(Pathtester, Vector3.zero, Quaternion.identity);
            particles[i].transform.localScale = new Vector3(0.05f,0.05f,0.05f);
            particles[i].transform.SetParent(this.transform);
            initData[i] = Vector3.zero;
        }
        result.SetData(initData);    
    }

    void Update() {
        Shader.SetBuffer(kernel, "Result", result);
        int thGroups = (int) ((particleCount + (thGroupSize - 1)) / thGroupSize);
        Shader.SetFloat("dt", Time.deltaTime);
        Shader.SetFloat("h", coneHeight);
        Shader.Dispatch(kernel, thGroups, 1, 1);
        result.GetData(output);

        for(int i = 0; i < particleCount; ++i) {
            particles[i].gameObject.SetActive(true);
            particles[i].transform.localPosition = output[i];
        }
    }

    void OnDestroy() {
        result.Release();
        result = null;
    }
}
