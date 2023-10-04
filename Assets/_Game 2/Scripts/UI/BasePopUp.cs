using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePopUp : MonoBehaviour
{
    public virtual void Show()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f);
    }
    public virtual void Hide()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.zero, 1f);
    }
    
}
