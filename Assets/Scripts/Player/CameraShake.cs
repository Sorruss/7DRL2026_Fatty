using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;


public class CameraShake : MonoBehaviour {
    public static CameraShake Instance;
    float shakeTimer;
    CinemachineCamera CVC;
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        CVC = GetComponent<CinemachineCamera>();
    }

    public void ShakeCamera(float intensity, float time) {
        CinemachineBasicMultiChannelPerlin CMP = CVC.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        CMP.AmplitudeGain = intensity; //CMP.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    private void Update() {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0) {
                CinemachineBasicMultiChannelPerlin CMP = CVC.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
                CMP.AmplitudeGain = 0;
            }
        }
    }
}
