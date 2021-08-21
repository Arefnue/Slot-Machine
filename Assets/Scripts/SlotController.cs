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
    
    public ScoreCard SelectedScoreCard { get; set; }
    
    private bool _canSpin=false;
    private bool _isSpinning = false;
    private StopType _myStopType = StopType.Instant;
    private ScoreCard.CardType _targetCardType = ScoreCard.CardType.A;
    
    public void StartSpinning()
    {
        if (!_canSpin && !_isSpinning)
        {
            _isSpinning = true;
            _canSpin = true;
            StartCoroutine(SlotTurnRoutine());
        }
    }

    public void StopSpinning(ScoreCard.CardType cardType,StopType stopType = StopType.Instant)
    {
        _myStopType = stopType;
        _canSpin = false;
        _targetCardType = cardType;
    }
    
    private IEnumerator SlotTurnRoutine()
    {
        var waitFrame = new WaitForEndOfFrame();
        var timer = 0f;
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(true));

        StartCoroutine(SpinRoutine(waitFrame));

        yield return new WaitWhile(() => _canSpin);
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(false));

        SelectedScoreCard = scoreCardList.FirstOrDefault(x => x.MyCardType == _targetCardType);
        
        switch (_myStopType)
        {
            case StopType.Instant:

                yield return StartCoroutine(StopInstant());
                
                break;
            case StopType.Normal:
                
                break;
            case StopType.Slow:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private IEnumerator StopInstant()
    {
        yield return new WaitUntil(() => SelectedScoreCard.LocalPosition.y.IsBetweenRange(0, gapBetweenCards));
        
        yield return new WaitUntil(() => SelectedScoreCard.LocalPosition.y <= 0.1f);
        SnapCards();
        _isSpinning = false;
        
    }
    
    private void SnapCards()
    {
        foreach (var scoreCard in scoreCardList)
        {
            var cardLocal = scoreCard.LocalPosition;
            cardLocal.y = cardLocal.y.RoundTo(gapBetweenCards);
            scoreCard.LocalPosition = cardLocal;
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