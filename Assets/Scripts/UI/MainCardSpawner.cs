using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCardSpawner : MonoBehaviour
{
    [Header("Card Spawn")]
    [SerializeField] private Rigidbody physicsCard;
    [SerializeField] private string cardPath;
    [SerializeField] private string[] cardFolders;
    private List<Card> cards = new List<Card>();

    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    private float nextSpawn;

    [SerializeField] private float minSpawnX;
    [SerializeField] private float maxSpawnX;
    [SerializeField] private float spawnY;
    [SerializeField] private float spawnZ;

    [SerializeField] private float minForceX;
    [SerializeField] private float maxForceX;
    [SerializeField] private float minForceY;
    [SerializeField] private float maxForceY;


    private void Start()
    {
        foreach (string s in cardFolders)
        {
            cards.AddRange(Resources.LoadAll<Card>(cardPath + "/" + s));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + Random.Range(minSpawnTime, maxSpawnTime);

            Rigidbody rb = Instantiate(physicsCard, new Vector3(Random.Range(minSpawnX, maxSpawnX), spawnY, spawnZ), Quaternion.identity);
            float xDir = Random.Range(minForceX, maxForceX);
            rb.AddForce(new Vector3(xDir, Random.Range(minForceY, minForceY), 0), ForceMode.Impulse);
            rb.angularVelocity = new Vector3(0, 0, -xDir);

            Instantiate(cards[Random.Range(0, cards.Count - 1)], rb.transform);
        }
    }
}
