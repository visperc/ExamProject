using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EAExamQuestion
{
    public string Question { get ; private set;}
    public List<string> Options { get ; private set;}
    public List<string> Answers { get ; private set;}
    public List<string> Notes { get ; private set;}

    public void ParseCSV(string[] itemDatas)
    {
        CSVUtility uti = new CSVUtility(itemDatas);
        Question = uti.GetString;
        for (int i = 0; i < 5; i++)
        {
            if (Options == null)
                Options = new List<string>();
            Options.Add(uti.GetString);
        }

        for (int i = 0; i < 5; i++)
        {
            if (Answers == null)
                Answers = new List<string>();
            Answers.Add(uti.GetString);
        }

        for (int i = 0; i < 2; i++)
        {
            if (Notes == null)
                Notes = new List<string>();
            Notes.Add(uti.GetString);
        }
    }
}

public class EAExamPaper
{
    private Dictionary<string , EAExamQuestion> m_questions = new Dictionary<string, EAExamQuestion>();
    public Dictionary<string , EAExamQuestion> Questions {get { return m_questions;}}
}

