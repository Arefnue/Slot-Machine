using UnityEngine;
using UnityEngine.U2D;

namespace SlotMachine
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AtlasObject : MonoBehaviour
    {
        private SpriteAtlas _mainAtlas;
        
        public void ConvertSpritesToAtlas(SpriteAtlas spriteAtlas)
        {
            _mainAtlas = spriteAtlas;
            var rend =GetComponent<SpriteRenderer>();
            var mask = GetComponent<SpriteMask>();
            if (mask)
            {
                var maskName = mask.sprite.name;
                mask.sprite = _mainAtlas.GetSprite(maskName);
            }
            var spriteName = rend.sprite.name;
            rend.sprite = _mainAtlas.GetSprite(spriteName);
        }
       
    }
}
