using System;
using System.Collections.Generic;
using SlotMachine;
using UnityEngine;

namespace Others
{
    [CreateAssetMenu(fileName = "Score Chance Data", menuName = "Data/Score Chance", order = 0)]
    public class ScoreChanceData : ScriptableObject
    {
        public List<ScoreTemplate> scoreList;
    }

    [Serializable]
    public class ScoreTemplate
    {
        public int scoreId;
        public List<ScoreCard.CardType> scoreOrder;
        [Range(0,100)]
        public float chance;
        public int Block { get; set; }
    }
}