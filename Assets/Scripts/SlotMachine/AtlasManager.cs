using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SlotMachine
{
    public class AtlasManager : MonoBehaviour
    {
        public static AtlasManager instance;
        private AtlasManager(){}
        
        [SerializeField] private SpriteAtlas mainAtlas;
        public SpriteAtlas GetMainAtlas => mainAtlas;
        
        private void Awake()
        {
            instance = this;
            var atlasObjects = FindObjectsOfType<AtlasObject>();
            foreach (var atlasObject in atlasObjects)
                atlasObject.ConvertSpritesToAtlas(mainAtlas);
        }
    }
}