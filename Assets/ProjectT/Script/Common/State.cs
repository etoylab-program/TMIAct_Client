
using System;
using System.Collections.Generic;


public class State
{
    public class sStateData
    {
        public Enum				Id;
        public Func<bool, bool>	Callback;
    }


	public Enum current { get { return mList[mCurrentIndex].Id; } }

	private List<sStateData>	mList;// = new List<sStateData>();
    private int					mCurrentIndex;


    public void Init(int count)
    {
        //mList.Clear();
		if(mList == null)
		{
			mList = new List<sStateData>(count);
		}

		mList.Clear();
		mCurrentIndex = 0;
    }

    public void Bind(Enum id, Func<bool, bool> callback)
    {
        sStateData data = mList.Find(x => x.Id == id);
		if (data != null)
		{
			return;
		}

        data = new sStateData();
        data.Id = id;
        data.Callback = callback;

        mList.Add(data);
    }

    public void ChangeState(Enum id, bool param)
    {
		bool check = false;

		for (int i = 0; i < mList.Count; i++)
        {
            if(mList[i].Id.Equals(id) == true)
            {
				check = true;
				if (mList[i].Callback != null)
				{
					check = mList[i].Callback(param);
				}

				if (check)
				{
					mCurrentIndex = i;
				}
            }
        }
    }
}
