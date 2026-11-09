using UnityEngine;

public class ProjectileSound : MonoBehaviour
{
    [SerializeField] string[] fireClipNames = { "FireworksExp2" };
    [SerializeField] float pitchMin = 0.75f;
    [SerializeField] float pitchMax = 1.25f;

    ParticleSystem _particleSystem;
    ParticleSystem.Particle[] _particles;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    }

    void OnParticleTrigger()
    {
        if (fireClipNames.Length == 0) return;

        int count = _particleSystem.GetParticles(_particles);
        if (count == 0) return;

        string clipName = fireClipNames[Random.Range(0, fireClipNames.Length)];
        float pitch = Random.Range(pitchMin, pitchMax);
        AudioManager.Singleton.PlaySFX3D(clipName, _particles[0].position, 0f, pitch);
    }
}
