namespace IgrisLib.MessageBox
{
    internal static class Util
    {
        internal static string TryAddKeyboardAccellerator(this string input)
        {
            if (input.Contains("_"))
            {
                return input;
            }
            return $"_{input}";
        }
    }
}
