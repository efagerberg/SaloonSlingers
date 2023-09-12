using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SaloonSlingers.Core
{
    public struct HandEvaluation: IEquatable<HandEvaluation>
    {
        public HandEvaluation(HandNames name, uint score, IEnumerable<int> keyIndexes = null)
        {
            Name = name;
            Score = score;
            if (keyIndexes != null) KeyIndexes = keyIndexes.ToList();
            else KeyIndexes = new List<int>();
        }
        public HandNames Name { get; private set; }
        public uint Score { get; private set; }
        public readonly IList<int> KeyIndexes;

        public readonly string DisplayName()
        {
            if (Name == HandNames.NONE) return "";

            string enumName = Enum.GetName(typeof(HandNames), Name);
            TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(enumName.Replace("_", " ").ToLower());
        }

        public readonly bool Equals(HandEvaluation other)
        {
            return Name == other.Name && Score == other.Score && KeyIndexes.SequenceEqual(other.KeyIndexes);
        }
    }
}
