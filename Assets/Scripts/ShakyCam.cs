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
    }

    public void Shake(float intensity, float decay)
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        shake_intensity = intensity;
        shake_decay = decay;
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