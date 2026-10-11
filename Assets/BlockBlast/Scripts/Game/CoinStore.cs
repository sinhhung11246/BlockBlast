using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>The player's coins, saved in PlayerPrefs.</summary>
    public static class CoinStore
    {
        const string Key = "BlockBlast.Coins";

        public static int Coins => PlayerPrefs.GetInt(Key, 0);

        public static void Add(int amount)
        {
            PlayerPrefs.SetInt(Key, Coins + amount);
            PlayerPrefs.Save();
        }

        /// <returns>False, spending nothing, when there are not enough coins.</returns>
        public static bool Spend(int amount)
        {
            if (Coins < amount) return false;
            Add(-amount);
            return true;
        }
    }
}
