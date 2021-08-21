using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineManager : MonoBehaviour
{
    public List<SlotController> slotControllerList;

    [SerializeField] private float slotMinDelayTime = 0.1f;
    [SerializeField] private float slotMaxDelayTime = 0.5f;

    private bool _canWin = false;
    
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(StartSlotsRoutine());
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(StopSlotsRoutine());
        }
#endif
    }


    private IEnumerator StartSlotsRoutine()
    {
        foreach (var slotController in slotControllerList)
        {
            slotController.StartSpinning();
            yield return new WaitForSeconds(Random.Range(slotMinDelayTime,slotMaxDelayTime));
        }
    } 
    private IEnumerator StopSlotsRoutine()
    {
        for (var i = 0; i < slotControllerList.Count; i++)
        {
           
            yield return new WaitForSeconds(Random.Range(slotMinDelayTime, slotMaxDelayTime));
            var slotController = slotControllerList[i];

            // if (i==(slotControllerList.Count-1))
            // {
            //     slotController.StopSpinning(_canWin ? SlotController.StopType.Slow : SlotController.StopType.Normal);
            // }
            // else
            // {
            //     slotController.StopSpinning();
            // }
            slotController.StopSpinning(ScoreCard.CardType.A);
        }
    }
    
    
    
    
}