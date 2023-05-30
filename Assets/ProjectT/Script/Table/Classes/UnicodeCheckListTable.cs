using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnicodeCheckListTable : ScriptableObject
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
			
			public int Min;
			public int Max;
		}
	}

	public Info.Param FindInfo(int KeyValue)
	{
		return Infos.Find(x => x.Min == KeyValue);
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
