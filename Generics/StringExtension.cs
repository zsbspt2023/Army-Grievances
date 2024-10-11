namespace ArmyGrievances.Generics
{
    public static class StringExtension
    {
        public static string RemoveEnd(this string str, int len)
        {
            if (str.Length < len)
            { return string.Empty; }
            return str.Remove(str.Length - len);
        }
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            source = source.Substring(source.Length - tail_length);
            return source;
        }

    }
    public static class FormFileExtensions
    {
        public static byte[] GetBytes(this IFormFile formFile)
        {
            using var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
