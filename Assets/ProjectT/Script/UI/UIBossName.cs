
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIBossName : MonoBehaviour
{
    [Header("[Property]")]
    public UILabel LbMidName;
    public UILabel LbName;


    public void Show(string midName, string name)
    {
        gameObject.SetActive(true);

        LbMidName.text = midName;
        LbName.text = name;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
