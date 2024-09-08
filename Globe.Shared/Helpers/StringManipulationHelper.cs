namespace Globe.Shared.Helpers
{
    public static class StringManipulationHelper
    {
        public static string ConvertToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

    }
}
