using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using SimpleJson;
using System;
using System.Globalization;

public class CSVUtility
{
	private int m_Count;
	private string[] m_ItemData;

	public CSVUtility(string[] itemdata)
	{
		m_Count = -1;
		m_ItemData = itemdata;
	}

	public int GetInt
	{
		get
		{
			int result = 0;
			try
			{
				if(m_Count + 1 < m_ItemData.Length)
				{	
					string str = m_ItemData[++m_Count].Trim();
					if(!string.IsNullOrEmpty(str))
					{
						result = int.Parse(str);
					}

				}
			}
			catch(Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			return result;
		}
	}

	public string GetString
	{
		get
		{
			string result = "";
			try
			{
				if(m_Count + 1 < m_ItemData.Length)
				{
					result = m_ItemData[++m_Count].ToString();
				}
			}
			catch(Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			return result;
		}
	}

	public float GetFloat
	{
		get
		{
			float result = 0;
			try
			{
				if(m_Count + 1 < m_ItemData.Length)
				{
					string str = m_ItemData[++m_Count].Trim();
					if(!string.IsNullOrEmpty(str))
					{
						result = float.Parse(str);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			return result;
		}
	}
	public List<float> GetFloatList
	{
		get
		{
			List<float> result = new List<float>();
			try
			{
				if(m_Count + 1 < m_ItemData.Length)
				{
					string listStr = m_ItemData[++m_Count];
					string[] list = listStr.Split('|');
					for(int i = 0; i < list.Length; i++)
					{
						if(!string.IsNullOrEmpty(list[i]))
						{
							result.Add(float.Parse(list[i]));
						}
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			return result;
		}
	}

	public bool GetBool
	{
		get
		{
			bool result = false;
			try
			{
				if(m_Count + 1 < m_ItemData.Length)
				{
					string str = m_ItemData[++m_Count].Trim();
					if(!string.IsNullOrEmpty(str))
					{
						result = str == "1" ? true : false;
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogError(ex.Message);
			}
			return result;
		}
	}
}