using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class AttributeReader : MonoBehaviour
    {
        [SerializeField]
        private TextAsset configAsset;

        private void Awake()
        {
            var configs = JsonConvert.DeserializeObject<List<AttributeConfig>>(configAsset.text);
            AttributePrimer.Prime(configs, gameObject);
        }
    }
}
