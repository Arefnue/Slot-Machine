using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    public enum StopType
    {
        Instant,
        Normal,
        Slow
    }
    
    public List<ScoreCard> scoreCardList;

    [SerializeField] private float spinSpeed = 5f;
    [SerializeField] private float bottomThreshold = -7.5f;
    [SerializeField] private float topThreshold = 5f;
    [SerializeField] private float gapBetweenCards = 2.5f;
    

    private bool _canSpin=false;
    private bool _isSpinning = false;
    private StopType _myStopType = StopType.Instant;
    
    public void StartSpinning()
    {
        if (!_canSpin && !_isSpinning)
        {
            _isSpinning = true;
            _canSpin = true;
            StartCoroutine(SlotTurnRoutine());
        }
    }

    public void StopSpinning(StopType stopType = StopType.Instant)
    {
        _myStopType = stopType;
        _canSpin = false;
    }
    
    public bool IsBetween(float targetValue, float bound1, float bound2)
    {
        return (targetValue >= Math.Min(bound1,bound2) && targetValue <= Math.Max(bound1,bound2));
    }

    private IEnumerator SlotTurnRoutine()
    {
        var waitFrame = new WaitForEndOfFrame();
        var timer = 0f;
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(true));

        StartCoroutine(SpinRoutine(waitFrame));

        yield return new WaitWhile(() => _canSpin);
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(false));

        switch (_myStopType)
        {
            case StopType.Instant:

                var centerCardCandidate = scoreCardList.FirstOrDefault(x=>x.LocalPosition.y.IsBetweenRange(0,gapBetweenCards));

                if (centerCardCandidate)
                {
                    yield return new WaitUntil(() => centerCardCandidate.LocalPosition.y <= 0.0001f);
                    Debug.Log(centerCardCandidate.MyCardType.ToString());
                    _isSpinning = false;
                }
                
                break;
            case StopType.Normal:
                
                break;
            case StopType.Slow:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
        
        
    }

    private IEnumerator SpinRoutine(WaitForEndOfFrame waitFrame)
    {
        while (_isSpinning)
        {
            foreach (var scoreCard in scoreCardList)
            {
                var cardLocalPos =scoreCard.LocalPosition;
                
                cardLocalPos.y -= Time.deltaTime * spinSpeed;
                
                if (cardLocalPos.y<=bottomThreshold) cardLocalPos.y = topThreshold;
                
                scoreCard.LocalPosition = cardLocalPos;
            }
            
            yield return waitFrame;
        }
    }
    
    
}