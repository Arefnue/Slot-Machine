using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using SlotMachine;
using UnityEngine;


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

        private bool IsSameOrder(List<ScoreCard.CardType> p1,List<ScoreCard.CardType> p2)
        {
            for (int i = 0; i < p1.Count; i++)
            {
                if (p1[i] != p2[i])
                {
                    return false;
                }
            }

            return true;

        }
        
        private List<(int,ScoreTemplateContainer)> TestTargetChance(List<ScoreCard.CardType> targetOrder)
        {
            List<ScoreTemplateContainer> scoreTemplateContainerList = new List<ScoreTemplateContainer>();
            
            List<ScoreTemplate> scoreList = new List<ScoreTemplate>();
            
            var order1 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.A, ScoreCard.CardType.Wild, ScoreCard.CardType.Bonus), 13);
            scoreList.Add(order1);
            var order2 = new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Wild, ScoreCard.CardType.Seven), 13);
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
           
            ScoreTemplateContainer scoreTemplateContainer = scoreTemplateContainerList.FirstOrDefault(x => IsSameOrder(targetOrder,x.myTemplate.scoreOrder));
            
            List<(int, ScoreTemplateContainer)> scoreTemplateTupleList = new List<(int, ScoreTemplateContainer)>();
            for (int i = 0; i < 100; i++)
            {
                var selected = SlotMachineLogic.GetMostPossibleScoreTemplate(i, scoreTemplateContainerList);
               
                if (selected == scoreTemplateContainer?.myTemplate)
                {
                   
                    scoreTemplateTupleList.Add((i,scoreTemplateContainer));
                }
            }

            bool canPass = false;
            for (int i = 0; i < scoreTemplateTupleList.Count; i++)
            {
                if (MathExtensions.IsBetweenRange(scoreTemplateTupleList[i].Item1,i*scoreTemplateTupleList[i].Item2.MyBlock,(i+1)*scoreTemplateTupleList[i].Item2.MyBlock))
                {
                    canPass = true;
                }
            }
            
            Assert.IsTrue(canPass);
           
            return scoreTemplateTupleList;
        }
        
        [Test]
        public void a_wild_bonus()
        {
           TestTargetChance(GetScoreOrder(ScoreCard.CardType.A,ScoreCard.CardType.Wild,ScoreCard.CardType.Bonus));
        }
        
        [Test]
        public void wild_wild_seven()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Wild,ScoreCard.CardType.Wild,ScoreCard.CardType.Seven));
        }
        
        [Test]
        public void jackpot_jackpot_a()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot,ScoreCard.CardType.A));
        }
        
        [Test]
        public void wild_bonus_a()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Wild,ScoreCard.CardType.Bonus,ScoreCard.CardType.A));
        }
        
        [Test]
        public void bonus_a_jackpot()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Bonus,ScoreCard.CardType.A,ScoreCard.CardType.Jackpot));
        }
        
        [Test]
        public void a_a_a()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.A,ScoreCard.CardType.A,ScoreCard.CardType.A));
        } 
        
        [Test]
        public void bonus_bonus_bonus()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Bonus,ScoreCard.CardType.Bonus,ScoreCard.CardType.Bonus));
        } 
        
        [Test]
        public void seven_seven_seven()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Seven,ScoreCard.CardType.Seven,ScoreCard.CardType.Seven));
        } 
        
        [Test]
        public void wild_wild_wild()
        {
            TestTargetChance(GetScoreOrder(ScoreCard.CardType.Wild,ScoreCard.CardType.Wild,ScoreCard.CardType.Wild));
        } 
        
        [Test]
        public void jackpot_jackpot_jackpot()
        {
            int lastElement=0;
            Queue<int> elementQueue = new Queue<int>();
            
            for (int i = 0; i < 10; i++)
            {
                var scoreTemplateTupleList = TestTargetChance(GetScoreOrder(ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot));
                elementQueue.Enqueue(scoreTemplateTupleList[0].Item1);
            }

            var sameNumberCount = 0;
            for (int i = 0; i < 10; i++)
            {
                var element = elementQueue.Dequeue();
                if (i!=0)
                {
                    if (element == lastElement)
                    {
                        sameNumberCount++;
                    }
                } 
                
                lastElement = element;
            }

            if (sameNumberCount>=3)
            {
                Assert.Fail();
            }
            
        }
        
        private void CheckRepeatedScores(List<(int,ScoreTemplateContainer)> tupleList)
        {
            var firstElement = tupleList[0];
            
        }
        
        
    }
}
