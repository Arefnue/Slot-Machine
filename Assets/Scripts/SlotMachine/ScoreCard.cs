using UnityEngine;

namespace SlotMachine
{
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

        public CardType MyCardType => myCardType;
        [SerializeField] private CardType myCardType;
        [SerializeField] private SpriteRenderer mySpriteRenderer;
        [SerializeField] private Sprite myNormalSprite;
        [SerializeField] private Sprite myBlurSprite;
    
        public Vector3 LocalPosition
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
    
        public void ChangeCardSprite(bool isBlur)
        {
            mySpriteRenderer.sprite = isBlur ?  myBlurSprite : myNormalSprite;
        }

    }
}