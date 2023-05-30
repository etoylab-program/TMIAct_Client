using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResultGemGradeUpPopup : FComponent
{
    [Header("GemView")]
    public GameObject kGemView;

    [Header("GemInfo")]
    public GameObject kGemInfo;
    public UILabel kGemNameLabel;
    public List<GameObject> kGemGradetList;
    public List<UIGemOptUnit> kGemOptList;
    public List<GameObject> kEffGemOptList;

    [Header("Buttons")]
    public UIButton kConfirmBtn;

    [Header("Add Gem UR Grade")]
    [SerializeField] private UISprite titleSpr = null;
    [SerializeField] private GameObject optObj = null;
    [SerializeField] private GameObject evolutionObj = null;

    private GemData _gemdata;
    private bool _isEvolution;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.GemUID);
        _gemdata = GameInfo.Instance.GetGemData(uid);
        kGemInfo.SetActive(true);

        for (int i = 0; i < kEffGemOptList.Count; i++)
            kEffGemOptList[i].SetActive(false);

        kGemView.SetActive(!_isEvolution);
        optObj.SetActive(!_isEvolution);
        evolutionObj.SetActive(_isEvolution);

        StartCoroutine(ResultCoroutine());
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        FLocalizeString.SetLabel(kGemNameLabel, _gemdata.TableData.Name);

        if (_isEvolution)
        {
            titleSpr.spriteName = "Txt_Evolution_Gem";
        }
        else
        {
            titleSpr.spriteName = "Txt_GradeUp_Gem";

            for (int i = 0; i < kGemGradetList.Count; i++)
            {
                kGemGradetList[i].SetActive(false);
            }

            for (int i = 0; i < kGemOptList.Count; i++)
            {
                kGemOptList[i].gameObject.SetActive(true);
                kGemOptList[i].Lock();
            }

            for (int i = 0; i < _gemdata.Wake - 1; i++)
            {
                kGemOptList[i].Opt(_gemdata, i);
            }
        }
    }

    public void OnClick_ConfirmBtn()
    {
        DirectorUIManager.Instance.StopItemWakeUp();
    }

    IEnumerator ResultCoroutine()
    {
        if (_isEvolution)
        {
            yield return null;
        }
        else
        {
            float fopeani = GetOpenAniTime();
            yield return new WaitForSeconds(fopeani);

            SoundManager.Instance.PlayUISnd(24);
            kEffGemOptList[_gemdata.Wake - 1].SetActive(true);
            yield return new WaitForSeconds(1.0f);


            kGemOptList[_gemdata.Wake - 1].Opt(_gemdata, _gemdata.Wake - 1, true);
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void SetEvolution(bool isEvolution)
    {
        _isEvolution = isEvolution;
    }

    public override bool IsBackButton()
    {
        return false;
    }

}
