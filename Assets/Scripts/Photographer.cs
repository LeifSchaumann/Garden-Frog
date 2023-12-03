using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photographer : MonoBehaviour
{
    public static Photographer main;
    
    private CameraMovement camMove;
    private Camera cam;
    private LevelManager levelManager;
    public RenderTexture renderTexture;

    private void Awake()
    {
        main = this;

        cam = GetComponent<Camera>();
        camMove = GetComponent<CameraMovement>();
        levelManager = camMove.levelManager;
    }

    public void PhotographLevel(LevelData levelData, Action onFinish = null)
    {
        onFinish ??= () => { };

        levelManager.UnloadLevel(true);
        levelManager.LoadLevel(levelData, true, () =>
        {
            StartCoroutine(PhotoRoutine(levelData, onFinish));
        });
    }

    IEnumerator PhotoRoutine(LevelData levelData, Action onFinish)
    {
        camMove.SetZoom(true, 1f);

        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0.1f);

        Texture2D photo = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, true); //, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = renderTexture;
        photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        photo.Apply();
        
        //byte[] bytes = photo.EncodeToPNG();
        //System.IO.File.WriteAllBytes("Assets/Level1.png", bytes);

        levelData.photo = photo;
        RenderTexture.active = null;
        onFinish();
    }
}
