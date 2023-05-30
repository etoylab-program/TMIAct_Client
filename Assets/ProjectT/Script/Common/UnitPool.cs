
using UnityEngine;
using System.Collections.Generic;


public class UnitPool
{
    public int SpawnUnitCount { get; private set; } = 0;

    private Dictionary<int, List<Unit>> mDicPool = new Dictionary<int, List<Unit>>();


    public UnitPool()
    {
        SpawnUnitCount = 0;
    }

    public void CreatePool(int tableId, string path, int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (HasPool(tableId) == true)
        {
            Debug.LogError(tableId + "은 이미 존재하는 풀입니다.");
            return;
        }

        Unit unit = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", path);
        if (unit == null)
        {
            return;
        }

        mDicPool.Add(tableId, new List<Unit>());
        for (int i = 0; i < count; i++)
        {
            unit.gameObject.SetActive(false);
            mDicPool[tableId].Add(unit);
        }
    }

    public bool HasPool(int tableId)
    {
        return mDicPool.ContainsKey(tableId);
    }

    public Unit AddUnit(int tableId, string path)
    {
        Unit unit = null;
        if (HasPool(tableId) == false)
        {
            CreatePool(tableId, path, 1);

            unit = GetUnit(tableId);
            return unit;
        }

        unit = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", path);
        if (unit == null)
        {
            return null;
        }

        unit.gameObject.SetActive(false);
        mDicPool[tableId].Add(unit);

        return unit;
    }

    public Unit GetUnit(int tableId)
    {
        if (HasPool(tableId) == false)
        {
            return null;
        }

        Unit unit = mDicPool[tableId].Find(x => x.tableId == tableId && !x.IsActivate());
        if(unit)
        {
            ++SpawnUnitCount;
        }

        return unit;
    }

    public List<Unit> GetUnits(int tableId, int count)
    {
        if (HasPool(tableId) == false)
        {
            Debug.LogError(tableId + "은 존재하지 않는 풀입니다.");
            return null;
        }

        if(count > mDicPool[tableId].Count)
        {
            Debug.LogError(tableId + "풀에 존재하는 수보다 많은 수를 요청했습니다.");
            return null;
        }

        List<Unit> listUnit = mDicPool[tableId].FindAll(x => x.IsActivate() == false);
        if(listUnit.Count < count)
        {
            Debug.LogError(tableId + "풀 안에 비활성화된 유닛이 요청한 수보다 적습니다.");
            return null;
        }

        SpawnUnitCount += listUnit.Count;
        return listUnit;
    }

    public void ReturnToPool(Unit unit)
    {
        unit.gameObject.SetActive(false);
        --SpawnUnitCount;
    }

    public void ReturnAllToPool(int tableId)
    {
        if (HasPool(tableId) == false)
        {
            Debug.LogError(tableId + "은 존재하지 않는 풀입니다.");
            return;
        }

        for (int i = 0; i < mDicPool[tableId].Count; i++)
        {
            ReturnToPool(mDicPool[tableId][i]);
        }
    }

    public void DestroyPool(int tableId)
    {
        if (HasPool(tableId) == false)
        {
            Debug.LogError(tableId + "은 존재하지 않는 풀입니다.");
            return;
        }

        for (int i = 0; i < mDicPool[tableId].Count; i++)
        {
            GameObject.DestroyImmediate(mDicPool[tableId][i].gameObject);
            --SpawnUnitCount;
        }

        mDicPool.Remove(tableId);
    }

    public void DestroyAll()
    {
        foreach (KeyValuePair<int, List<Unit>> kv in mDicPool)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {
                GameObject.DestroyImmediate(kv.Value[i].gameObject);
            }
        }

        mDicPool.Clear();
        SpawnUnitCount = 0;
    }
}