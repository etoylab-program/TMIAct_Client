
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FigureRoomFunc : MonoBehaviour
{
    [Header("[Property]")]
    public int renderQueue = 0;

    public GameTable.RoomFunc.Param Param { get; private set; } = null;

    private List<Renderer> mListRenderer = new List<Renderer>();
    

    public void Init()
    {
        mListRenderer.Clear();
        mListRenderer.AddRange(GetComponentsInChildren<Renderer>());

        for (int i = 0; i < mListRenderer.Count; i++)
        {
            mListRenderer[i].material.renderQueue = renderQueue;
        }

        Param = GameInfo.Instance.GameTable.FindRoomFunc(x => x.Function == name);
		Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
