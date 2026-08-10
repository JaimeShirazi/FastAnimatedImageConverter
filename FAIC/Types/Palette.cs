namespace FAIC.Types
{
    public static class Palette
    {
        public struct Set
        {
            public struct Color
            {
                public byte r;
                public byte g;
                public byte b;
                public static implicit operator System.Drawing.Color(Color c) => System.Drawing.Color.FromArgb(c.r, c.g, c.b);
                public static implicit operator System.Windows.Media.Color(Color c) => System.Windows.Media.Color.FromArgb(255, c.r, c.g, c.b);
            }
            public Color onBright;
            public Color onSelected;
            public Color onDark;
            public Set(Color onBright, Color onSelected, Color onDark)
            {
                this.onBright = onBright;
                this.onSelected = onSelected;
                this.onDark = onDark;
            }
            public static Color FromBytes(byte r, byte g, byte b) => new Color()
            {
                r = r,
                g = g,
                b = b
            };
        }
        //Trying to optimise for colour blindness
        private static readonly Set[] sets =
        {
            //Blue
            new(onBright:   Set.FromBytes(41, 96, 164),
                onSelected: Set.FromBytes(9, 34, 65),
                onDark:     Set.FromBytes(88, 156, 218)),
            //Orange
            new(onBright:   Set.FromBytes(131, 72, 3),
                onSelected: Set.FromBytes(34, 18, 0),
                onDark:     Set.FromBytes(218, 118, 44)),
            //Teal
            new(onBright:   Set.FromBytes(26, 120, 97),
                onSelected: Set.FromBytes(6, 53, 41),
                onDark:     Set.FromBytes(56, 190, 145)),
            //Gold
            new(onBright:   Set.FromBytes(128, 105, 3),
                onSelected: Set.FromBytes(57, 46, 0),
                onDark:     Set.FromBytes(232, 211, 70)),
            //Grey
            new(onBright:   Set.FromBytes(82, 82, 82),
                onSelected: Set.FromBytes(12, 12, 12),
                onDark:     Set.FromBytes(194, 199, 208)),
        };
        public static Set GetForIndex(int index) => sets[Math.Max(0, index % sets.Length)];
        public static Set.Color StaticA { get; } = Set.FromBytes(104, 41, 166);
        public static Set.Color StaticB { get; } = Set.FromBytes(128, 32, 56);
    }
}
