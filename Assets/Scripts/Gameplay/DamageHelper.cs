using UnityEngine;
using System.Collections;

public class DamageHelper : MonoBehaviour
{
    public enum ParticleAmount
    {
        damage = 100,
        death = 1000
    };

    private AudioSource aSource;
    private ParticleSystem pSystem;

	void Start ()
    {
        pSystem = GetComponent<ParticleSystem>();
        aSource = GetComponent<AudioSource>();
        pSystem.GetComponent<Renderer>().sortingLayerName = "Foreground";
	}

    public void BurstParticles(int numParticles, Color color)
    {
        pSystem.startColor = color;
        pSystem.Emit(numParticles);
    }

    public void BurstParticles(int numParticles, Color color, Vector2 pos)
    {
        transform.position = pos;
        BurstParticles(numParticles, color);
    }

    public void PlaySound()
    {
        aSource.Play();
    }
}