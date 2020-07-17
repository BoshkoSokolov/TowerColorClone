using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        BlockController controller = collision.collider.GetComponent<BlockController>();
        if (!controller) return;
        controller.GroundHit();
    }

}
