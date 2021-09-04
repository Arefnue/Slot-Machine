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
        [SerializeField] private ScoreCard.CardType overrideScoreHitType;
        
        private bool _canPlaySlotMachine = true;
        private float _totalChance;
        private int _currentSpinCount;
        private ScoreTemplate _selectedScoreTemplate;
        private bool _isWin;

        private List<ScoreTemplateContainer> _scoreTemplateContainerList = new List<ScoreTemplateContainer>();
        
        #region Setup

        private void Start()
        {
           InitScoreTemplateContainers(scoreChanceData.scoreList);
        }
        
        public void InitScoreTemplateContainers(List<ScoreTemplate> scoreList)
        {
            scoreList.ForEach(x => _scoreTemplateContainerList.Add(new ScoreTemplateContainer(x)));
        }

        
        private void OnEnable()
        {
            foreach (var slotController in slotControllerList)
                slotController.OnFinalSpinEnd += OnFinalSpinEnd;
        }
        
        private void OnDisable()
        {
            foreach (var slotController in slotControllerList)
                slotController.OnFinalSpinEnd -= OnFinalSpinEnd;
        }

        #endregion

        #region Test
        private void Update()
        {
#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.T))
            {
               
                for (int i = 0; i < 100; i++)
                    DetermineScore();
            }
                
#endif
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
            for (var i = 0; i < slotControllerList.Count; i++)
            {
                yield return new WaitForSeconds(Random.Range(slotMinDelayTime, slotMaxDelayTime));

                var slotController = slotControllerList[i];
                
                if (i == slotControllerList.Count - 1)
                    slotController.StopSpinning(_selectedScoreTemplate.scoreOrder[i],
                        isSlowStop ? SlotController.StopType.Slow : SlotController.StopType.Normal);
                else
                    slotController.StopSpinning(_selectedScoreTemplate.scoreOrder[i]);
            }

        }
        
        #endregion

        #region Methods
        
        private void OnFinalSpinEnd()
        {
            slotControllerList.ForEach(slotController =>
                _isWin = slotController.SelectedScoreCard.MyCardType == slotControllerList[0].SelectedScoreCard.MyCardType);

            if (_isWin) coinSpawner.SpawnCoins(_selectedScoreTemplate.scoreOrder[0]);

            SetPlayButton(true);
        }
        
        private void DetermineScore()
        {
            _selectedScoreTemplate =SlotMachineLogic.GetMostPossibleScoreTemplate(_currentSpinCount,_scoreTemplateContainerList);
            var str =
                $"Spin: {_currentSpinCount} --- 1. {_selectedScoreTemplate.scoreOrder[0].ToString()} 2. {_selectedScoreTemplate.scoreOrder[1].ToString()} 3. {_selectedScoreTemplate.scoreOrder[2].ToString()}";
            Debug.Log(str);
            
            _currentSpinCount++;
        }
        
        // private ScoreTemplate GetOverrideHitScoreTemplate(ScoreCard.CardType targetCardType)
        // {
        //     var newTemplate = new ScoreTemplate();
        //     
        //     newTemplate.chance = 100;
        //     
        //     var newCardList = new List<ScoreCard.CardType>();
        //     for (int i = 0; i < slotControllerList.Count; i++)
        //         newCardList.Add(targetCardType);
        //
        //     newTemplate.scoreOrder = newCardList;
        //
        //     return newTemplate;
        // }

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