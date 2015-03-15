using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Camera shake cred: http://www.mikedoesweb.com/2012/camera-shake-in-unity

public class ShakyCam : MonoBehaviour
{
    private Vector3 originPosition;
    private Quaternion originRotation;
    private float shake_decay;
    private float shake_intensity;

    private bool shaking = false;

    void Update()
    {
        if (shake_intensity > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            transform.rotation = new Quaternion(
            originRotation.x + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
            originRotation.y + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
            originRotation.z + Random.Range(-shake_intensity, shake_intensity) * 0.2f,
            originRotation.w + Random.Range(-shake_intensity, shake_intensity) * 0.2f);
            shake_intensity -= shake_decay;
        }

        if (shake_intensity <= 0 && shaking)
        {
            transform.position = originPosition;
            transform.rotation = originRotation;
            shaking = false;
        }
    }

    public void Shake(float intensity, float decay)
    {
        if(!shaking)
        {
            //Reset Origin
            originPosition = transform.position;
            originRotation = transform.rotation;
        }
            shake_intensity = intensity;
            shake_decay = decay;

            shaking = true;
    }

    public void Sleep(float newScale, float resetTime)
    {
        Time.timeScale = newScale;

        float realResetTime = resetTime * newScale;
        Invoke("ResetTimeScale", realResetTime);
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }
}