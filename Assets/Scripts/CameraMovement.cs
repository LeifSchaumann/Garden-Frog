using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 viewDirection;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void CenterOnLevel()
    {
        Vector2Int levelSize = LevelManager.instance.level.size;
        Vector3 levelCenter = new Vector3(levelSize.x / 2, 0, levelSize.y / 2);
        cam.orthographicSize = Mathf.Max(levelSize.x, levelSize.y) * 1.2f / 2f;
        transform.position = levelCenter - viewDirection.normalized * 30f;
        transform.rotation = Quaternion.LookRotation(viewDirection, Vector3.up);
    }
}
