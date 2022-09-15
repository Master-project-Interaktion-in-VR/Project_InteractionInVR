using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Credits: https://www.youtube.com/watch?v=JCyJ26cIM0Y by Valem Tutorials
public class FadeScreen : MonoBehaviour
{
    [SerializeField] private bool fadeOnStart = true;
    public float fadeDuration = 2;

    [SerializeField] private Color fadeColor;

    private Renderer _renderer;
    
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (fadeOnStart)
            FadeIn(fadeDuration);
    }

    public void FadeIn(float duration)
    {
        fadeDuration = duration;
        Fade(1,0);
    }
    
    public void FadeOut()
    {
        Fade(0,1);
    }

    private void Fade(float alphaIn, float alphaOut)
    {
        _renderer.enabled = true;
        StartCoroutine(FadeCoroutine(alphaIn, alphaOut));
    }

    private IEnumerator FadeCoroutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            var newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            
            _renderer.material.SetColor("_Color", newColor);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        var newColor2 = fadeColor;
        newColor2.a = alphaOut;
        _renderer.material.SetColor("_Color", newColor2);
        _renderer.enabled = false;
    }
}
