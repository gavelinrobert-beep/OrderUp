using NUnit.Framework;
using UnityEngine;
using OrderUp.Core;

namespace OrderUp.Tests
{
    public class GameStateManagerTests
    {
        private GameObject gameManagerObject;
        private GameObject scoreManagerObject;
        private GameObject gameStateManagerObject;

        [SetUp]
        public void SetUp()
        {
            gameManagerObject = new GameObject("GameManager");
            gameManagerObject.AddComponent<GameManager>();

            scoreManagerObject = new GameObject("ScoreManager");
            scoreManagerObject.AddComponent<ScoreManager>();

            gameStateManagerObject = new GameObject("GameStateManager");
            gameStateManagerObject.AddComponent<GameStateManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameStateManagerObject);
            Object.DestroyImmediate(scoreManagerObject);
            Object.DestroyImmediate(gameManagerObject);
        }

        [Test]
        public void GameStateManager_TracksRoundTransitions()
        {
            GameManager.Instance.StartRound();

            Assert.AreEqual(GameStateManager.GameState.InRound, GameStateManager.Instance.CurrentState);
            Assert.AreEqual(1, GameStateManager.Instance.CurrentRound);

            GameManager.Instance.PauseRound();
            Assert.AreEqual(GameStateManager.GameState.Paused, GameStateManager.Instance.CurrentState);

            GameManager.Instance.ResumeRound();
            Assert.AreEqual(GameStateManager.GameState.InRound, GameStateManager.Instance.CurrentState);

            GameManager.Instance.EndRound();
            Assert.AreEqual(GameStateManager.GameState.Summary, GameStateManager.Instance.CurrentState);
        }
    }

    public class ScoreManagerTests
    {
        private GameObject scoreManagerObject;

        [SetUp]
        public void SetUp()
        {
            scoreManagerObject = new GameObject("ScoreManager");
            scoreManagerObject.AddComponent<ScoreManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(scoreManagerObject);
        }

        [Test]
        public void ScoreManager_AddsScoreAndTracksOrders()
        {
            ScoreManager.Instance.ResetScore();
            ScoreManager.Instance.CompleteOrder(false, 50, 10f);
            ScoreManager.Instance.CompleteOrder(true, 75, 20f);
            ScoreManager.Instance.RecordMissedExpressOrder();

            Assert.AreEqual(125, ScoreManager.Instance.CurrentScore);
            Assert.AreEqual(2, ScoreManager.Instance.OrdersCompleted);
            Assert.AreEqual(1, ScoreManager.Instance.StandardOrdersCompleted);
            Assert.AreEqual(1, ScoreManager.Instance.ExpressOrdersCompleted);
            Assert.AreEqual(1, ScoreManager.Instance.MissedExpressOrders);
            Assert.AreEqual(15f, ScoreManager.Instance.AverageCompletionTime, 0.01f);
        }
    }
}
