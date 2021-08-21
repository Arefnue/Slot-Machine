using System;
using System.Collections.Generic;
using SlotMachine;
using UnityEngine;

namespace Utility
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private List<ScoreCardCoinData> scoreCardCoinDataList;
        
    }
    
    [Serializable]
    public class ScoreCardCoinData
    {
        public ScoreCard.CardType cardType;
        public int coinCount;
    }
}