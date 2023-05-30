using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScoreText : MonoBehaviour
{
    public UILabel kScoreLb;
    public UILabel kComboLb;
    public float kDuration;

    private int _curScore;
    public bool IsHide { get; private set; } = true;


    void Start()
    {
        kScoreLb.gameObject.SetActive(false);
        kComboLb.gameObject.SetActive(false);
    }

    public void ShowScore(int score, int combo, Vector3 pos)
    {
        if (score.Equals(0))
            return;

        this.transform.position = pos;
        
        _curScore = score;

        kScoreLb.textlocalize = string.Format("+{0}", _curScore);
        kComboLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(2006), combo);
        GameSupport.TweenerPlay(kScoreLb.gameObject);
        GameSupport.TweenerPlay(kComboLb.gameObject);
        IsHide = false;

        StartCoroutine(Hide());
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(kDuration);
        IsHide = true;

        kScoreLb.gameObject.SetActive(false);
        kComboLb.gameObject.SetActive(false);
    }
    
}
