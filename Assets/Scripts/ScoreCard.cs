using UnityEngine;
using UnityEngine.UI;

public class ScoreCard : MonoBehaviour
{
    public enum CardType
    {
        Jackpot,
        Wild,
        Seven,
        Bonus,
        A
    }

    [SerializeField] private CardType myCardType;
    [SerializeField] private SpriteRenderer mySpriteRenderer;
    [SerializeField] private Sprite myNormalSprite;
    [SerializeField] private Sprite myBlurSprite;

    public CardType GetCardType()
    {
        return myCardType;
    }
    
    public void ChangeCardSprite(bool isBlur = false)
    {
        mySpriteRenderer.sprite = isBlur ? myNormalSprite : myBlurSprite;
    }
    
}