using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Transform shadow;
    public int itemId;
    public bool isDropping;

    float targetY;
    float timeScale;
    float shadowScale;
    float accumulatedDelta;
    float deltaY;

    Vector3 shadowOriginLocalPos;
    Vector3 shadowOriginLocalScale;
    WaitForFixedUpdate waitFix;


    private void Awake()
    {
        targetY = 0.15f;
        timeScale = 2;
        shadowScale = 2;
        shadowOriginLocalPos = shadow.localPosition;
        shadowOriginLocalScale = shadow.localScale;
        waitFix = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        shadow.localScale = shadowOriginLocalScale;
        shadow.localPosition = shadowOriginLocalPos;
        deltaY = targetY;
        accumulatedDelta = 0f;
        isDropping = true;
    }
    private void FixedUpdate()
    {
        if (isDropping) return;

        accumulatedDelta += deltaY * Time.fixedDeltaTime * timeScale;
        if (accumulatedDelta >= targetY)
        {
            deltaY = accumulatedDelta - targetY;
            transform.position += new Vector3(0f, deltaY, 0f);
            shadow.localPosition -= new Vector3(0f, deltaY, 0f);
            accumulatedDelta = targetY;
            shadow.localScale -= new Vector3(deltaY * shadowScale * (shadowOriginLocalScale.y / 1.5f),
                deltaY * shadowScale * (shadowOriginLocalScale.y / 1.5f), 0f);

            deltaY = -targetY;
        }
        else if (accumulatedDelta <= 0)
        {
            deltaY = accumulatedDelta;
            transform.position += new Vector3(0f, deltaY, 0f);
            shadow.localPosition -= new Vector3(0f, deltaY, 0f);
            accumulatedDelta = 0;
            shadow.localScale -= new Vector3(deltaY * shadowScale * (shadowOriginLocalScale.y / 1.5f),
                deltaY * shadowScale * (shadowOriginLocalScale.y / 1.5f), 0f);

            deltaY = targetY;
        }
        else
        {
            var tempDeltaY = deltaY;
            deltaY = tempDeltaY * Time.fixedDeltaTime;
            transform.position += new Vector3(0f, deltaY, 0f);
            shadow.localPosition -= new Vector3(0f, deltaY, 0f);
            shadow.localScale -= new Vector3(deltaY * shadowScale * (shadowOriginLocalScale.y / 1.5f),
                deltaY * shadowScale * (shadowOriginLocalScale.y / 1.5f), 0f);

            deltaY = tempDeltaY;
        }
    }

    public void Init()
    {
        StartCoroutine(Dropping());
    }

    IEnumerator Dropping()
    {
        isDropping = true;
        var originScale = transform.localScale;
        var originPos = transform.position;
        Vector3 deltaPos = new Vector3(0f, 2f, 0f);
        transform.localScale = Vector3.zero;
        float timer = 0f;
        float endTime = .3f;
        
        while (true)
        {
            if (!isDropping)
                break;

            yield return waitFix;
            timer += Time.fixedDeltaTime;
            if (timer > endTime)
            {
                transform.position = originPos;
                break;
            }
            else if (timer > endTime / 2)
            {
                transform.position -= deltaPos * Time.fixedDeltaTime / endTime;
                transform.localScale -= originScale * 0.5f * Time.fixedDeltaTime / endTime;
                continue;
            }

            transform.position += deltaPos * Time.fixedDeltaTime / endTime;
            transform.localScale += originScale * 2f * Time.fixedDeltaTime / endTime;
        }
        isDropping = false;
        transform.localScale = originScale;
    }
}
