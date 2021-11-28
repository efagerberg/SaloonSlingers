using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class CardGraphicsHelper
    {
        public static void SetFaceTexture(Card card, Renderer faceRenderer)
        {
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        private static string GetTexturePath(Card card) => string.Format("Textures/{0}", card.ToString());
    }
}
