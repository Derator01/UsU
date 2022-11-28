using System.Linq;

namespace UsU.Extended;

public static class Extensions
{
    private static readonly char[] _capitals = new char[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M' }

    public static string SplitByCapitals(this string self)
    {
        for (int i = 2; i < self.Length; i++)
            if (_capitals.Contains(self[i]) && !_capitals.Contains(self[i - 1]))
                self.Insert(i - 1, " ");
        return self;
    }
}