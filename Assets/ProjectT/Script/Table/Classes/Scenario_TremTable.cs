using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scenario_TremTable : ScriptableObject
{	

	// Excel Sheet : Trem
	//
	public List<Trem.Param> Trems = new List<Trem.Param> ();

	[System.SerializableAttribute]
	public class Trem
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public string Title_KOR;
			public string Desc_KOR;
			public string Title_JPN;
			public string Desc_JPN;
			public string Title_ENG;
			public string Desc_ENG;
			public string Title_CHS;
			public string Desc_CHS;
			public string Title_CHT;
			public string Desc_CHT;
			public string Title_ESP;
			public string Desc_ESP;
		}
	}

	public Trem.Param FindTrem(int KeyValue)
	{
		return Trems.Find(x => x.Group == KeyValue);
	}

	public Trem.Param FindTrem(System.Predicate<Trem.Param> match)
	{
		return Trems.Find(match);
	}

	public List<Trem.Param> FindAllTrem(System.Predicate<Trem.Param> match)
	{
		return Trems.FindAll(match);
	}

}
