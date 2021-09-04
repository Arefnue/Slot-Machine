using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace SlotMachine
{
    public static class SlotMachineLogic
    {
        private static ScoreTemplateContainer _selectedContainer;
        private static List<ScoreTemplateContainer> _scoreTemplateContainerList = new List<ScoreTemplateContainer>();
        public static void InitScoreTemplateContainers(ScoreChanceData scoreChanceData)
        {
             scoreChanceData.scoreList.ForEach(x=>_scoreTemplateContainerList.Add(new ScoreTemplateContainer(x)));
        }
        
        public static ScoreTemplate GetMostPossibleScoreTemplate(int currentSpinCount)
        {
            ScoreTemplate targetTemplate = null;
            
            _selectedContainer = _scoreTemplateContainerList[0];
            float bestChance = 0;
            _scoreTemplateContainerList.Shuffle();
            foreach (var scoreTemplateContainer in _scoreTemplateContainerList)
            {
                var mod = currentSpinCount %  scoreTemplateContainer.MyBlock;

                if (mod == 0)
                {
                    scoreTemplateContainer.isOpen = true;
                }
                
                var newChance = (float)1 / (scoreTemplateContainer.MyBlock - mod);
                
                if (scoreTemplateContainer.isOpen && newChance>bestChance)
                {
                    bestChance = newChance;
                    _selectedContainer = scoreTemplateContainer;
                }
            }

            if (bestChance <=0)
            {
                _selectedContainer.isOpen = true;
            }
            
            if (_selectedContainer.isOpen)
            {
                targetTemplate = _selectedContainer.myTemplate;
                _selectedContainer.isOpen = false;
            }

            return targetTemplate;
        }
    }
    
    [Serializable]
    public class ScoreTemplateContainer
    {
        public ScoreTemplate myTemplate;
        public int MyBlock { get; private set; }
        public bool isOpen;

        public ScoreTemplateContainer(ScoreTemplate scoreTemplate,bool isOpen = true)
        {
            myTemplate = scoreTemplate;
            MyBlock = Mathf.RoundToInt(100/myTemplate.chance);
            this.isOpen = isOpen;
        }
    }
}