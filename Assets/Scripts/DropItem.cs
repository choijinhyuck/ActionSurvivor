using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Transform shadow;

    float targetY;
    float timeScale;
    float shadowScale;
    float accumulatedDelta;
    float deltaY;
    Vector3 shadowOriginLocalPos;
    Vector3 shadowOriginLocalScale;


    private void Awake()
    {
        targetY = 0.15f;
        timeScale = 2;
        shadowScale = 2;
        shadowOriginLocalPos = shadow.localPosition;
        shadowOriginLocalScale = shadow.localScale;
    }

    private void OnEnable()
    {
        shadow.localScale = shadowOriginLocalScale;
        shadow.localPosition = shadowOriginLocalPos;
        deltaY = targetY;
        accumulatedDelta = 0f;
    }
    private void FixedUpdate()
    {

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
}
