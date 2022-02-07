using System;
using System.Globalization;
using System.Threading;

namespace SaloonSlingers.Core
{
    public struct HandType
    {
        public HandType(HandNames name, uint score)
        {
            Name = name;
            Score = score;
        }

        public HandNames Name { get; private set; }
        public uint Score { get; private set; }

        public string DisplayName()
        {
            if (Name == HandNames.EMPTY) return "";

            string enumName = Enum.GetName(typeof(HandNames), Name);
            TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(enumName.Replace("_", " ").ToLower());
        }
    }
}
