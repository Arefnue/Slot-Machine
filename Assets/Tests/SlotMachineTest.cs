using System.Collections.Generic;
using System.Linq;
using Extensions;
using NUnit.Framework;
using SlotMachine;

namespace Tests
{
    public class SlotMachineTest
    {
        #region Tests

        [SetUp]
        public void InitTestSetup()
        {
            _scoreList = GetExampleScoreList();
        }
        
        [Category("Score Test")]
        [TestCase(ScoreCard.CardType.A,ScoreCard.CardType.Wild,ScoreCard.CardType.Bonus)]
        [TestCase(ScoreCard.CardType.Wild,ScoreCard.CardType.Wild,ScoreCard.CardType.Seven)]
        [TestCase(ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot,ScoreCard.CardType.A)]
        [TestCase(ScoreCard.CardType.Wild,ScoreCard.CardType.Bonus,ScoreCard.CardType.A)]
        [TestCase(ScoreCard.CardType.Bonus,ScoreCard.CardType.A,ScoreCard.CardType.Jackpot)]
        [TestCase(ScoreCard.CardType.A,ScoreCard.CardType.A,ScoreCard.CardType.A)]
        [TestCase(ScoreCard.CardType.Bonus,ScoreCard.CardType.Bonus,ScoreCard.CardType.Bonus)]
        [TestCase(ScoreCard.CardType.Seven,ScoreCard.CardType.Seven,ScoreCard.CardType.Seven)]
        [TestCase(ScoreCard.CardType.Wild,ScoreCard.CardType.Wild,ScoreCard.CardType.Wild)]
        [TestCase(ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot)]
        public void Score_Test(ScoreCard.CardType card1, ScoreCard.CardType card2, ScoreCard.CardType card3)
        {
            TestTargetChance(GetScoreOrder(card1, card2, card3));
        }
        
        [Category("Repeat Test")]
        [TestCase(ScoreCard.CardType.A,ScoreCard.CardType.Wild,ScoreCard.CardType.Bonus)]
        [TestCase(ScoreCard.CardType.Wild,ScoreCard.CardType.Wild,ScoreCard.CardType.Seven)]
        [TestCase(ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot,ScoreCard.CardType.A)]
        [TestCase(ScoreCard.CardType.Wild,ScoreCard.CardType.Bonus,ScoreCard.CardType.A)]
        [TestCase(ScoreCard.CardType.Bonus,ScoreCard.CardType.A,ScoreCard.CardType.Jackpot)]
        [TestCase(ScoreCard.CardType.A,ScoreCard.CardType.A,ScoreCard.CardType.A)]
        [TestCase(ScoreCard.CardType.Bonus,ScoreCard.CardType.Bonus,ScoreCard.CardType.Bonus)]
        [TestCase(ScoreCard.CardType.Seven,ScoreCard.CardType.Seven,ScoreCard.CardType.Seven)]
        [TestCase(ScoreCard.CardType.Wild,ScoreCard.CardType.Wild,ScoreCard.CardType.Wild)]
        [TestCase(ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot,ScoreCard.CardType.Jackpot)]
        public void Repeat_Test(ScoreCard.CardType card1, ScoreCard.CardType card2, ScoreCard.CardType card3)
        {
           TestRepeat(GetScoreOrder(card1,card2,card3),10,3);
        }
        
        #endregion

        #region Methods
        
        private void TestRepeat(List<ScoreCard.CardType> targetOrder,int turnCount,int threshold)
        {
            var lastElement = 0;
            var elementQueue = new Queue<int>();
            for (var i = 0; i < turnCount; i++)
            {
                var scoreTemplateTupleList = TestTargetChance(targetOrder);
                elementQueue.Enqueue(scoreTemplateTupleList[0].Item1);
            }

            var sameNumberCount = 0;
            for (var i = 0; i < turnCount; i++)
            {
                var element = elementQueue.Dequeue();
                if (i != 0)
                    if (element == lastElement)
                        sameNumberCount++;
                lastElement = element;
            }
        
            if (sameNumberCount >= threshold) Assert.Fail();
        }

        private List<(int, ScoreTemplateContainer)> TestTargetChance(List<ScoreCard.CardType> targetOrder)
        {
            var scoreTemplateContainerList = new List<ScoreTemplateContainer>();
            
            _scoreList.ForEach(x => scoreTemplateContainerList.Add(new ScoreTemplateContainer(x)));
            
            var selectedTemplateContainer = scoreTemplateContainerList.FirstOrDefault(x => IsSameOrder(targetOrder, x.myTemplate.scoreOrder));
            
            var scoreTemplateTupleList = new List<(int, ScoreTemplateContainer)>();
            
            for (var i = 0; i < 100; i++)
                if (SlotMachineLogic.GetMostPossibleScoreTemplate(i, scoreTemplateContainerList) ==
                    selectedTemplateContainer?.myTemplate)
                    scoreTemplateTupleList.Add((i, selectedTemplateContainer));
            
            var canPass = false;
            for (var i = 0; i < scoreTemplateTupleList.Count; i++)
                if (MathExtensions.IsBetweenRange(scoreTemplateTupleList[i].Item1,
                    i * scoreTemplateTupleList[i].Item2.MyBlock, (i + 1) * scoreTemplateTupleList[i].Item2.MyBlock))
                    canPass = true;
            
            Assert.IsTrue(canPass);
            
            return scoreTemplateTupleList;
        }

        private List<ScoreCard.CardType> GetScoreOrder(ScoreCard.CardType t1, ScoreCard.CardType t2, ScoreCard.CardType t3)
        {
            var newList = new List<ScoreCard.CardType>();
            newList.Add(t1);
            newList.Add(t2);
            newList.Add(t3);
            return newList;
        }

        private bool IsSameOrder(List<ScoreCard.CardType> p1, List<ScoreCard.CardType> p2)
        {
            for (var i = 0; i < p1.Count; i++)
                if (p1[i] != p2[i])
                    return false;
            return true;
        }

        private List<ScoreTemplate> _scoreList = new List<ScoreTemplate>();

        private List<ScoreTemplate> GetExampleScoreList()
        {
            var scoreList = new List<ScoreTemplate>();
            
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.A, ScoreCard.CardType.Wild, ScoreCard.CardType.Bonus), 13));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Wild, ScoreCard.CardType.Seven), 13));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Jackpot, ScoreCard.CardType.Jackpot, ScoreCard.CardType.A), 13));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Bonus, ScoreCard.CardType.A), 13));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Bonus, ScoreCard.CardType.A, ScoreCard.CardType.Jackpot), 13));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.A, ScoreCard.CardType.A, ScoreCard.CardType.A), 9));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Bonus, ScoreCard.CardType.Bonus, ScoreCard.CardType.Bonus), 8));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Seven, ScoreCard.CardType.Seven, ScoreCard.CardType.Seven), 7));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Wild, ScoreCard.CardType.Wild, ScoreCard.CardType.Wild), 6));
            scoreList.Add(new ScoreTemplate(GetScoreOrder(ScoreCard.CardType.Jackpot, ScoreCard.CardType.Jackpot, ScoreCard.CardType.Jackpot), 5));
            
            return scoreList;
        }

        #endregion
    }
}