using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WhackAMoleGameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 3;
    [SerializeField] private int height = 3;
    [SerializeField] private float cellSpacing = 2f;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Mole molePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float startSpawnInterval = 1.5f;
    [SerializeField] private float minSpawnInterval = 0.35f;
    [SerializeField] private float acceleration = 0.03f;
    [SerializeField] private float moleLifeTime = 1.2f;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    private List<Vector3> spawnPositions = new List<Vector3>();
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private int score;
    private float currentSpawnInterval;

    private void Start()
    {
        currentSpawnInterval = startSpawnInterval;

        CreateGrid();
        UpdateScoreText();

        StartCoroutine(SpawnLoop());
    }

    private void CreateGrid()
    {
        spawnPositions.Clear();

        float offsetX = (width - 1) * cellSpacing / 2f;
        float offsetZ = (height - 1) * cellSpacing / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(
                    x * cellSpacing - offsetX,
                    0f,
                    z * cellSpacing - offsetZ
                );

                Instantiate(floorPrefab, position, Quaternion.identity, transform);

                Vector3 molePosition = position + Vector3.up * 0.5f;
                spawnPositions.Add(molePosition);
            }
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnMole();

            currentSpawnInterval -= acceleration;
            currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnMole()
    {
        if (spawnPositions.Count == 0) return;

        List<Vector3> availablePositions = new List<Vector3>();

        foreach (Vector3 pos in spawnPositions)
        {
            if (!occupiedPositions.Contains(pos))
            {
                availablePositions.Add(pos);
            }
        }

        if (availablePositions.Count == 0) return;

        Vector3 spawnPos = availablePositions[Random.Range(0, availablePositions.Count)];

        occupiedPositions.Add(spawnPos);

        Mole mole = Instantiate(molePrefab, spawnPos, Quaternion.identity);
        mole.Initialize(this, spawnPos, moleLifeTime);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void RemoveOccupiedPosition(Vector3 position)
    {
        occupiedPositions.Remove(position);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score : {score}";
        }
    }
}