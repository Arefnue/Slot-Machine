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
    public class ScoreContainer
    {
        public ScoreTemplate myCard;
        public bool isOpen;
        public int myBlock;
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

        private List<ScoreContainer> _scoreContainersList = new List<ScoreContainer>();
      
        
        private readonly bool _isSlowStop = true;
        private bool _canPlaySlotMachine = true;
        private float totalChance = 0;
        private int _currentSpinCount=0;
        private ScoreTemplate _selectedScoreTemplate;

        private bool _isWin = false;

        private void Start()
        {
            totalChance = 0;
            scoreChanceData.scoreList.ForEach(x=>totalChance+=x.chance);
            
            foreach (var scoreTemplate in scoreChanceData.scoreList)
            {
                var newContainer = new ScoreContainer();
                newContainer.myCard = scoreTemplate;
                newContainer.myBlock = Mathf.RoundToInt(totalChance / scoreTemplate.chance);
                newContainer.isOpen = true;
                _scoreContainersList.Add(newContainer);
            }
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                for (int i = 0; i < 100; i++)
                {
                    DetermineScore();
                }
            }
        }

        public void DetermineScore()
        {
            foreach (var scoreContainer in _scoreContainersList)
            {
                var mod = _currentSpinCount % scoreContainer.myBlock;
                if (mod == 0)
                    scoreContainer.isOpen = true;
            }

            _selectedScoreTemplate = _scoreContainersList.Find(x => x.isOpen).myCard;

            if (_selectedScoreTemplate == null)
            {
                Debug.Log("AAA");
            }

            _scoreContainersList.FirstOrDefault(x => x.myCard == _selectedScoreTemplate).isOpen = false;
            
            var str =
                $"Spin: {_currentSpinCount} --- 1. {_selectedScoreTemplate.scoreOrder[0].ToString()} 2. {_selectedScoreTemplate.scoreOrder[1].ToString()} 3. {_selectedScoreTemplate.scoreOrder[2].ToString()}";
            Debug.Log(str);
            _currentSpinCount++;
        }
    
        #region Routines

        private IEnumerator StartSlotsRoutine()
        {
            SetPlayButton(false);

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
                    slotController.StopSpinning(ScoreCard.CardType.A,
                        _isSlowStop ? SlotController.StopType.Slow : SlotController.StopType.Normal);
                else
                    slotController.StopSpinning(ScoreCard.CardType.A);

                waitFinishTime += slotController.FinishDelayTime;
            
            }
        
            yield return new WaitForSeconds(waitFinishTime);

            slotControllerList.ForEach(slotController => _isWin = slotController.SelectedScoreCard.MyCardType == slotControllerList[0].SelectedScoreCard.MyCardType);
       
            if (_isWin)
            {
                Debug.Log("Win");
            }
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