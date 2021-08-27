using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static IEnumerator CrossFading<A>(A from, A to, float duration, Action<A> setter, Func<A, A, float, A> lerp)
    {
        float timer = 0f;

        while (timer <= duration)
        {
            timer += Time.unscaledDeltaTime;
            setter(lerp(from, to, timer / duration));
            yield return null;
        }
    }

    public static IEnumerator DelayedCall(float delay, Action call)
    {
        yield return new WaitForSeconds(delay);
        call.Invoke();
    }

    public static IEnumerator DelayedCrossFading<A>(float delay, A from, A to, float duration, Action<A> setter, Func<A, A, float, A> lerp)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0f;

        while (timer <= duration)
        {
            timer += Time.unscaledDeltaTime;
            setter(lerp(from, to, timer / duration));
            yield return null;
        }
    }

    public static IEnumerator CFLocalScale(Vector3 from, Vector3 to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (scale) => target.localScale = scale, (a, b, c) => Vector3.Lerp(a, b, c));
    }

    public static IEnumerator CFPosition(Vector3 from, Vector3 to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (pos) => target.position = pos, (a, b, c) => Vector3.Lerp(a, b, c));
    }

    public static IEnumerator CFLocalPosition(Vector3 from, Vector3 to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (pos) => target.localPosition = pos, (a, b, c) => Vector3.Lerp(a, b, c));
    }

    public static IEnumerator CFAnchoredPosition(Vector2 from, Vector2 to, float duration, RectTransform target)
    {
        return CrossFading(from, to, duration, (pos) => target.anchoredPosition = pos, (a, b, c) => Vector2.Lerp(a, b, c));
    }

    public static IEnumerator CFEulerAngle(Vector3 from, Vector3 to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (angle) => target.eulerAngles = angle, (a, b, c) => Vector3.Lerp(a, b, c));
    }

    public static IEnumerator CFLocalEulerAngle(Vector3 from, Vector3 to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (angle) => target.localEulerAngles = angle, (a, b, c) => Vector3.Lerp(a, b, c));
    }

    public static IEnumerator CFRotation(Quaternion from, Quaternion to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (rotation) => target.rotation = rotation, (a, b, c) => Quaternion.Lerp(a, b, c));
    }

    public static IEnumerator CFLocalRotation(Quaternion from, Quaternion to, float duration, Transform target)
    {
        return CrossFading(from, to, duration, (rotation) => target.localRotation = rotation, (a, b, c) => Quaternion.Lerp(a, b, c));
    }

    public static IEnumerator CFAlpha(float from, float to, float duration, Graphic target)
    {
        return CrossFading(from, to, duration,
            (alpha) =>
            {
                Color color = target.color;
                color.a = alpha;
                target.color = color;
            },
            (a, b, c) => Mathf.Lerp(a, b, c));
    }

    public static IEnumerator CFAlphaAll(float from, float to, float duration, GameObject target)
    {
        return CrossFading(from, to, duration,
            (alpha) =>
            {
                Color color;
                {
                    if (target.TryGetComponent(out Graphic graphic))
                    {
                        color = graphic.color;
                        color.a = alpha;
                        graphic.color = color;
                    }
                }

                {
                    foreach (Graphic graphic in target.GetComponentsInChildren<Graphic>())
                    {
                        color = graphic.color;
                        color.a = alpha;
                        graphic.color = color;
                    }
                }
            },
            (a, b, c) => Mathf.Lerp(a, b, c));
    }
}