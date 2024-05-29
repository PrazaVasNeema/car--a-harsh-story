using System;
using System.Collections.Generic;
using DopeRP.CPU;
using UnityEngine;

[CreateAssetMenu(menuName = "DopeRP/MainSolutions")]
public class MainSolutionsSettings : ScriptableObject
{
    

    
    [Serializable] 
    public struct MainSolutionChoice
    {
        public bool solutionIsOn;
        public GraphicalSolutionAbstract solution;
    }
    public List<MainSolutionChoice> currentMainSolutionsList;
    
}