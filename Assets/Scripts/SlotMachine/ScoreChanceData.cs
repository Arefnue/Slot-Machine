using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMachine
{
    [CreateAssetMenu(fileName = "Score Chance Data", menuName = "Data/Score Chance", order = 0)]
    public class ScoreChanceData : ScriptableObject
    {
        public List<ScoreTemplate> scoreList;
    }

    [Serializable]
    public class ScoreTemplate
    {
        public List<ScoreCard.CardType> scoreOrder;
        [Range(0,100)]
        public float chance;

        public ScoreTemplate(List<ScoreCard.CardType> scoreOrder, float chance)
        {
            this.scoreOrder = scoreOrder;
            this.chance = chance;
        }
    }
}