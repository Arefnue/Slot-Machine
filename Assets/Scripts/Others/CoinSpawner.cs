using System;
using System.Collections.Generic;
using SlotMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Others
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private List<ScoreCardCoinData> scoreCardCoinDataList;
        [SerializeField] private Coin coinPrefab;
        [SerializeField] private Transform coinSpawnTransform;
        [SerializeField] private float minExplosionValue;
        [SerializeField] private float maxExplosionValue;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                SpawnCoins(10);
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                SpawnCoins(50);
            }
        }

        public void SpawnCoins(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var coin = Instantiate(coinPrefab, coinSpawnTransform);
                coin.ExplodeCoin(minExplosionValue,maxExplosionValue,coinSpawnTransform.position + new Vector3(Random.Range(-2,2),-1,1),5f,10);
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