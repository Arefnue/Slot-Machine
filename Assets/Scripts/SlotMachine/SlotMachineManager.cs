using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SlotMachine
{
    [Serializable]
    public class ScoreContainer
    {
        public int scoreId;
        public ScoreTemplate scoreTemplate;
        public ScoreContainer(int scoreId,ScoreTemplate scoreTemplate)
        {
            this.scoreId = scoreId;
            this.scoreTemplate = scoreTemplate;
        }
    }
    public class SlotMachineManager : MonoBehaviour
    {
        [Header("Machine Slots")]
        [SerializeField] private List<SlotController> slotControllerList;
        
        [Header("Settings")]
        [SerializeField] private float minChooseTime = 2f;
        [SerializeField] private float maxChooseTime = 5f;
        [SerializeField] private float slotMinDelayTime = 0.1f;
        [SerializeField] private float slotMaxDelayTime = 0.5f;
        [SerializeField] private bool isSlowStop;
        
        [Header("References")]
        [SerializeField] private Button playButton;
        [SerializeField] private CoinSpawner coinSpawner;
        [SerializeField] private ScoreChanceData scoreChanceData;
        
        [Header("Override")]
        [SerializeField] private bool enableOverrideScore;
        [SerializeField] private ScoreTemplate overrideScore;
        
        private bool _canPlaySlotMachine = true;
        private float _totalChance = 0;
        private int _currentSpinCount=0;
        private ScoreTemplate _selectedScoreTemplate;
        private bool _isWin = false;
        private List<ScoreContainer> _scoreTemplateList = new List<ScoreContainer>();


        #region Setup

        private void Awake()
        {
            Load();
        }

        private void Start()
        {
            var chanceMod = _currentSpinCount % _totalChance;

            if (chanceMod == 0)
            {
                CalculateScoreChances();
            }
        }


        #endregion

        #region Test

        private void Update()
        {
#if UNITY_EDITOR
            
            if (Input.GetKeyDown(KeyCode.T)) 
                for (int i = 0; i < 100; i++)
                    DetermineScore();

            if (Input.GetKeyDown(KeyCode.S)) Save();
            
            if (Input.GetKeyDown(KeyCode.L)) Load();
           
#endif
        }

        #endregion
        
        //todo Daha iyi bir algoritma bul
        private void CalculateScoreChances()
        {
            _scoreTemplateList?.Clear();
            
            //Toplam ihtimal kadar score üret
            int index = 0;
            foreach (var scoreTemplate in scoreChanceData.scoreList)
            {
                scoreTemplate.Block = Mathf.CeilToInt(_totalChance / scoreTemplate.chance);
                
                for (int i = 0; i < scoreTemplate.chance; i++, index++) 
                    _scoreTemplateList.Add(new ScoreContainer(scoreTemplate.scoreId, scoreTemplate));
            }

            _scoreTemplateList.Shuffle();

            //Her block için kontrol et
            Dictionary<int, List<ScoreContainer>> blockDict = new Dictionary<int, List<ScoreContainer>>();

            foreach (var scoreTemplate in scoreChanceData.scoreList)
                blockDict.Add(scoreTemplate.scoreId, new List<ScoreContainer>());
            
            foreach (var scoreTemplate in scoreChanceData.scoreList)
            {
                for (int i = 0; i < scoreTemplate.chance; i++)
                {
                    for (int j = 0; j < scoreTemplate.Block; j++)
                    {
                        var blockIndex = (i * scoreTemplate.Block) + j;

                        if (blockIndex >= _totalChance)  continue;
                        
                        var card = _scoreTemplateList[blockIndex];
                        if (card.scoreTemplate.scoreId == scoreTemplate.scoreId)
                        {
                            blockDict[scoreTemplate.scoreId].Add(card);
                            break;
                        }
                    }
                }
            }
            
            //Score olmayan blocklara score ekle
            foreach (var scoreTemplate in scoreChanceData.scoreList)
            {
                for (var i = 0; i < blockDict[scoreTemplate.scoreId].Count; i++)
                {
                    var scoreContainer = blockDict[scoreTemplate.scoreId][i];
                    if (scoreContainer == null)
                    {
                        var newValue = 0;
                        do
                        {
                            newValue = Random.Range(i * scoreTemplate.Block, (i + 1) * scoreTemplate.Block);
                        } while (newValue >= _totalChance);

                        var a = _scoreTemplateList[newValue];
                        a.scoreTemplate = scoreTemplate;
                    }
                }
            }
        }
        
        #region SaveLoad

        private void Save()
        { 
            PlayerPrefs.SetInt("CurrentSpin",_currentSpinCount);

            for (var i = 0; i < _scoreTemplateList.Count; i++)
            {
                var scoreContainer = _scoreTemplateList[i];
                PlayerPrefs.SetInt($"Score_{i}",scoreContainer.scoreId);
            }
        }

        private void Load()
        {
            _totalChance = 0;
            scoreChanceData.scoreList.ForEach(x => _totalChance += x.chance);
            
            _currentSpinCount =PlayerPrefs.GetInt("CurrentSpin",0);
            _scoreTemplateList?.Clear();
            
            for (int i = 0; i < _totalChance; i++)
            {
                if (!PlayerPrefs.HasKey($"Score_{i}")) break;
               
                var scoreId =PlayerPrefs.GetInt($"Score_{i}");
                
                _scoreTemplateList.Add(new ScoreContainer(scoreId,scoreChanceData.scoreList.FirstOrDefault(x=>x.scoreId == scoreId)));
            }
            
        }

        #endregion
        
        #region Routines

        private IEnumerator StartSlotsRoutine()
        {
            SetPlayButton(false);

            DetermineScore();
            
            foreach (var slotController in slotControllerList)
            {
                slotController.StartSpinning();
                yield return new WaitForSeconds(Random.Range(slotMinDelayTime, slotMaxDelayTime));
            }

            yield return new WaitForSeconds(Random.Range(minChooseTime, maxChooseTime));

            StartCoroutine(StopSlotsRoutine());
        }

        private IEnumerator StopSlotsRoutine()
        {
            var waitFinishTime = 0f;
            for (var i = 0; i < slotControllerList.Count; i++)
            {
                yield return new WaitForSeconds(Random.Range(slotMinDelayTime, slotMaxDelayTime));

                var slotController = slotControllerList[i];
                
                if (i == slotControllerList.Count - 1)
                    slotController.StopSpinning(_selectedScoreTemplate.scoreOrder[i],
                        isSlowStop ? SlotController.StopType.Slow : SlotController.StopType.Normal);
                else
                    slotController.StopSpinning(_selectedScoreTemplate.scoreOrder[i]);

                waitFinishTime += slotController.FinishDelayTime;
            
            }
        
            yield return new WaitForSeconds(waitFinishTime);

            slotControllerList.ForEach(slotController => _isWin = slotController.SelectedScoreCard.MyCardType == slotControllerList[0].SelectedScoreCard.MyCardType);
       
            if (_isWin)
            {
                coinSpawner.SpawnCoins(_selectedScoreTemplate.scoreOrder[0]);
            }
            
            Save();
            
            SetPlayButton(true);
        }

        #endregion

        #region Methods
        
        private void DetermineScore()
        {
            _selectedScoreTemplate = enableOverrideScore ? overrideScore : _scoreTemplateList[_currentSpinCount].scoreTemplate;
            
            var str =
                $"Spin: {_currentSpinCount} --- 1. {_selectedScoreTemplate.scoreOrder[0].ToString()} 2. {_selectedScoreTemplate.scoreOrder[1].ToString()} 3. {_selectedScoreTemplate.scoreOrder[2].ToString()}";
            Debug.Log(str);
            
            _currentSpinCount++;
        }
    
        public void OnPlaySlotMachine()
        {
            if (_canPlaySlotMachine)
            {
                _isWin = false;
                StartCoroutine(StartSlotsRoutine());
            }
        }

        private void SetPlayButton(bool canPlay)
        {
            _canPlaySlotMachine = canPlay;
            playButton.interactable = canPlay;
        }

        #endregion
    }
    
   
}