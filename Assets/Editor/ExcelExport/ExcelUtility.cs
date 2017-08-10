using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;
using UnityEditor;

public class ExcelUtility 
{
	[UnityEditor.MenuItem("CustomBuild/ExportCSVFromExcel")]
	public static void ExportCSVFromExcel()
	{
		string dirpath = Application.dataPath;
		dirpath = dirpath.Substring(0, dirpath.Length - 6);
		dirpath += "ExcelConfig/";
		DirectoryInfo exceldir = new DirectoryInfo(dirpath);
		FileInfo[] files = exceldir.GetFiles();

		foreach(FileInfo file in files)
		{
			string[] fileStr = file.Name.Split(new char[]{ '.' });
			string fileType = fileStr[fileStr.Length - 1];
//			if(fileType == "meta" || 
//			   fileType == "DS_Store")
//			{
//				continue;
//			}
			if(fileType == "xlsx")
			{
				string fileName = file.Name.Substring(0, file.Name.Length - fileType.Length - 1);
                ExportNormalExcel(dirpath, fileName);
			}
		}

//		LoadLevelData(dirpath);
		AssetDatabase.Refresh();
		Debug.Log("ExportCSV Success");
	}

	private static void ExportNormalExcel(string dirpath, string ExcelFileName)
	{
		FileStream stream = File.Open(dirpath + ExcelFileName + ".xlsx" , FileMode.Open, FileAccess.Read);
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
		
		DataSet result = excelReader.AsDataSet();
		Debug.Log("Load :" + ExcelFileName);
		for(int i = 0; i < result.Tables.Count; i++) 
		{
            string text = "";
			if(ExcelFileName == "SensitiveWordsData")
			{
				text = GetSensitiveWordsTable(result.Tables[i]);
			}
			else if(result.Tables[i].TableName == "LotteryDrawData" || result.Tables[i].TableName == "SpeLotteryData" || result.Tables[i].TableName == "DivinationData")
			{
				text = GetLotteryDrawTable(result.Tables[i]);
			}
            else
            {
                text = GetTableString(result.Tables[i]);
            }
			
			string savePath = "";
			if(result.Tables.Count == 1)
			{
				savePath = Application.dataPath + "/Resources/Config/" + ExcelFileName + ".csv";
			}
			else
			{
				savePath = Application.dataPath + "/Resources/Config/" + result.Tables[i].TableName + ".csv";
			}
			 
			FileInfo t = new FileInfo (savePath);
			if (t.Exists)
			{
				t.Delete();
			}
			using(StreamWriter sw = t.CreateText())
			{
				sw.WriteLine(text);
			}
		}
		stream.Close();
		stream.Dispose();
	}

//	private static void LoadLevelData(string dirpath)
//	{
//		string leveldatapath = dirpath + "LevelData/";
//		DirectoryInfo exceldir = new DirectoryInfo(leveldatapath);
//		FileInfo[] files = exceldir.GetFiles();
//
//		string leveldataStr = "id,LevelName,LevelLand,LimitType,LimitValue,Star1Score,Star2Score,Star3Score,Enemy,SpeObject,Plot\n";
//		List<CompareFileName> filenames = new List<CompareFileName>();
//		foreach(FileInfo file in files)
//		{
//			string[] fileStr = file.Name.Split(new char[]{ '.' });
//			string fileType = fileStr[fileStr.Length - 1];
////			if(fileType == "meta" || 
////			   fileType == "DS_Store")
////			{
////				continue;
////			}
//			if(fileType == "xlsx")
//			{
//				string fileName = file.Name.Substring(0, file.Name.Length - fileType.Length - 1);
//				filenames.Add(new CompareFileName(fileName));
//			}
//		}
//		filenames.Sort();
//		filenames.ForEach(s=>leveldataStr += LoadOneLevelDataTabel(leveldatapath, s.fileName) + "\n");
//
//		
//
//		string savePath = Application.dataPath + "/Resources/Config/LevelData.csv";
//		FileInfo t = new FileInfo (savePath);
//		if (t.Exists)
//		{
//			t.Delete();
//		}
//		using(StreamWriter sw = t.CreateText())
//		{
//			sw.WriteLine(leveldataStr);
//		}
//	}

//	private static string LoadOneLevelDataTabel(string dirpath, string ExcelFileName)
//	{
//		string text = "";
//
//		FileStream stream = File.Open(dirpath + ExcelFileName + ".xlsx" , FileMode.Open, FileAccess.Read);
//		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
//		
//		DataSet result = excelReader.AsDataSet();
//
//		text = GetTableForSpaceString(result.Tables[0], 1);
//		text += "," + GetSpaceString(result.Tables[1], 1);//add enemy
//		text += "," + GetSpaceString(result.Tables[2], 1);//add speObject
//		text += "," + GetSpaceString(result.Tables[3], 1);//add Plot config
//		stream.Close();
//		stream.Dispose();
//
//		return text;
//	}

	private static string GetTableString(DataTable table, int rowStartFrom = 0)
	{
		string text = "";

		int columns = table.Columns.Count;
		int rows = table.Rows.Count;
		List<int> IgnoreColumns = new List<int>();
		for(int j = 0; j < columns; j++)
		{
			string nvalue  = table.Rows[0][j].ToString();
			if(nvalue.StartsWith("*"))
			{
				IgnoreColumns.Add(j);
			}
		}

		for(int i = rowStartFrom;  i< rows; i++)
		{
			bool havevalue = false;
			string rowstring = "";
			for(int j =0; j < columns; j++)
			{
				if(!IgnoreColumns.Exists(s => s == j ? true : false))
				{
					string nvalue  = table.Rows[i][j].ToString();
                    nvalue = nvalue.Replace("\n" , "");
					rowstring += nvalue + "|";
					if(!string.IsNullOrEmpty(nvalue))
					{
						havevalue = true;
					}
				}
			}
			if(havevalue)
			{
                if(i != rowStartFrom)
                {
                    text += "\n";
                }
                text += rowstring;
				text = text.Substring (0, text.Length - 1);
			}
		}
        //if(text.Length > 0)
        //    text = text.Substring(0, text.Length - 1);

        return text;
	}

    private static string GetSensitiveWordsTable(DataTable table, int rowStartFrom = 0)
	{
		string text = "";

		int columns = table.Columns.Count;
		int rows = table.Rows.Count;

		for(int i = rowStartFrom; i < rows; i++)
		{
			if(i == 0)
			{
				bool havevalue = false;
				string rowstring = "";
				for(int j = 0; j < columns; j++)
				{
					string nvalue = table.Rows[i][j].ToString();
					rowstring += nvalue + ",";
					if(!string.IsNullOrEmpty(nvalue))
					{
						havevalue = true;
					}
				}
				if(havevalue)
				{
					text += rowstring;
					text = text.Substring(0, text.Length - 1);
					text += "\n";
				}
			}
			else
			{
				bool havevalue = false;
				string rowstring = "";
				for(int j = 0; j < columns; j++)
				{
					string nvalue = table.Rows[i][j].ToString();
					if(!string.IsNullOrEmpty(nvalue))
					{
						rowstring += nvalue + "|";
						havevalue = true;
					}
				}
				if(havevalue)
				{
					text += rowstring;
					// text = text.Substring(0, text.Length - 1);
				}
			}
		}
		if(text.Length > 0)
			text = text.Substring(0, text.Length - 1);

		return text;
	}
     
	private static string GetLotteryDrawTable(DataTable table, int rowStartFrom = 0)
	{
		string text = "";

		int columns = table.Columns.Count;
		int rows = table.Rows.Count;
		List<int> IgnoreColumns = new List<int>();
		for(int j = 0; j < columns; j++)
		{
			string nvalue  = table.Rows[0][j].ToString();
			if(nvalue.StartsWith("*"))
			{
				IgnoreColumns.Add(j);
			}
		}

		for(int i = rowStartFrom;  i< rows; i++)
		{
			bool havevalue = false;
			string rowstring = "";
			for(int j =0; j < 4; j++)
			{
				if(!IgnoreColumns.Exists(s => s == j ? true : false))
				{
					string nvalue  = table.Rows[i][j].ToString();
					rowstring += nvalue + ",";
					if(!string.IsNullOrEmpty(nvalue))
					{
						havevalue = true;
					}
				}
			}

			if(havevalue && columns > 4 && (columns - 4) % 2 == 0)
			{
				for(int j = 4; j < columns; j++)
				{
					string nvalue  = table.Rows[i][j].ToString();
					if(j % 2 == 0)
					{
						rowstring += string.Format("{0}\"", nvalue);
					}
					else
					{
						rowstring += string.Format("{0}|", nvalue);
					}
				}
				rowstring += ",";
			}

			if(havevalue)
			{
				if(i != rowStartFrom)
				{
					text += "\n";
				}
				text += rowstring;
				text = text.Substring (0, text.Length - 1);
			}
		}
		//if(text.Length > 0)
		//    text = text.Substring(0, text.Length - 1);

		return text;
	}

    //private static string GetSpaceString(DataTable table, int rowStartFrom = 0)
    //{
    //	string text = "";

    //	int columns = table.Columns.Count;
    //	int rows = table.Rows.Count;

    //	for(int i = rowStartFrom;  i< rows; i++)
    //	{
    //		for(int j =0; j < columns; j++)
    //		{
    //			string nvalue  = table.Rows[i][j].ToString();
    //			if(!string.IsNullOrEmpty(nvalue))
    //			{
    //				text += nvalue + "\"";
    //			}
    //		}
    //		if(text.Length > 0)
    //		{
    //			text = text.Substring (0, text.Length - 1);
    //		}
    //		text += "|";
    //	}
    //	if(!string.IsNullOrEmpty(text.Trim()) && text.Length > 0)
    //		text = text.Substring (0, text.Length - 1);

    //	return text;
    //}

    //private static string GetTableForSpaceString(DataTable table, int rowStartFrom = 0)
    //{
    //	string text = "";

    //	int columns = table.Columns.Count;
    //	int rows = table.Rows.Count;

    //	for(int i = rowStartFrom;  i< rows; i++)
    //	{
    //		for(int j =0; j < columns; j++)
    //		{
    //			string nvalue  = table.Rows[i][j].ToString();
    //			if(!string.IsNullOrEmpty(nvalue))
    //			{
    //				text += nvalue + ",";
    //			}
    //		}
    //		text = text.Substring (0, text.Length - 1);
    //		text += "\n";
    //	}	
    //	if(text.Length > 0)
    //	text = text.Substring (0, text.Length - 1);

    //	return text;
    //}
}

public class CompareFileName : IComparable
{
	private string m_fileName;
	public  string fileName{ get{ return m_fileName; }}
	private int    m_Level;
	public  int	   Level{ get{ return m_Level; }}

	public CompareFileName(string filename)
	{
		m_fileName = filename;
		m_Level = int.Parse(m_fileName.Substring(9).Trim());
	}
	
	//result > 0 mean's self is bigger
	public int CompareTo(object obj)
	{
		int result = 0;
		
		CompareFileName filename = obj as CompareFileName;
		result = m_Level - filename.Level;


		return result;
	}
}
