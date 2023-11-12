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
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.003f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, 0.003f);
    }

    public void FocusOn(Vector3 focus, bool instant = false)
    {
        Vector2Int levelSize = LevelManager.instance.level.size;
        targetSize = Mathf.Max(levelSize.y * 0.55f, levelSize.x * 9 / 16 * 0.7f);
        targetPosition = focus - viewDirection.normalized * 30f;
        if (instant)
        {
            transform.position = targetPosition;
            cam.orthographicSize = targetSize;
        }
    }
}
