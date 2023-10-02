using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    TextMeshProUGUI txt;

    private void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        StartCoroutine(IEUpdateText());
    }
    IEnumerator IEUpdateText()
    {
        yield return new WaitForSeconds(0.1f);
        txt.text = (1 / Time.deltaTime).ToString("#.##");
        StartCoroutine(IEUpdateText());
    }
  
}
