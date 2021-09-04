using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using SlotMachine;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SlotMachineTest
    {

        private List<ScoreCard.CardType> GetScoreOrder(ScoreCard.CardType t1,ScoreCard.CardType t2, ScoreCard.CardType t3)
        {
            var newList = new List<ScoreCard.CardType>();
            newList.Add(t1);
            newList.Add(t2);
            newList.Add(t3);
            return newList;
        }

        private void TestTargetChance(float chance)
        {
            List<ScoreTemplateContainer> scoreTemplateContainerList = new List<ScoreTemplateContainer>();

            List<ScoreTemplate> scoreList = new List<ScoreTemplate>();
            
            var order1 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.A, ScoreCard.CardType.Wild, ScoreCard.CardType.Bonus), 13);
            scoreList.Add(order1);
            var order2 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Wild, ScoreCard.CardType.Wild), 13);
            scoreList.Add(order2);
            var order3 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Jackpot, ScoreCard.CardType.Jackpot, ScoreCard.CardType.A), 13);
            scoreList.Add(order3);
            var order4 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Bonus, ScoreCard.CardType.A), 13);
            scoreList.Add(order4);
            var order5 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Bonus, ScoreCard.CardType.A, ScoreCard.CardType.Jackpot), 13);
            scoreList.Add(order5);
            var order6 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.A, ScoreCard.CardType.A, ScoreCard.CardType.A), 9);
            scoreList.Add(order6);
            var order7 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Bonus, ScoreCard.CardType.Bonus, ScoreCard.CardType.Bonus), 8);
            scoreList.Add(order7);
            var order8 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Seven, ScoreCard.CardType.Seven, ScoreCard.CardType.Seven), 7);
            scoreList.Add(order8);
            var order9 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Wild, ScoreCard.CardType.Wild), 6);
            scoreList.Add(order9);
            var order10 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Jackpot, ScoreCard.CardType.Jackpot, ScoreCard.CardType.Jackpot), 5);
            scoreList.Add(order10);
            
            
            foreach (var scoreTemplate in scoreList)
            {
                scoreTemplateContainerList.Add(new ScoreTemplateContainer(scoreTemplate));
            }
            
            ScoreTemplateContainer scoreTemplateContainer = scoreTemplateContainerList.FirstOrDefault(x => x.myTemplate.chance == chance);

            List<(int, ScoreTemplateContainer)> scoreTemplateTupleList = new List<(int, ScoreTemplateContainer)>();
            for (int i = 0; i < 100; i++)
            {
                var selected = SlotMachineLogic.GetMostPossibleScoreTemplate(i, scoreTemplateContainerList);
                if (selected == scoreTemplateContainer?.myTemplate)
                {
                    scoreTemplateTupleList.Add((i,scoreTemplateContainer));
                }
            }

            bool canPass = true;
            for (int i = 0; i < scoreTemplateTupleList.Count; i++)
            {
                if (!MathExtensions.IsBetweenRange(scoreTemplateTupleList[i].Item1,i*scoreTemplateTupleList[i].Item2.MyBlock,(i+1)*scoreTemplateTupleList[i].Item2.MyBlock))
                {
                    canPass = false;
                }
            }
            
            Assert.IsTrue(canPass);
            
        }
        
        // // A Test behaves as an ordinary method
        [Test]
        public void slot_machine_chance_5()
        {
            TestTargetChance(5);
        } 
        
        [Test]
        public void slot_machine_chance_13()
        {
            TestTargetChance(13);
        }
        
    }
}
