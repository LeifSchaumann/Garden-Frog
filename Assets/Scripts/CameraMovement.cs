using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 viewDirection;

    private Camera cam;
    private Vector3 targetPosition;
    private float targetSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        transform.rotation = Quaternion.LookRotation(viewDirection, Vector3.up);
        targetPosition = transform.position;
        targetSize = cam.orthographicSize;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.003f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, 0.003f);
    }

    public void CenterOnLevel()
    {
        Vector2Int levelSize = LevelManager.instance.level.size;
        Vector3 levelCenter = LevelManager.instance.LevelToWorld(new Vector3(levelSize.x / 2, 0, levelSize.y / 2));
        targetSize = Mathf.Max(levelSize.x, levelSize.y) * 1.2f / 2f;
        targetPosition = levelCenter - viewDirection.normalized * 30f;
    }
}
