using UnityEngine;

namespace BlockBlast.Game
{
    public static class HighScoreStore
    {
        const string Key = "BlockBlast.HighScore";

        public static int Load() => PlayerPrefs.GetInt(Key, 0);

        /// <returns>True when score is a new record.</returns>
        public static bool Submit(int score)
        {
            if (score <= Load()) return false;
            PlayerPrefs.SetInt(Key, score);
            PlayerPrefs.Save();
            return true;
        }
    }
}
