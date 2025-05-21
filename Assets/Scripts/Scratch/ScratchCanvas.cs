using System;
using System.Collections.Generic;
using UnityEngine;

public class ScratchCanvas : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject scraperPrefab;
    [SerializeField] private float scratchZAxis;
    [SerializeField] private float generationCooldown;

    private Queue<GameObject> _scraperPool;
    private float _generationTimer;

    private void Awake()
    {
        _scraperPool = new Queue<GameObject>();
    }

    void Update()
    {
        _generationTimer -= Time.deltaTime;
        if (Input.GetMouseButton(0) && _generationTimer < 0)
        {
            _generationTimer = generationCooldown;

            Vector3 scraperPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            scraperPos.z = scratchZAxis;
            GameObject scraperGo = Instantiate(scraperPrefab, scraperPos, Quaternion.identity);
            _scraperPool.Enqueue(scraperGo);
        }
    }

    public void Dispose()
    {
        while (_scraperPool.Count > 0)
        {
            GameObject scraperGo = _scraperPool.Dequeue();
            Destroy(scraperGo);
        }
    }
}