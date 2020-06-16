using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake (float duration, float magnitude) {

        List<Tuple<float, float>> path = new List<Tuple<float, float>>();
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration / 2) {
            float x = UnityEngine.Random.Range(-0.05f, 0.05f) * magnitude;
            float y = UnityEngine.Random.Range(-0.05f, 0.05f) * magnitude;
            path.Add(new Tuple<float, float>(x, y));

            transform.localPosition += new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        elapsed = 0.0f;
        int index = path.Count - 1;
        while (elapsed < duration / 2) {
            path.Reverse();

            transform.localPosition -= new Vector3(path[index].Item1, path[index].Item2, 0);
            index--;

            elapsed += Time.deltaTime;

            if (index < 0) break;

            yield return null;
        }
        
        GetComponent<CameraLogic>().ReFocus();
    }

}
