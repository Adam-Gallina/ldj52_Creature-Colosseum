using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropIcon : MonoBehaviour
{
    public CropClass CropType;
    [HideInInspector] public bool consumed = false;

    public Coroutine MoveIcon(Transform target, float duration, bool consumeAfterMove=false)
    {
        return MoveIcon(target.position, duration, consumeAfterMove);
    }
    public Coroutine MoveIcon(Vector3 target, float duration, bool consumeAfterMove=false)
    {
        return StartCoroutine(DoMoveIcon(target, duration, consumeAfterMove));
    }

    protected IEnumerator DoMoveIcon(Vector3 target, float duration, bool consumeAfterMove=false)
    {
        if (consumeAfterMove)
            consumed = true;

        float end = Time.time + duration;
        Vector3 startPos = transform.position;

        while (Time.time < end)
        {
            float t = 1 - (end - Time.time) / duration;
            transform.position = startPos + (target - startPos) * t;
            yield return new WaitForEndOfFrame();
        }

        if (consumeAfterMove)
            gameObject.SetActive(false);
    }
}
