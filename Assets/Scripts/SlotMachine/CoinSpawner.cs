using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlotMachine
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private List<ScoreCardCoinData> scoreCardCoinDataList;
        [SerializeField] private Coin coinPrefab;
        [SerializeField] private Transform coinSpawnTransform;
        [SerializeField] private float minExplosionValue;
        [SerializeField] private float maxExplosionValue;
        
        public void SpawnCoins(ScoreCard.CardType targetType)
        {
            var count = scoreCardCoinDataList.FirstOrDefault(x => x.cardType == targetType).coinCount;
            for (int i = 0; i < count; i++)
            {
                var coin = Instantiate(coinPrefab, coinSpawnTransform);
                coin.ExplodeCoin(minExplosionValue,maxExplosionValue,
                    coinSpawnTransform.position + new Vector3(Random.Range(-2,2),-1,2),
                    5f,
                    10);
            }
        }
        
    }
    
    [Serializable]
    public class ScoreCardCoinData
    {
        public ScoreCard.CardType cardType;
        public int coinCount;
    }
}