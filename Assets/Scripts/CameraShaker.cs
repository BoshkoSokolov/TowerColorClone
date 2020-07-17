using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public Transform ShakeTransform;
    public float Duration = 0.25f;
    public float ShakeAmount = 0.7f;
    public float Decrease = 1.0f;

    private float shakeTimer = 0f;
    private bool isShaking = false;
    private Vector3 startingPosition;

    public static CameraShaker Instance;

    private void Awake()
    {
        Instance = this;
    }


    public void Shake(object obj = null)
    {
        if (isShaking) return;
        shakeTimer = Duration;
        startingPosition = ShakeTransform.localPosition;
        isShaking = true;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            ShakeTransform.localPosition = startingPosition + Random.insideUnitSphere * ShakeAmount;
            shakeTimer -= Time.deltaTime * Decrease;
        }
        else
        {
            shakeTimer = 0f;
            ShakeTransform.localPosition = startingPosition;
            isShaking = false;
        }
    }
}
