namespace Helpers
{
    public static class PrettyLogs
    {
        public static void LogMultiple<T>(T[] values, string[] names)
        {
            string s = "";
            for (int i = 0; i < values.Length; i++)
            {
                s += $"{names[i]} : {values[i]}. ";
            }
            UnityEngine.Debug.Log(s);
        }
    
        public static void Log<T>(T value, string name)
        {
            UnityEngine.Debug.Log($"{name} : {value}");
        }
    }
}
