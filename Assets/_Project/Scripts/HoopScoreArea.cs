using System;
using UnityEngine;
using UnityEngine.Events;

namespace BasketballAudition
{
    public class HoopScoreArea : MonoBehaviour
    {
        [Header("Score Triggers")]
        public HoopTrigger topTrigger;
        public HoopTrigger bottomTrigger;


        [Header("Events")]
        public UnityEvent OnScore = new UnityEvent();
        public UnityEvent OnSwish = new UnityEvent();


        private float recentTopEntryTime;
        private Basketball recentBall;


        public float maxValidationTime = 1.0f;

        private void OnEnable()
        {
            if (topTrigger) topTrigger.OnBallEntered += HandleTopTrigger;
            if (bottomTrigger) bottomTrigger.OnBallEntered += HandleBottomTrigger;
        }

        private void OnDisable()
        {
            if (topTrigger) topTrigger.OnBallEntered -= HandleTopTrigger;
            if (bottomTrigger) bottomTrigger.OnBallEntered -= HandleBottomTrigger;
        }

        private void HandleTopTrigger(Basketball ball)
        {
            recentTopEntryTime = Time.time;
            recentBall = ball;
        }

        private void HandleBottomTrigger(Basketball ball)
        {
            if (ball == recentBall && (Time.time - recentTopEntryTime) <= maxValidationTime)
            {
                HandleValidScore(ball);
                recentBall = null;
            }
        }


        private void HandleValidScore(Basketball ball)
        {
            if (!ball.HasTouchedRimSinceThrow)
            {
                OnSwish?.Invoke();
            }


            OnScore?.Invoke();
        }
    }
}
