using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameSettings",menuName ="GameSettings")]
public class GameSettings :ScriptableObject
{
    [Header("Block and Tower")]
    public Color[] BlockColors;
    public float BlockColorConsistency = 15f;
    public Vector3 LevelGroundCenterPosition;
    public int NumberOfBlockPerCircle;
    public int TowerHeight;
    public float BlockHeight;
    public float BlockExplosionRadius = 2;
    public float BlockExplosionForce = 1f;
    public float BlockGravityForce = 50f;
    public int ActiveBlocks = 8;
    [Header("Controlls")]
    public Controlls Controlls;
    [Header("Ball")]
    public float ShootForce;
    public int BallsPerLevel = 15;
    public float ShootTime = 0.35f;
    public AnimationCurve BallPath;
}

[System.Serializable]
public class Controlls
{
    public float TouchThreshold = 0.1f;
    public float CameraRotateSpeed = 10f;
    public float CameraMovementSpeed = 10f;
}
