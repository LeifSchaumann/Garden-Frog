using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialUpdate : MonoBehaviour
{
    private float timePassed;
    private Material material;

    private void Awake()
    {
        timePassed = 0;
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if (material.HasProperty("_CurrentTime"))
        {
            // Set the new color for the "_Color" property
            material.SetFloat("_CurrentTime", timePassed);
        }
        else
        {
            Debug.LogError("Material does not have a '_CurrentTime' property.", transform);
        }
    }
}
