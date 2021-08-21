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
   

   

    public void StartSpinning()
    {
        if (!_canSpin)
        {
            _canSpin = true;
            StartCoroutine(SlotTurnRoutine());
        }
    }

    public void StopSpinning()
    {
        _canSpin = false;
    }

    private IEnumerator SlotTurnRoutine()
    {
        var waitFrame = new WaitForEndOfFrame();
        var timer = 0f;
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(true));
        
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
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(false));
        
        
    }
    
    
    
}