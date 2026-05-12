using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour
{
    [SerializeField] private int point = 1;
    [SerializeField] private ParticleSystem hitParticle;

    private WhackAMoleGameManager gameManager;
    private Vector3 spawnPosition;
    private bool isHit;
    private float lifeTime;

    public void Initialize(
        WhackAMoleGameManager manager,
        Vector3 position,
        float moleLifeTime
    )
    {
        gameManager = manager;
        spawnPosition = position;
        lifeTime = moleLifeTime;

        StartCoroutine(LifeTimer());
    }

    private void OnMouseDown()
    {
        Hit();
    }

    private void Hit()
    {
        if (isHit) return;

        isHit = true;

        gameManager.AddScore(point);

        if (hitParticle != null)
        {
            ParticleSystem particle = Instantiate(
                hitParticle,
                transform.position,
                Quaternion.identity
            );

            particle.Play();
            Destroy(particle.gameObject, particle.main.duration);
        }

        DestroyMole();
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!isHit)
        {
            DestroyMole();
        }
    }

    private void DestroyMole()
    {
        if (gameManager != null)
        {
            gameManager.RemoveOccupiedPosition(spawnPosition);
        }

        Destroy(gameObject);
    }
}