using System.Collections.Generic;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class SpeakerSync : MonoBehaviour
    {
        [SerializeField]
        private List<AudioSource> toPlay;

        private void Start()
        {
            foreach (var x in toPlay) x.Play();
        }
    }
}
