using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class PlayerAttributes : MonoBehaviour
    {
        public Deck Deck { get; private set; } = new Deck(3);
    }
}
