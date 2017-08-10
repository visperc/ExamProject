using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;


public enum ExamType
{
    SingleChoice,
    MultiChoice,
    Judge
}

public class EAFileLoaderManager
{
    private static  EAFileLoaderManager m_instance;
    public static EAFileLoaderManager Instance 
    {
        get
        {
            if (m_instance == null)
                m_instance = new EAFileLoaderManager();
            return m_instance;
        }
    }

    private EAFileLoaderManager()
    {
    }

    List<EAExamPaper> m_papers = new List<EAExamPaper>();
    public List<EAExamPaper> Papers {get { return m_papers;}}

    public void Init()
    {
        TextAsset[] datas = Resources.LoadAll<TextAsset>("Config");

        for(int i = 0 ; i < datas.Length ; i++)
        {
            EAExamPaper p = LoadDataFromAsset(datas[i]);
            m_papers.Add(p);
        }
    }

    public EAExamPaper LoadDataFromAsset(TextAsset asset)
    {
        EAExamPaper paper = new EAExamPaper();
        try
        {
            string text = asset.text;
            string[] ItemInfo = text.Split('\n','\r');
            //start from row 1
            for (int i = 1; i < ItemInfo.Length; i++)
            {
                if(string.IsNullOrEmpty(ItemInfo[i]))
                    continue;

                string[] ItemData = ItemInfo[i].Split(new char[] { '|' });
                EAExamQuestion q = new EAExamQuestion();
               
                q.ParseCSV(ItemData);
               
                if(paper.Questions.ContainsKey(q.Question))
                    Debug.LogWarning(q.Question);
                else
                    paper.Questions.Add(q.Question ,q);

//                Debug.LogWarning(q.Question);
                
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            paper = null;
        }
        return paper;
    }
}

