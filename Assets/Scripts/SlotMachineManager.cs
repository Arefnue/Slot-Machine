using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineManager : MonoBehaviour
{
    public List<SlotController> slotControllerList;

    [SerializeField] private float slotMinDelayTime = 0.1f;
    [SerializeField] private float slotMaxDelayTime = 0.5f;
    
    
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
        foreach (var slotController in slotControllerList)
        {
            yield return new WaitForSeconds(Random.Range(slotMinDelayTime,slotMaxDelayTime));
            slotController.StopSpinning();
        }
    }
    
    
    
    
}