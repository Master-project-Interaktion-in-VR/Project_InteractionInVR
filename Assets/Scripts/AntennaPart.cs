using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaPart : MonoBehaviour
{
    private Renderer _renderer;
    
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void HoverEnter()
    {
        _renderer.material.EnableKeyword("_EMISSION");
    }
    
    public void HoverExit()
    {
        _renderer.material.DisableKeyword("_EMISSION");
    }
}
