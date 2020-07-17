using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    public Circle ParentCircle;

    private void OnTriggerEnter(Collider other)
    {
        BlockController controller = other.GetComponent<BlockController>();
        if (!controller) return;
        if (controller.ParentCircle != ParentCircle) return;
        controller.GroundHit();
    }
}
