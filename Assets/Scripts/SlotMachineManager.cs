using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private readonly bool _isSlowStop = true;
    private bool _canPlaySlotMachine = true;

    private bool _isWin = false;
    
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