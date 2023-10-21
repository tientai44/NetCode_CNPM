using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChooseModel : BasePopUp
{
    [SerializeField] GameObject Camera_Model;
    [SerializeField] List<ModelSelect> modelSelects = new List<ModelSelect>();
    [SerializeField] Button btnBack;
    [SerializeField] Button btnNext;
    [SerializeField] Button btnSelect;
    [SerializeField] Button btnHome;
    List<Vector3> showPostions;
    [SerializeField] private int currentSelectIndex;
    private float timeDelaySwitchModel = 0.5f;
    private bool isReadySwitch = true;
    private void Awake()
    {
        for(int i = 0; i < Camera_Model.transform.childCount; i++)
        {
            modelSelects.Add(Camera_Model.transform.GetChild(i).GetComponent<ModelSelect>());
        }
        showPostions = GetShowPositions();
        btnBack.onClick.AddListener(BackButton);
        btnNext.onClick.AddListener(NextButton);
        btnSelect.onClick.AddListener(SelectButton);
        btnHome.onClick.AddListener(HomeButton);
    }
    private void OnEnable()
    {
        currentSelectIndex = UserData.CurrentModelIndex;
        ShowModel();
        Camera_Model.SetActive(true);
    }
    private void OnDisable()
    {
        Camera_Model.SetActive(false);
    }
    public void ShowModel()
    {
        for (int i = 0; i < modelSelects.Count; i++)
        {
            modelSelects[i].ChooseModel(currentSelectIndex + i - 2);
        }
        UpdateButton();
    }
    public void UpdateButton()
    {
        if (currentSelectIndex == UserData.CurrentModelIndex)
        {
            btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
        }
        else
        {
            btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
        }
    }
    public List<Vector3> GetShowPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < modelSelects.Count; i++)
        {
            positions.Add(modelSelects[i].transform.position);
        }
        return positions;
    }
    public void UpdateShowModel(bool isNext)
    {
        List<Vector3> positions = showPostions;
        if (!isNext)
        {
            for(int i = 0; i < modelSelects.Count-1; i++)
            {
                modelSelects[i].transform.DOMove(positions[i+1], timeDelaySwitchModel);
            }
            modelSelects[modelSelects.Count - 1].transform.position = positions[0];
            modelSelects[modelSelects.Count - 1].ChooseModel(currentSelectIndex-2);
        }
        else
        {
            for (int i = modelSelects.Count - 1; i >0; i--)
            {
                modelSelects[i].transform.DOMove(positions[i - 1], timeDelaySwitchModel);
            }
            modelSelects[0].ChooseModel(currentSelectIndex + 2);
            modelSelects[0].transform.position = positions[modelSelects.Count - 1];
        }
        DOVirtual.DelayedCall(timeDelaySwitchModel, () =>
        {
            isReadySwitch = true;
        });
        UpdateButton();
    }
    private void NextButton()
    {
        if (!isReadySwitch)
        {
            return;
        }
        isReadySwitch = false;
        currentSelectIndex += 1;
        if (currentSelectIndex > ModelSelect.ModelCount - 1)
        {
            currentSelectIndex = 0;
        }
        UpdateShowModel(true);

        ModelSelect modelSelect = modelSelects[0];
        modelSelects.RemoveAt(0);
        modelSelects.Add(modelSelect);

;       
    }
    private void BackButton()
    {
        if (!isReadySwitch)
        {
            return;
        }
        isReadySwitch = false;
        currentSelectIndex -= 1;
        if (currentSelectIndex < 0)
        {
            currentSelectIndex = ModelSelect.ModelCount - 1;
        }

        UpdateShowModel(false);
        ModelSelect modelSelect = modelSelects[modelSelects.Count - 1];
        modelSelects.RemoveAt(modelSelects.Count - 1);
        List<ModelSelect> temp = new List<ModelSelect>();
        temp.Add(modelSelect);
        temp.AddRange(modelSelects);

        modelSelects = temp;
        
    }
    private void SelectButton()
    {
        UserData.CurrentModelIndex = currentSelectIndex;
        GameObject model = modelSelects[2].CurrentModel;
        model.GetComponent<Animator>().Play(G2_PlayerState.Attack.ToString());
        btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
    }
    private void HomeButton()
    {
        Hide();
    }
}
