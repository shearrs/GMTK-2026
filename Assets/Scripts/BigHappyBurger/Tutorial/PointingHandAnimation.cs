using UnityEngine;
using Shears.Tweens;
using System.Collections;

public class PointingHandAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform handTransform;

    [SerializeField]
    private Transform originalTransform;

    [SerializeField]
    private Transform backTransform;

    [SerializeField]
    private TweenData moveTween;

    private readonly TweenStorage tweenStorage = new();

    private Coroutine coroutine;
    private Coroutine coroutine2;

    private void OnEnable()
    {
        coroutine = StartCoroutine(moveBack());
    }

    private IEnumerator moveBack()
    {
        var backTween = tweenStorage.Store(handTransform.DoMoveLocalTween(backTransform.localPosition, moveTween));

        backTween.Completed += () =>
        {
            coroutine2 = StartCoroutine(moveForward());
        };
        yield return null;
    }

    private IEnumerator moveForward()
    {
        var forwardTween = tweenStorage.Store(handTransform.DoMoveLocalTween(originalTransform.localPosition, moveTween));

        forwardTween.Completed += () =>
        {
            coroutine = StartCoroutine(moveBack());
        };
        yield return null;
    }

    private void OnDisable()
    {
        tweenStorage.Dispose();
        StopAllCoroutines();
    }
}
