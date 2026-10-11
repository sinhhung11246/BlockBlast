using System;
using System.Collections;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>A rewarded video ad. Swap the placeholder for an ads SDK (e.g. LevelPlay) behind this interface.</summary>
    public interface IRewardedAds
    {
        /// <param name="done">Called with true when the player earned the reward.</param>
        void Show(Action<bool> done);
    }

    /// <summary>Placeholder ad: waits a few seconds, then always grants the reward.</summary>
    public sealed class FakeRewardedAds : MonoBehaviour, IRewardedAds
    {
        public float seconds = 2f;

        public void Show(Action<bool> done) => StartCoroutine(Play(done));

        IEnumerator Play(Action<bool> done)
        {
            yield return new WaitForSecondsRealtime(seconds);
            done(true);
        }
    }
}
