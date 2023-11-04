using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINotify : MonoBehaviour
{
    [SerializeField] Image backGround;
    [SerializeField] TextMeshProUGUI notifyText;

    public void Notify(string message, float timeOut = 0)
    {
        backGround.DOKill();
        notifyText.DOKill();
        gameObject.SetActive(true);
        backGround.DOFade(1, 0f);
        notifyText.DOFade(1, 0f);
        notifyText.DOFade(0, 2f).SetEase(Ease.InQuart);
        backGround.DOFade(0, 2f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        notifyText.text = message;
    }

}
