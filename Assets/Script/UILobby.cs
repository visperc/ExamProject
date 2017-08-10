using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

public enum PaperType
{
    Option,
    Judgement
}

public class UILobby : MonoBehaviour 
{
    public InputField m_input;
    public Button m_inputConfirm;
    public Text m_content;
    public ScrollRect m_questionList;
    public VerticalLayoutGroup m_gridGroup;

    void Start()
    {
        m_inputConfirm.onClick.AddListener(OnConfirmButtonClick);
    }

    void OnConfirmButtonClick()
    {
//        ClearQuestionNode();
//        m_currentResult = m_input.text;
        m_content.text = string.Empty;

        List<EAExamPaper> m_papers = EAFileLoaderManager.Instance.Papers;
        StringBuilder questionBuilder = new StringBuilder();

        for(int i = 0 ; i < m_papers.Count ; i++)
        {
            EAExamPaper paper = m_papers[i];
            foreach(KeyValuePair<string , EAExamQuestion> pair in paper.Questions)
            {
                string question = pair.Key;
                if (pair.Key.Contains(m_input.text))
                {
                    questionBuilder.AppendLine(question);
                    PaperType type = GetPaperType(pair.Value.Options);
                    for (int j = 0; j < pair.Value.Options.Count; j++)
                    {
                        string option = pair.Value.Options[j];
                        if (string.IsNullOrEmpty(option))
                            continue;
                        if(type == PaperType.Judgement)
                            questionBuilder.AppendLine(option);
                        else
                            questionBuilder.AppendLine(IntToLetters(j + 1) + " . " + option);
                    }
//                    StringBuilder answerBuilder = new StringBuilder();
                    questionBuilder.AppendLine();
                    questionBuilder.Append("答案 ： ");
                    for(int j = 0 ; j < pair.Value.Answers.Count ; j++)
                    {
                        string answer = pair.Value.Answers[j];
                        if (string.IsNullOrEmpty(answer))
                            continue;
                      
//                        if (answer == "是" || answer == "否")
                        if(type == PaperType.Judgement)
                            questionBuilder.Append(IntToJudgement(int.Parse(answer)));
                        else
                            questionBuilder.Append(IntToLetters(int.Parse(answer)));
                    }
                    questionBuilder.AppendLine(""); 
                    foreach(string note in pair.Value.Notes)
                    {
                        questionBuilder.AppendLine(note); 
                    }
                    questionBuilder.AppendLine("");
                    questionBuilder.AppendLine("");
                    questionBuilder.AppendLine("");
                    questionBuilder.AppendLine("");
                    questionBuilder.AppendLine("");

                    AddQuestionNode(questionBuilder.ToString());
//                    Debug.LogError(questionBuilder.ToString());
                }
            }
        }
        m_content.text = questionBuilder.ToString();
    }

    public PaperType GetPaperType(List<string> options)
    {
        int optCount = 0;
        for(int i = 0 ; i < options.Count ; i++)
        {
            optCount = string.IsNullOrEmpty(options[i]) ? optCount : optCount + 1;
        }
        return optCount == 2 ? PaperType.Judgement : PaperType.Option;
    }

    public string IntToJudgement(int value)
    {
        return value == 1 ? "正确" : "错误";
    }

    public string IntToLetters(int value)
    {
        string result = string.Empty;
        while (--value >= 0)
        {
            result = (char)('A' + value % 26 ) + result;
            value /= 26;
        }
        return result;
    }

    public void AddQuestionNode(string content)
    {
        GameObject questionIndexPrefab = Resources.Load("questionIndex") as GameObject;
        GameObject q = GameObject.Instantiate(questionIndexPrefab, Vector3.zero, Quaternion.identity, m_gridGroup.transform) as GameObject;
        Text t = q.GetComponent<Text>();
        t.text = content;
    }

    public void ClearQuestionNode()
    {
        for(int i = 0 ; i < m_gridGroup.transform.childCount; i++)
        {
            GameObject.Destroy(m_gridGroup.transform.GetChild(i).gameObject);   
        }
    }
}

