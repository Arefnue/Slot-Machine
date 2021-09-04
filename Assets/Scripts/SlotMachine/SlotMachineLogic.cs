using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace SlotMachine
{
    public static class SlotMachineLogic
    {
        private static ScoreTemplateContainer _selectedContainer;
        public static ScoreTemplate GetMostPossibleScoreTemplate(int currentSpinCount,List<ScoreTemplateContainer> scoreTemplateContainerList)
        {
            ScoreTemplate targetTemplate = null;
            
            _selectedContainer = scoreTemplateContainerList[0];
            float bestChance = 0;
            scoreTemplateContainerList.Shuffle();
            foreach (var scoreTemplateContainer in scoreTemplateContainerList)
            {
                var mod = currentSpinCount %  scoreTemplateContainer.MyBlock;

                if (mod == 0)
                {
                    scoreTemplateContainer.RepeatCount = 1;
                    scoreTemplateContainer.isOpen = true;
                }
                
                scoreTemplateContainer.RepeatCount++;
                
                var newChance = (float)1 / (scoreTemplateContainer.RepeatCount - mod);
                
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
        public int MyBlock => _myBlock;
        private int _myBlock;
        public bool isOpen;
        
        public int RepeatCount
        {
            get => _repeatCount;
            set => _repeatCount = value;
        }
        private int _repeatCount;
        public ScoreTemplateContainer(ScoreTemplate scoreTemplate,bool isOpen = true)
        {
            
            myTemplate = scoreTemplate;
            _myBlock = Mathf.RoundToInt(100/myTemplate.chance);
            this.isOpen = isOpen;
        }
    }
}