using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingCounter : MonoBehaviour
{
    private float offset = 0.0f;
    private Transform followTransform = null;
    private TextMeshProUGUI text;
    private float time = 20f;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Show(float time, Transform transform, float offset)
    {
        Destroy(gameObject, time);
        followTransform = transform;
        this.time = time;
        this.offset = offset;
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while (time >= 0.0f) {
            yield return new WaitForSeconds(0.1f);
            time -= 0.1f;
            text.text = time.ToString("F1") + "s";
        }
    }

    void LateUpdate()
    {
        if (followTransform != null) {
            RectTransform myRect = GetComponent<RectTransform>();
            transform.position = RectTransformUtility.WorldToScreenPoint(null, (followTransform.position + new Vector3(0, offset, 0)));
        }
    }
}
