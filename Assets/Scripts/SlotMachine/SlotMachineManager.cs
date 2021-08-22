using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Others;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SlotMachine
{
    [Serializable]
    public class ScoreContainer
    {
        public int order;
        public ScoreTemplate scoreTemplate;
        public ScoreContainer(int order,ScoreTemplate scoreTemplate)
        {
            this.order = order;
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
    
        [Header("References")]
        [SerializeField] private Button playButton;
        [SerializeField] private CoinSpawner coinSpawner;
        [SerializeField] private ScoreChanceData scoreChanceData;

        private readonly bool _isSlowStop = true;
        private bool _canPlaySlotMachine = true;
        private float _totalChance = 0;
        private int _currentSpinCount=0;
        private ScoreTemplate _selectedScoreTemplate;

        private bool _isWin = false;

        private List<ScoreContainer> _scoreTemplateList = new List<ScoreContainer>();
        private void Start()
        {
            _totalChance = 0;
            scoreChanceData.scoreList.ForEach(x=>_totalChance+=x.chance);
            
            _currentSpinCount = PlayerPrefs.GetInt("CurrentSpin");
            
            int index = 0;
            foreach (var scoreTemplate in scoreChanceData.scoreList)
            {
                scoreTemplate.Block = Mathf.RoundToInt(_totalChance / scoreTemplate.chance);
                for (int i = 0; i < scoreTemplate.chance; i++)
                {
                    _scoreTemplateList.Add(new ScoreContainer(index,scoreTemplate));
                    index++;
                }
            }

        }
        public void DetermineScore()
        {
            
            _selectedScoreTemplate = _scoreTemplateList.FirstOrDefault(x=>x.order == _currentSpinCount).scoreTemplate;
            
            var str =
                $"Spin: {_currentSpinCount} --- 1. {_selectedScoreTemplate.scoreOrder[0].ToString()} 2. {_selectedScoreTemplate.scoreOrder[1].ToString()} 3. {_selectedScoreTemplate.scoreOrder[2].ToString()}";
            Debug.Log(str);
            _currentSpinCount++;
        }


        public void Save()
        { 
            PlayerPrefs.SetInt("CurrentSpin",_currentSpinCount);
        }
        
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
                        _isSlowStop ? SlotController.StopType.Slow : SlotController.StopType.Normal);
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