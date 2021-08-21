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
    
    [Header("Score Cards")]
    [SerializeField] private List<ScoreCard> scoreCardList;
    
    [Header("Settings")]
    [SerializeField] private float spinSpeed = 5f;
    [SerializeField] private float bottomThreshold = -7.5f;
    [SerializeField] private float topThreshold = 5f;
    [SerializeField] private float gapBetweenCards = 2.5f;
    [SerializeField] private AnimationCurve stopSpinSpeedCurve;
    
    public ScoreCard SelectedScoreCard { get; private set; }
    
    private bool _canSpin=false;
    private bool _isSpinning = false;
    private StopType _myStopType = StopType.Instant;
    private ScoreCard.CardType _targetCardType = ScoreCard.CardType.A;
  
    
    #region MainRoutine

    private IEnumerator SlotTurnRoutine()
    {
        var waitFrame = new WaitForEndOfFrame();
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(true));

        StartCoroutine(SpinRoutine(waitFrame));

        yield return new WaitWhile(() => _canSpin);
        
        scoreCardList.ForEach(x=>x.ChangeCardSprite(false));

        SelectedScoreCard = scoreCardList.FirstOrDefault(x => x.MyCardType == _targetCardType);

        yield return _myStopType switch
        {
            StopType.Instant => StartCoroutine(StopSpinningRoutine()),
            StopType.Normal => StartCoroutine(StopSpinningRoutine(waitFrame, 1f)),
            StopType.Slow => StartCoroutine(StopSpinningRoutine(waitFrame, 2.25f)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    #endregion
    
    #region SpinRoutines
    private IEnumerator StopSpinningRoutine()
    {
        yield return new WaitUntil(() => SelectedScoreCard.LocalPosition.y.IsBetweenRange(0, gapBetweenCards));
        
        yield return new WaitUntil(() => SelectedScoreCard.LocalPosition.y <= 0.001f);
        SnapCards();
        _isSpinning = false;
        
    }
    private IEnumerator StopSpinningRoutine(WaitForEndOfFrame waitFrame,float slowDuration)
    {
        var timer = 0f;
        var oldSpinSpeed = spinSpeed;
        while (true)
        {
            timer += Time.deltaTime;

            spinSpeed = oldSpinSpeed*stopSpinSpeedCurve.Evaluate(timer/slowDuration);
            
            if (timer>=slowDuration)
            {
                break;
            }

            yield return waitFrame;
        }
        
        yield return new WaitUntil(() => SelectedScoreCard.LocalPosition.y.IsBetweenRange(0, gapBetweenCards));
        
        yield return new WaitUntil(() => SelectedScoreCard.LocalPosition.y <= 0.001f);
       
        SnapCards();
        _isSpinning = false;
        spinSpeed = oldSpinSpeed;
        
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

    #endregion
    
    #region Methods
    public void StartSpinning()
    {
        if (_canSpin || _isSpinning) return;
        
        _isSpinning = true;
        _canSpin = true;
        StartCoroutine(SlotTurnRoutine());
    }
    public void StopSpinning(ScoreCard.CardType cardType,StopType stopType = StopType.Instant)
    {
        _myStopType = stopType;
        _canSpin = false;
        _targetCardType = cardType;
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

    #endregion
}