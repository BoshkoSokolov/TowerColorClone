using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputReciever : MonoBehaviour
{
    public static UnityAction<Touch> OnTouchBegan = null;
    public static UnityAction<Touch> OnTouchMoved = null;
    public static UnityAction<Touch> OnTouchStationary = null;
    public static UnityAction<Touch> OnTouchEnded = null;
    public static UnityAction<Touch> OnTouchCanceled = null;

    private void Awake()
    {
        GameManager.Instance.OnUpdate += CheckInput;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnUpdate -= CheckInput;
    }

    private void CheckInput()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began) OnTouchBegan?.Invoke(touch);
        else if (touch.phase == TouchPhase.Stationary) OnTouchStationary?.Invoke(touch);
        else if (touch.phase == TouchPhase.Moved) OnTouchMoved?.Invoke(touch);
        else if (touch.phase == TouchPhase.Ended) OnTouchEnded?.Invoke(touch);
        else if (touch.phase == TouchPhase.Canceled) OnTouchCanceled?.Invoke(touch);
    }
}