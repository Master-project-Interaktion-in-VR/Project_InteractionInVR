using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Credits: https://www.youtube.com/watch?v=JCyJ26cIM0Y by Valem Tutorials
public class FadeScreen : MonoBehaviour
{
    public float fadeDuration = 2;
    
    
    [SerializeField] 
    private bool fadeOnStart = true;

    [SerializeField] 
    private Color fadeColor;
    
    
    private static readonly int Color = Shader.PropertyToID("_Color");

    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        
        if (fadeOnStart)
            FadeIn(fadeDuration);
    }

    /// <summary>
    /// Calls the Fade function from 1 to 0
    /// </summary>
    /// <param name="duration"> fade duration </param>
    public void FadeIn(float duration)
    {
        fadeDuration = duration;
        Fade(1,0);
    }
    
    /// <summary>
    /// Calls the Fade function from 0 to 1
    /// </summary>
    public void FadeOut()
    {
        Fade(0,1);
    }

    /// <summary>
    /// Enables the fade renderer and starts a coroutine to fade from alphaIn to alphaOut
    /// </summary>
    /// <param name="alphaIn"> start alpha </param>
    /// <param name="alphaOut"> end alpha </param>
    private void Fade(float alphaIn, float alphaOut)
    {
        _renderer.enabled = true;
        StartCoroutine(FadeCoroutine(alphaIn, alphaOut));
    }

    /// <summary>
    /// Lerp from alphaIn to alphaOut depend on fadeDuration and set renderer color
    /// </summary>
    /// <param name="alphaIn"> start alpha </param>
    /// <param name="alphaOut"> end alpha </param>
    /// <returns> IEnumerator for Coroutine </returns>
    private IEnumerator FadeCoroutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            var newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            
            _renderer.material.SetColor(Color, newColor);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        var newColor2 = fadeColor;
        newColor2.a = alphaOut;
        _renderer.material.SetColor(Color, newColor2);
        _renderer.enabled = false;
    }
}
