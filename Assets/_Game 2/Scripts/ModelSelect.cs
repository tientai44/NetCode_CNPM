using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSelect : MonoBehaviour
{
    List<GameObject> models= new List<GameObject>();
    [SerializeField] private int currentIndexSelected=0;
    public static int ModelCount;
    public GameObject CurrentModel { get { return models[currentIndexSelected]; } }

    private void OnInit()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            models.Add(transform.GetChild(i).gameObject);
        }   
        for(int i = 0; i < models.Count; i++)
        {
            models[i].SetActive(false);
        }
        if (ModelCount > 0)
        {
            return;
        }
        ModelCount = models.Count;
    }

    public GameObject ChooseModel(int index)
    {
        if (models.Count == 0)
        {
            OnInit();
        }
        if (index < 0)
        {
            index = models.Count + index;
        }
        if (index > models.Count - 1)
        {
            index = index - models.Count;
        }

        models[currentIndexSelected].SetActive(false);
        currentIndexSelected = index;
        models[currentIndexSelected].SetActive(true);
        return models[currentIndexSelected];
    }
}
