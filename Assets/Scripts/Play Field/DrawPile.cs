using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPile : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    private List<Transform> cards = new List<Transform>();

    [SerializeField] private float cardOffset = -0.001f;
    [SerializeField] private float maxRotationDelta;

    public void SpawnCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            cards.Insert(0, Instantiate(cardPrefab, 
                                        transform.position + new Vector3(0, 0, cardOffset * i), 
                                        Quaternion.Euler(new Vector3(0, 0, Random.Range(-maxRotationDelta, maxRotationDelta))),
                                        transform).transform);
        }
    }

    public Coroutine DoDrawCardAnim(int count, Transform target, float duration)
    {
        return StartCoroutine(DrawCardAnim(target, duration));
    }

    private IEnumerator DrawCardAnim(Transform target, float duration)
    {
        Transform card = cards[0];
        cards.Remove(card);
        
        float end = Time.time + duration;
        Vector3 startPos = card.position;
        //Vector3 startRot = card.eulerAngles;

        while (Time.time <= end)
        {
            float t = 1 - (end - Time.time) / duration;
            card.position = startPos + (target.position - startPos) * t;
            //card.eulerAngles = startRot + (target.eulerAngles - startRot) * t;

            yield return new WaitForEndOfFrame();
        }

        Destroy(card.gameObject);
    }
}
