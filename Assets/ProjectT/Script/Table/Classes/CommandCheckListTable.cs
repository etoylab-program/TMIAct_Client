using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandCheckListTable : ScriptableObject
{
    public List<Keyword.Param> Keywords = new List<Keyword.Param>();

    [System.SerializableAttribute]
    public class Keyword
    {
        [System.SerializableAttribute]
        public class Param
        {
            public string keyword;
        }
    }

    public Keyword.Param FindString(string KeyValue)
    {
        return Keywords.Find(x => x.keyword == KeyValue);
    }

    public Keyword.Param FindString(System.Predicate<Keyword.Param> match)
    {
        return Keywords.Find(match);
    }

    public List<Keyword.Param> FindAllString(System.Predicate<Keyword.Param> match)
    {
        return Keywords.FindAll(match);
    }
}
