using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceGameManager : MonoBehaviour
{
    bool bIsInit = false;

    public float kFirstDistance = 100f;

    public int kLevelCnt = 3;
    public float kLevel_Length = 200f;
    public float kLevel_Interval = 50f;
    public List<GameObject> kLevel_Prefabs = new List<GameObject>();
    List<GameObject> levels;

    private RaceRider _player;
    //private void Awake() { Init(); }
    void Start() { Init(); }

    void Init()
    {
        if (bIsInit)
            return;

        //Utility.SetPhysicsLayerCollision(null, eLayer.DropItem, eLayer.Player);
        bIsInit = true;

        if (levels == null)
            levels = new List<GameObject>();
        
        if(kLevel_Prefabs.Count <= 0)
        {
            Debug.LogError("Level_Prefabs가 비어있습니다.");
            return;
        }

        float posZ = kFirstDistance + UnityEngine.Random.Range(50f, 100f);

        int prevIdx = -1;
        int curIdx = 0;

        for(int i = 0; i < kLevelCnt; i++)
        {
            curIdx = UnityEngine.Random.Range(0, kLevel_Prefabs.Count);
            if (prevIdx.Equals(curIdx))
            {
                i--;
                continue;
            }
            prevIdx = curIdx;
            GameObject obj = GameObject.Instantiate(kLevel_Prefabs[curIdx]);
            obj.transform.parent = this.transform;
            obj.transform.localPosition = new Vector3(0, 0, posZ);

            posZ += kLevel_Length;
            //posZ += UnityEngine.Random.Range(50f, 100f);
            posZ += kLevel_Interval;
            levels.Add(obj);
            if(i >= 3)
                obj.SetActive(false);
        }

        
        
    }

    private void Update()
    {
        if (World.Instance.IsEndGame)
            return;

        if (_player == null)
            return;

        for(int i = 3; i < levels.Count; i++)
        {
            if (levels[i].transform.position.z < _player.transform.position.z - kLevel_Length)
            {
                if(levels[i].activeSelf)
                    levels[i].SetActive(false);
                continue;
            }

            if (Vector3.Distance(levels[i].transform.position, _player.transform.position) < kLevel_Length * 3)
            {
                if (!levels[i].activeSelf)
                    levels[i].SetActive(true);
            }
        }
    }

    public void SetPlayer(RaceRider rider)
    {
        _player = rider;
    }
}
