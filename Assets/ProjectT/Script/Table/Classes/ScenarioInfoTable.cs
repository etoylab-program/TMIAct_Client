using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioInfoTable : ScriptableObject
{	

	// Excel Sheet : Info
	//
	public List<Info.Param> Infos = new List<Info.Param> ();

	[System.SerializableAttribute]
	public class Info
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public string Name;
			public long MinGroupId;
			public long MaxGroupId;
		}
	}

	public Info.Param FindInfo(string KeyValue)
	{
		return Infos.Find(x => x.Name == KeyValue);
	}

	public Info.Param FindInfo(System.Predicate<Info.Param> match)
	{
		return Infos.Find(match);
	}

	public List<Info.Param> FindAllInfo(System.Predicate<Info.Param> match)
	{
		return Infos.FindAll(match);
	}

}
