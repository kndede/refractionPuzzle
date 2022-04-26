using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandAnimation : MonoBehaviour
{
    [Header("Finger Settings")]
    [SerializeField] float moveSpeed = 250f;
    [SerializeField] bool isInfinite = true;
    [SerializeField] int repeateCount = 3;
    [Range(0.01f, 0.2f)]
    [SerializeField] float dissolveSpeed = 0.04f;
    [Range(0.01f, 0.2f)]
    [SerializeField] float appearSpeed = 0.04f;
    public RectTransform point1;
    public RectTransform point2;

    [Header("Condition Settings")]
    [SerializeField] string conditionName;
    [SerializeField] int conditionVal;
    [SerializeField] bool isTrue = false;

    [Header("Animation Settings")]
    [SerializeField] Image handImage;
    [SerializeField] bool reverseAnimation;
    [SerializeField] bool dissolveAnimation;

    [Header("Text Settings")]
    [SerializeField] GameObject infoText;

    void Start()
    {
        if (isTrue)
            StartCoroutine(FingerMovement());
        else
            StartCoroutine(CheckIsReady());
    }

    IEnumerator FingerMovement()
    {
        handImage.enabled = true;
        infoText.SetActive(true);
        for (int i = 0; i < repeateCount; i++)
        {
            transform.position = point1.position;
            while (Vector3.Distance(transform.position, point2.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point2.position, UnityEngine.Time.deltaTime * moveSpeed);
                yield return new WaitForSeconds(0.01f);
            }
            if (reverseAnimation)
            {
                while (Vector3.Distance(transform.position, point1.position) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, point1.position, UnityEngine.Time.deltaTime * moveSpeed);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            if (isInfinite)
                i--;
        }

    }

    IEnumerator CheckIsReady()
    {
        yield return new WaitUntil(() => PlayerPrefs.GetInt(conditionName) == conditionVal || isTrue);
        isTrue = true;
        StartCoroutine(FingerMovement());
    }
}
