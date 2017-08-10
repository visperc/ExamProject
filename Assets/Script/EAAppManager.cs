using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAAppManager : MonoBehaviour 
{


	// Use this for initialization
	void Start () 
    {
        InitManagers();
	}
	
    public void InitManagers()
    {
        EAFileLoaderManager.Instance.Init();
    }
}
