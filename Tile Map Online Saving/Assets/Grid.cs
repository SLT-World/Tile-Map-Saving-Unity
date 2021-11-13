using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    Vector3 Position;

    void LateUpdate()
    {
        Position.x = Mathf.Round(transform.position.x);
        Position.y = Mathf.Round(transform.position.y);
        Position.z = Mathf.Round(transform.position.z);

        transform.position = Position;
    }
}
