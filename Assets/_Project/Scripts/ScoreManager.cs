using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BasketballAudition
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI feedbackText; 
        
        [Header("Audio")]
        public AudioSource source;
        public AudioClip crowdCheer;
        
        private int currentScore = 0;
        private int currentStreak = 0;
        private bool scoredOnLastThrow = true;
        private bool isFirstThrow = true;
        private float feedbackFadeTime = 0f;

        private void Start()
        {
            UpdateUI();
            if (feedbackText != null) feedbackText.gameObject.SetActive(false);
            
            if (source == null) source = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (feedbackText != null && feedbackText.gameObject.activeSelf)
            {
                if (Time.time > feedbackFadeTime)
                {
                    feedbackText.gameObject.SetActive(false);
                }
            }
        }

        public void RegisterThrow()
        {
            isFirstThrow = false;
            scoredOnLastThrow = false;
        }

        public void RegisterRetrieve()
        {
            if (!scoredOnLastThrow && !isFirstThrow)
            {
                currentStreak = 0;
                ShowFeedback(GetRandomMockText(), 2f);
                UpdateUI();
                
                scoredOnLastThrow = true; 
            }
        }
        
        private string GetRandomMockText()
        {
            string[] mocks = new string[] 
            {
                "<color=#aaaaaa>BRICK!</color>",
                "<color=#aaaaaa>AIRBALL...</color>",
                "<color=#aaaaaa>NOT EVEN CLOSE.</color>",
                "<color=#aaaaaa>ARE YOU SICK?</color>",
                "<color=#aaaaaa>MY GRANDMA SHOOTS BETTER.</color>",
                "<color=#aaaaaa>EMBARRASSING.</color>",
                "<color=#aaaaaa>ARE YOUR EYES OPEN?</color>",
                "<color=#aaaaaa>PLEASE RETIRE.</color>",
                "<color=#aaaaaa>PATHETIC.</color>",
                "<color=#aaaaaa>WAS THAT ON PURPOSE?</color>",
                "<color=#aaaaaa>JUST STOP PLAYING.</color>"
            };
            return mocks[Random.Range(0, mocks.Length)];
        }

        private string GetRandomReluctantCompliment()
        {
            string[] comps = new string[] 
            {
                "Ugh, beginner's luck.",
                "Okay, that was decent.",
                "Are you cheating?",
                "I let you have that one.",
                "Lucky bounce.",
                "Fine, good shot.",
                "Whatever.",
                "Don't get used to it.",
                "I guess you're... kinda good."
            };
            return comps[Random.Range(0, comps.Length)].ToUpper();
        }

        public void RegisterNormalScore()
        {
            ProcessScore(2, "NICE!", 1f);
        }

        public void RegisterSwish()
        {
            ProcessScore(3, "SWISH!!", 1.5f);
            
            if (source && crowdCheer) source.PlayOneShot(crowdCheer);
            
            Time.timeScale = 0.5f;
            Invoke(nameof(ResetTime), 0.5f);
        }
        
        private void ProcessScore(int basePoints, string message, float duration)
        {
            if (!scoredOnLastThrow)
            {
                scoredOnLastThrow = true;
                currentStreak++;
            }
            else
            {
                // Edge case: someone scored twice without throwing (like a multi-ball setup)
                currentStreak++;
            }

            int comboMultiplier = Mathf.Clamp(currentStreak, 1, 5);
            int totalPoints = basePoints * comboMultiplier;
            currentScore += totalPoints;
            UpdateUI();
            
            if (currentStreak >= 3)
            {
                string reluctantCompliment = GetRandomReluctantCompliment();
                ShowFeedback($"<color=#FF8800>{reluctantCompliment} (x{currentStreak})</color>\n{message}\n+{totalPoints}", duration + 0.5f);
            }
            else
            {
                ShowFeedback($"{message}\n+{totalPoints}", duration);
            }
        }

        private void ResetTime()
        {
            Time.timeScale = 1f;
        }

        private void ShowFeedback(string msg, float duration = 1f)
        {
            if (feedbackText != null)
            {
                feedbackText.text = msg;
                feedbackText.gameObject.SetActive(true);
                feedbackFadeTime = Time.time + duration * Time.timeScale;
            }
        }

        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE: {currentScore:000}";
            }
        }
    }
}
