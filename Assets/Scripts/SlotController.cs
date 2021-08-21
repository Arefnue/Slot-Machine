using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{ 
    public List<ScoreCard> scoreCardList;

    [SerializeField] private float spinSpeed = 5f;
    [SerializeField] private float bottomThreshold = -7.5f;
    [SerializeField] private float topThreshold = 5f;
    

    private bool _canSpin=false;
    private bool _isSpinning=false;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaySlot();
        }
#endif
    }

    public void PlaySlot()
    {
        if (!_isSpinning)
        {
            StartCoroutine(SlotTurnRoutine());
        }
       
    }

    private IEnumerator SlotTurnRoutine()
    {
        var waitFrame = new WaitForEndOfFrame();
        var timer = 0f;

        _isSpinning = true;
        _canSpin = true;
        
        while (_canSpin)
        {
            timer += Time.deltaTime;

            foreach (var scoreCard in scoreCardList)
            {
                var cardLocalPos =scoreCard.transform.localPosition;
                cardLocalPos.y -= Time.deltaTime * spinSpeed;

                if (cardLocalPos.y<=bottomThreshold)
                {
                    cardLocalPos.y = topThreshold;
                }
                
                scoreCard.transform.localPosition = cardLocalPos;
                
            }
            
            yield return waitFrame;
        }
        
        
    }
    
    
    
}