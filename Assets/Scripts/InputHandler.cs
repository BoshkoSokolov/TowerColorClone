using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector3 TouchStartPosition { get { return touchStartPosition; } }
    private Vector3 touchStartPosition;

    private List<TouchPhase> sortedPreviousPhases = new List<TouchPhase>();

    private void Awake()
    {
        InputReciever.OnTouchBegan += Began;
        InputReciever.OnTouchCanceled += Canceled;
        InputReciever.OnTouchEnded += Ended;
        InputReciever.OnTouchMoved += Moved;
        InputReciever.OnTouchStationary += Stationary;
    }

    private void OnDestroy()
    {
        InputReciever.OnTouchBegan -= Began;
        InputReciever.OnTouchCanceled -= Canceled;
        InputReciever.OnTouchEnded -= Ended;
        InputReciever.OnTouchMoved -= Moved;
        InputReciever.OnTouchStationary -= Stationary;
    }

    public void Began(Touch touch)
    {
        sortedPreviousPhases.Clear();
        sortedPreviousPhases.Add(TouchPhase.Began);
        touchStartPosition = touch.position;
    }

    public void Moved(Touch touch)
    {
        if (Vector3.Distance(touch.deltaPosition, touchStartPosition) < GameManager.Instance.GetGameSettings().Controlls.TouchThreshold && !sortedPreviousPhases.Contains(TouchPhase.Moved)) return;
        Vector3 touchPosition = touch.position;
        CameraController.Instance.InterpretControlls(touchPosition - touchStartPosition);
        sortedPreviousPhases.Add(TouchPhase.Moved);
        touchStartPosition = touch.position;
    }

    public void Stationary(Touch touch)
    {
        sortedPreviousPhases.Add(TouchPhase.Stationary);
    }

    public void Ended(Touch touch)
    {
        sortedPreviousPhases.Add(TouchPhase.Ended);
        if (!sortedPreviousPhases.Contains(TouchPhase.Moved)) BallThrower.Instance.ThrowBall(touch.position);
        else
        {
            Vector3 touchPosition = touch.position;
            CameraController.Instance.InterpretControlls(touchPosition - touchStartPosition, true);
        }
    }

    public void Canceled(Touch touch)
    {
        sortedPreviousPhases.Add(TouchPhase.Canceled);
    }
}
