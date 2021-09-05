using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlotMachine
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private List<ScoreCardCoinData> scoreCardCoinDataList;
        [SerializeField] private ParticleSystem coinParticle;

        public void SpawnCoins(ScoreCard.CardType targetType)
        {
            if (scoreCardCoinDataList != null)
            {
                var count = scoreCardCoinDataList.FirstOrDefault(x => x.cardType == targetType).coinCount;
                var tempEmission =coinParticle.emission;
                tempEmission.enabled = true;
                tempEmission.rateOverTimeMultiplier = count;
            }

            coinParticle.Play();
        }
        
    }
    
    [Serializable]
    public class ScoreCardCoinData
    {
        public ScoreCard.CardType cardType;
        public int coinCount;
    }
}