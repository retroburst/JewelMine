using JewelMine.Engine.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace JewelMine.Engine
{
    /// <summary>
    /// Encapsulates the game state models
    /// and all the logic and functionality for game play.
    /// </summary>
    public class GameLogic : IGameStateProvider
    {
        public Random Random { get; private set; }
        private JewelType[] jewelTypes = null;
        private GameState state = null;
        private GameGroupCollisionDetector collisionDetector = null;
        private GameLogicUserSettings userSettings = null;
        private static ILog logger = LogManager.GetLogger(typeof(GameLogic));

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public GameLogic(GameLogicUserSettings settings)
        {
            userSettings = settings;
            Initialise(userSettings.UserPreferredDifficulty);
        }

        /// <summary>
        /// Initialises this instance.
        /// </summary>
        private void Initialise(DifficultyLevel level)
        {
            jewelTypes = (JewelType[])Enum.GetValues(typeof(JewelType)).Cast<JewelType>().Where(x => x != JewelType.Unknown).ToArray();
            state = new GameState(level);
            Random = new Random();
            collisionDetector = new GameGroupCollisionDetector(state);
            AddInitialLinesToMine(state.Difficulty.InitialLines);
        }

        /// <summary>
        /// Initialises the specified saved game.
        /// </summary>
        /// <param name="savedGame">The saved game.</param>
        private void Initialise(SavedGameState savedGame)
        {
            jewelTypes = (JewelType[])Enum.GetValues(typeof(JewelType)).Cast<JewelType>().Where(x => x != JewelType.Unknown).ToArray();
            state = savedGame.State;
            Random = new Random();
            collisionDetector = new GameGroupCollisionDetector(state);
        }

        /// <summary>
        /// Gets the game state model.
        /// </summary>
        /// <value>
        /// The game state model.
        /// </value>
        public GameState State
        {
            get { return (state); }
        }

        /// <summary>
        /// Performs the game logic.
        /// </summary>
        /// <param name="logicInput">The logicInput.</param>
        /// <returns></returns>
        /// </exception>
        public GameLogicUpdate PerformGameLogic(GameLogicInput logicInput)
        {
            GameLogicUpdate logicUpdate = new GameLogicUpdate();
            bool immediateReturn = false;
            // process any game save or game load events
            ProcessGamePersistence(logicInput, logicUpdate, out immediateReturn);
            if (immediateReturn) { return (logicUpdate); }
            // process any game state changes - may need to return immediately after
            ProcessGameStateChange(logicInput, logicUpdate, out immediateReturn);
            if (immediateReturn) { return (logicUpdate); }
            // process any game difficulty changes
            ProcessGameDifficultyChange(logicInput, logicUpdate, out immediateReturn);
            if (immediateReturn) { return (logicUpdate); }
            // if we are in a playing state, we don't need to do anything else so let's return
            if (state.PlayState != GamePlayState.Playing) return (logicUpdate);
            // increment level if the score has reached the threshold for current level
            ProcessLevelAdvancement(logicInput, logicUpdate, out immediateReturn);
            if (immediateReturn) { return (logicUpdate); }
            // check for jewels that need to move down because of successful collisions
            MoveDownJewelsInLimbo(logicUpdate);
            // move delta based on logicInput or down by default if no logicInput
            bool userInputMovement = logicInput.DeltaMovement.HasValue;
            MovementType deltaMovement = logicInput.DeltaMovement.HasValue ? logicInput.DeltaMovement.Value : MovementType.Down;
            ProcessDeltaMovement(logicInput, logicUpdate, userInputMovement, deltaMovement);
            // check for new collisions and update existing
            ProcessCollisions(logicUpdate);
            // add a delta if required
            ProcessAddDelta(logicUpdate);
            return (logicUpdate);
        }

        /// <summary>
        /// Processes the game persistence.
        /// </summary>
        /// <param name="logicInput">The logic input.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="immediateReturn">if set to <c>true</c> [immediate return].</param>
        private void ProcessGamePersistence(GameLogicInput logicInput, GameLogicUpdate logicUpdate, out bool immediateReturn)
        {
            immediateReturn = false;
            if (logicInput.SaveGame)
            {
                TrySaveGame(logicUpdate);
            }
            else if (logicInput.LoadGame)
            {
                TryLoadGame(logicUpdate, out immediateReturn);
            }
        }

        /// <summary>
        /// Tries to load the game.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="immediateReturn">if set to <c>true</c> [immediate return].</param>
        private void TryLoadGame(GameLogicUpdate logicUpdate, out bool immediateReturn)
        {
            immediateReturn = false;
            try
            {
                SavedGameState saveGame = null;
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = File.Open(GameConstants.GAME_DEFAULT_SAVE_GAME_FILENAME, FileMode.Open))
                {
                    saveGame = (SavedGameState)formatter.Deserialize(fs);
                    fs.Close();
                }
                Initialise(saveGame);
                logicUpdate.GameLoaded = true;
                logicUpdate.Messages.Add(string.Format(GameConstants.GAME_MESSAGE_LOAD_GAME_PATTERN, "Done"));
                immediateReturn = true;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to load game.", ex);
                logicUpdate.Messages.Add(string.Format(GameConstants.GAME_MESSAGE_LOAD_GAME_PATTERN, "Failed"));
            }
        }

        /// <summary>
        /// Tries to save the game.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void TrySaveGame(GameLogicUpdate logicUpdate)
        {
            try
            {
                SavedGameState saveGame = new SavedGameState();
                saveGame.SavedOn = DateTime.Now;
                saveGame.State = state;
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = File.Open(GameConstants.GAME_DEFAULT_SAVE_GAME_FILENAME, FileMode.OpenOrCreate))
                {
                    fs.SetLength(0);
                    formatter.Serialize(fs, saveGame);
                    fs.Close();
                }
                logicUpdate.Messages.Add(string.Format(GameConstants.GAME_MESSAGE_SAVE_GAME_PATTERN, "Done"));
            }
            catch (Exception ex)
            {
                logger.Error("Failed to save game.", ex);
                logicUpdate.Messages.Add(string.Format(GameConstants.GAME_MESSAGE_SAVE_GAME_PATTERN, "Failed"));
            }
        }

        /// <summary>
        /// Processes the game difficulty change.
        /// </summary>
        /// <param name="logicInput">The logic input.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="immediateReturn">if set to <c>true</c> [immediate return].</param>
        private void ProcessGameDifficultyChange(GameLogicInput logicInput, GameLogicUpdate logicUpdate, out bool immediateReturn)
        {
            immediateReturn = false;
            if (logicInput.ChangeDifficulty)
            {
                immediateReturn = true;
                DifficultyLevel currentLevel = state.Difficulty.DifficultyLevel;
                DifficultyLevel nextLevel = GameDifficulty.FindNextDifficultyLevel(currentLevel);
                Initialise(nextLevel);
                logicUpdate.GameStarted = true;
                state.PlayState = GamePlayState.Playing;
            }
        }

        /// <summary>
        /// Determines whether [is game state change] [the specified logic update].
        /// </summary>
        /// <param name="logicInput">The logic logicInput.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private void ProcessGameStateChange(GameLogicInput logicInput, GameLogicUpdate logicUpdate, out bool immediateReturn)
        {
            immediateReturn = false;
            if (logicInput.RestartGame)
            {
                RestartGame(logicUpdate);
                immediateReturn = true;
            }
            else if (logicInput.PauseGame)
            {
                PauseGame(logicUpdate);
                immediateReturn = true;
            }
            else if (logicInput.GameStarted)
            {
                StartGame(logicUpdate);
                immediateReturn = false;
            }
        }

        /// <summary>
        /// Processes the level advancement.
        /// </summary>
        /// <param name="logicInput">The logic input.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="immediateReturn">if set to <c>true</c> [immediate return].</param>
        private void ProcessLevelAdvancement(GameLogicInput logicInput, GameLogicUpdate logicUpdate, out bool immediateReturn)
        {
            immediateReturn = false;
            if (state.Mine.IsEmpty())
            {
                GameWon(logicUpdate);
                immediateReturn = true;
            }
            else if (LevelThresholdReached())
            {
                if (state.Level == state.Difficulty.LastLevel)
                {
                    GameWon(logicUpdate);
                    immediateReturn = true;
                }
                else
                {
                    IncrementLevel(logicUpdate);
                    immediateReturn = false;
                }
            }
        }

        /// <summary>
        /// Processes the delta movement.
        /// </summary>
        /// <param name="logicInput">The logic input.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="userInputMovement">if set to <c>true</c> [user input movement].</param>
        /// <param name="deltaMovement">The delta movement.</param>
        private void ProcessDeltaMovement(GameLogicInput logicInput, GameLogicUpdate logicUpdate, bool userInputMovement, MovementType deltaMovement)
        {
            MovementType originalDeltaMovement = deltaMovement;
            if (state.Mine.Delta != null)
            {
                if (logicInput.DeltaSwapJewels) { SwapDeltaJewels(logicUpdate); }
                bool deltaStationary = false;
                int numPositionsToMove = 1;
                // if delta is up against a boundary on either side that the movement is towards, override and move delta down instead
                if (IsDeltaAgainstBoundary(deltaMovement) || IsDeltaAgainstMineObject(deltaMovement)) deltaMovement = MovementType.Down;
                // if the user pressed down - drop the delta all the way
                if (userInputMovement && originalDeltaMovement == MovementType.Down) numPositionsToMove = state.Mine.Depth;
                MoveDelta(deltaMovement, logicUpdate, numPositionsToMove);
                if (IsDeltaStationary()) deltaStationary = true;
                if (deltaStationary)
                {
                    if (state.Mine.Delta.StationarySince.HasValue && (state.Mine.Delta.StationarySince.Value + state.Difficulty.DeltaStationaryTimeSpan) < DateTime.Now)
                    {
                        // delta is now in position and a new one will be added
                        state.Mine.Delta = null;
                    }
                    else
                    {
                        if (!state.Mine.Delta.StationarySince.HasValue)
                        {
                            state.Mine.Delta.StationarySince = DateTime.Now;
                            logicUpdate.DeltaStationary = true;
                        }
                    }
                }
                else
                {
                    state.Mine.Delta.StationarySince = null;
                }
            }
        }

        /// <summary>
        /// Processes the collisions.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void ProcessCollisions(GameLogicUpdate logicUpdate)
        {
            state.Mine.MarkedCollisions.ForEach(x => x.IncrementCollisionTickCount());
            var markedCollisionsForFinalising = state.Mine.MarkedCollisions.Where(x => x.CollisionTickCount >= state.CollisionFinailseTickCount).ToArray();
            collisionDetector.FinaliseCollisionGroups(logicUpdate, markedCollisionsForFinalising);
            state.Score += GameHelpers.CalculateScore(markedCollisionsForFinalising, state.Difficulty.GroupCollisionScore);
            collisionDetector.MarkGroupCollisions(logicUpdate);
        }

        /// <summary>
        /// Processes the add delta.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void ProcessAddDelta(GameLogicUpdate logicUpdate)
        {
            // if no delta add a new one
            if (state.Mine.Delta == null)
            {
                bool added = AddDelta(logicUpdate);
                if (!added) { GameOver(logicUpdate); }
            }
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void StartGame(GameLogicUpdate logicUpdate)
        {
            state.PlayState = GamePlayState.Playing;
            logicUpdate.GameStarted = true;
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void PauseGame(GameLogicUpdate logicUpdate)
        {
            state.PlayState = GamePlayState.Paused;
            logicUpdate.GamePaused = true;
        }

        /// <summary>
        /// Games the over.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void GameOver(GameLogicUpdate logicUpdate)
        {
            state.PlayState = GamePlayState.GameOver;
            logicUpdate.GameOver = true;
        }

        /// <summary>
        /// Games the won.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void GameWon(GameLogicUpdate logicUpdate)
        {
            state.PlayState = GamePlayState.GameWon;
            logicUpdate.GameWon = true;
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void RestartGame(GameLogicUpdate logicUpdate)
        {
            DifficultyLevel currentLevel = state.Difficulty.DifficultyLevel;
            Initialise(currentLevel);
            state.PlayState = GamePlayState.Playing;
            logicUpdate.GameStarted = true;
        }

        /// <summary>
        /// Indicates if the level threshold was reached.
        /// </summary>
        /// <returns></returns>
        private bool LevelThresholdReached()
        {
            return (state.Score >= state.Level * state.Difficulty.LevelIncrementScoreThreshold);
        }

        /// <summary>
        /// Increments the level.
        /// </summary>
        private void IncrementLevel(GameLogicUpdate logicUpdate)
        {
            logicUpdate.LevelIncremented = true;
            state.Level += 1;
            if (state.TickSpeedMilliseconds == state.Difficulty.TickSpeedMillisecondsFloor) return;
            // calculate a new speed
            double newTickSpeed = state.TickSpeedMilliseconds - state.Difficulty.LevelIncrementSpeedChange;
            if (newTickSpeed >= state.Difficulty.TickSpeedMillisecondsFloor)
            {
                state.TickSpeedMilliseconds -= state.Difficulty.LevelIncrementSpeedChange;
            }
            else
            {
                double difference = newTickSpeed - state.Difficulty.TickSpeedMillisecondsFloor;
                if (difference < state.Difficulty.LevelIncrementSpeedChange)
                {
                    state.TickSpeedMilliseconds = state.Difficulty.TickSpeedMillisecondsFloor;
                }
            }
        }

        /// <summary>
        /// Moves down jewels in limbo.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void MoveDownJewelsInLimbo(GameLogicUpdate logicUpdate)
        {
            for (int x = state.Mine.Grid.GetUpperBound(0); x >= 0; x--)
            {
                for (int y = state.Mine.Grid.GetUpperBound(1); y >= 0; y--)
                {
                    MineObject mineObject = state.Mine.Grid[x, y];
                    if (!(mineObject is Jewel)) continue;
                    if (state.Mine.Grid[x, y] != null && (state.Mine.Delta == null || !state.Mine.Delta.IsGroupMember((Jewel)mineObject)))
                    {
                        if (state.Mine.CoordinatesInBounds(new Coordinates(x, y + 1)))
                        {
                            // if the position under has nothing, need to move the jewel down
                            if (state.Mine.Grid[x, y + 1] == null) MoveJewel(new Coordinates(x, y), MovementType.Down, logicUpdate);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Swaps the delta jewels downwards.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void SwapDeltaJewels(GameLogicUpdate logicUpdate)
        {
            JewelGroup delta = state.Mine.Delta;
            Jewel top = delta.Top.Jewel;
            delta.Top.Jewel = delta.Bottom.Jewel;
            delta.Bottom.Jewel = delta.Middle.Jewel;
            delta.Middle.Jewel = top;
            if (state.Mine.CoordinatesInBounds(delta.Top.Coordinates)) state.Mine[delta.Top.Coordinates] = delta.Top.Jewel;
            if (state.Mine.CoordinatesInBounds(delta.Middle.Coordinates)) state.Mine[delta.Middle.Coordinates] = delta.Middle.Jewel;
            if (state.Mine.CoordinatesInBounds(delta.Bottom.Coordinates)) state.Mine[delta.Bottom.Coordinates] = delta.Bottom.Jewel;
            logicUpdate.DeltaJewelsSwapped = true;
        }

        /// <summary>
        /// Determines whether [is delta against boundary] [the specified delta movement].
        /// </summary>
        /// <param name="deltaMovement">The delta movement.</param>
        /// <returns></returns>
        private bool IsDeltaAgainstBoundary(MovementType deltaMovement)
        {
            bool result = false;
            JewelGroup delta = state.Mine.Delta;
            if ((deltaMovement == MovementType.Left && (delta.Bottom.Coordinates.X == 0))
            || (deltaMovement == MovementType.Right && delta.Bottom.Coordinates.X == state.Mine.Grid.GetUpperBound(0)))
            {
                result = true;
            }
            return (result);
        }

        /// <summary>
        /// Determines whether [is delta against mine object] [the specified delta movement].
        /// </summary>
        /// <param name="deltaMovement">The delta movement.</param>
        /// <returns></returns>
        private bool IsDeltaAgainstMineObject(MovementType deltaMovement)
        {
            bool result = false;
            JewelGroup delta = state.Mine.Delta;
            Coordinates left = FindClosestLeftPositionForDelta(delta, 1);
            Coordinates right = FindClosestRightPositionForDelta(delta, 1);
            if ((deltaMovement == MovementType.Left && left == null)
            || (deltaMovement == MovementType.Right && right == null))
            {
                result = true;
            }
            return (result);
        }

        /// <summary>
        /// Determines whether [is delta stationary].
        /// </summary>
        /// <returns></returns>
        private bool IsDeltaStationary()
        {
            JewelGroup delta = state.Mine.Delta;
            return (delta.Bottom.Coordinates.Y == state.Mine.Grid.GetUpperBound(1)
                || state.Mine.Grid[delta.Bottom.Coordinates.X, delta.Bottom.Coordinates.Y + 1] != null);
        }

        /// <summary>
        /// Finds the jewel types around target. Returns the names.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns></returns>
        private JewelType[] FindJewelTypesAroundTarget(Coordinates coordinates)
        {
            List<JewelType> surroundingTypes = new List<JewelType>();
            Coordinates[] surrounding = Coordinates.GenerateSurroundingCoordinates(coordinates);
            foreach (Coordinates surroundingCoordinate in surrounding)
            {
                if (state.Mine.CoordinatesInBounds(surroundingCoordinate) && !state.Mine.CoordinatesAvailable(surroundingCoordinate))
                {
                    MineObject mineObject = state.Mine[surroundingCoordinate];
                    if (mineObject is Jewel)
                    {
                        Jewel surroundingJewel = (Jewel)mineObject;
                        surroundingTypes.Add(surroundingJewel.JewelType);
                    }
                }
            }
            return (surroundingTypes.ToArray());
        }

        /// <summary>
        /// Adds the initial lines to mine.
        /// </summary>
        /// <param name="numLines">The number lines.</param>
        private void AddInitialLinesToMine(int numLines)
        {
            for (int y = state.Mine.Depth - 1; y > state.Mine.Depth - (numLines + 1); y--)
            {
                for (int x = state.Mine.Columns - 1; x >= 0; x--)
                {
                    bool addJewelAtPosition = true;
                    if (y == (state.Mine.Depth - numLines))
                    {
                        // if top line, don't always add a jewel - gives a ragged random look
                        addJewelAtPosition = (Random.Next(0, 2) == 1);
                    }
                    if (addJewelAtPosition)
                    {
                        state.Mine.Grid[x, y] = GenerateRandomJewelAvoidCollision(new Coordinates(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// Generates the type of the random jewel.
        /// </summary>
        /// <returns></returns>
        private JewelType GenerateRandomJewelType()
        {
            int randomIndex = Random.Next(0, jewelTypes.Length);
            return (jewelTypes[randomIndex]);
        }

        /// <summary>
        /// Generates the random type of the jewel.
        /// </summary>
        /// <param name="avoidTypes">The avoid types.</param>
        /// <returns></returns>
        private JewelType GenerateRandomJewelType(params JewelType[] avoidTypes)
        {
            JewelType[] jewelTypesToUse = jewelTypes.Where(x => !avoidTypes.Contains(x)).ToArray();
            // if surrounded by all types of jewels (i.e. avoid all types) return a random
            if (jewelTypesToUse.Length == 0) return (GenerateRandomJewelType());
            int randomIndex = Random.Next(0, jewelTypesToUse.Length);
            return (jewelTypesToUse[randomIndex]);
        }

        /// <summary>
        /// Generates the random jewel avoid collision.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns></returns>
        private Jewel GenerateRandomJewelAvoidCollision(Coordinates coordinates)
        {
            JewelType[] avoidTypes = FindJewelTypesAroundTarget(coordinates);
            Jewel result = new Jewel(GenerateRandomJewelType(avoidTypes));
            return (result);
        }

        /// <summary>
        /// Adds the delta.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private bool AddDelta(GameLogicUpdate logicUpdate)
        {
            int[] free = FindFreeCoordinatesForDelta();
            if (free.Length == 0)
            {
                return (false);
            }
            int randomIndex = Random.Next(0, free.Length);
            int targetCoorindinate = free[randomIndex];
            JewelGroup delta = GenerateRandomDeltaJewelGroup();
            state.Mine.Delta = delta;
            state.Mine.Grid[targetCoorindinate, 0] = delta.Bottom.Jewel;
            delta.Bottom.Coordinates.X = targetCoorindinate;
            delta.Bottom.Coordinates.Y = 0;
            delta.Bottom.HasEnteredBounds = true;
            logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Bottom.Jewel, Original = Coordinates.CreateInvalidatedCoordinates(), New = new Coordinates(targetCoorindinate, 0) });
            return (true);
        }

        /// <summary>
        /// Finds the free coordinates for delta.
        /// </summary>
        /// <returns></returns>
        private int[] FindFreeCoordinatesForDelta()
        {
            List<int> free = new List<int>();
            for (int i = 0; i < state.Mine.Columns; i++)
            {
                if (state.Mine.Grid[i, 0] == null) free.Add(i);
            }
            return (free.ToArray());
        }

        /// <summary>
        /// Generates the random delta delta.
        /// </summary>
        /// <returns></returns>
        private JewelGroup GenerateRandomDeltaJewelGroup()
        {
            Jewel[] randomJewels = new Jewel[3];
            double tripleJewelChance = Random.NextDouble();
            double doubleJewelChance = Random.NextDouble();

            JewelType firstRandomJewelType = GenerateRandomJewelType();
            randomJewels[0] = new Jewel(firstRandomJewelType);

            double tripleJewelChanceForLevel = CalculateChanceBasedOnLevel(state.Level, state.Difficulty.LastLevel, state.Difficulty.DeltaTripleJewelChance, state.Difficulty.DeltaTripleJewelChanceFloor);
            double doubleJewelChanceForLevel = CalculateChanceBasedOnLevel(state.Level, state.Difficulty.LastLevel, state.Difficulty.DeltaDoubleJewelChance, state.Difficulty.DeltaDoubleJewelChanceFloor);
            if (tripleJewelChance <= tripleJewelChanceForLevel)
            {
                randomJewels[1] = new Jewel(firstRandomJewelType);
                randomJewels[2] = new Jewel(firstRandomJewelType);
            }
            else if (doubleJewelChance <= doubleJewelChanceForLevel)
            {
                randomJewels[1] = new Jewel(firstRandomJewelType);
                randomJewels[2] = new Jewel(GenerateRandomJewelType(firstRandomJewelType));
            }
            else
            {
                randomJewels[1] = new Jewel(GenerateRandomJewelType(firstRandomJewelType));
                JewelType secondRandomJewel = randomJewels[1].JewelType;
                randomJewels[2] = new Jewel(GenerateRandomJewelType(firstRandomJewelType, secondRandomJewel));
            }
            return (new JewelGroup(randomJewels[0], randomJewels[1], randomJewels[2]));
        }

        /// <summary>
        /// Moves the delta.
        /// </summary>
        /// <param name="movement">The movement.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private bool MoveDelta(MovementType movement, GameLogicUpdate logicUpdate, int numPositionsToMove)
        {
            JewelGroup delta = state.Mine.Delta;
            Coordinates targetCoordinates = null;
            switch (movement)
            {
                case MovementType.Down:
                    targetCoordinates = FindClosestDownPositionForDelta(delta, numPositionsToMove);
                    break;
                case MovementType.Left:
                    targetCoordinates = FindClosestLeftPositionForDelta(delta, numPositionsToMove);
                    break;
                case MovementType.Right:
                    targetCoordinates = FindClosestRightPositionForDelta(delta, numPositionsToMove);
                    break;
            }

            if (targetCoordinates == null) return (false);
            // save current positions
            Coordinates originalTop = delta.Top.Coordinates;
            Coordinates originalMiddle = delta.Middle.Coordinates;
            Coordinates originalBottom = delta.Bottom.Coordinates;
            // clear grid
            ClearGridPosition(delta.Top.Coordinates);
            ClearGridPosition(delta.Middle.Coordinates);
            ClearGridPosition(delta.Bottom.Coordinates);
            // shuffle down
            delta.Top.Coordinates = new Coordinates(targetCoordinates.X, targetCoordinates.Y - 2);
            delta.Middle.Coordinates = new Coordinates(targetCoordinates.X, targetCoordinates.Y - 1);
            delta.Bottom.Coordinates = targetCoordinates;
            if (state.Mine.CoordinatesInBounds(delta.Top.Coordinates))
            {
                if (!delta.Top.HasEnteredBounds) delta.Top.HasEnteredBounds = true;
                state.Mine[delta.Top.Coordinates] = delta.Top.Jewel;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Top.Jewel, Original = originalTop, New = delta.Top.Coordinates });
            }
            if (state.Mine.CoordinatesInBounds(delta.Middle.Coordinates))
            {
                if (!delta.Middle.HasEnteredBounds) delta.Middle.HasEnteredBounds = true;
                state.Mine[delta.Middle.Coordinates] = delta.Middle.Jewel;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Middle.Jewel, Original = originalMiddle, New = delta.Middle.Coordinates });
            }
            if (state.Mine.CoordinatesInBounds(delta.Bottom.Coordinates))
            {
                if (!delta.Bottom.HasEnteredBounds) delta.Bottom.HasEnteredBounds = true;
                state.Mine[delta.Bottom.Coordinates] = delta.Bottom.Jewel;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = delta.Bottom.Jewel, Original = originalBottom, New = delta.Bottom.Coordinates });
            }
            return (true);
        }

        /// <summary>
        /// Finds the closest down position for delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private Coordinates FindClosestDownPositionForDelta(JewelGroup delta, int numPositionsToMove)
        {
            Coordinates result = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                Coordinates newBottom = new Coordinates(delta.Bottom.Coordinates.X, delta.Bottom.Coordinates.Y + i);
                Coordinates newMiddle = new Coordinates(delta.Middle.Coordinates.X, delta.Middle.Coordinates.Y + i);
                Coordinates newTop = new Coordinates(delta.Top.Coordinates.X, delta.Top.Coordinates.Y + i);

                if (state.Mine.CoordinatesInBounds(newBottom) && state.Mine.CoordinatesAvailable(newBottom)
                    && (!delta.Middle.HasEnteredBounds || (state.Mine.CoordinatesInBounds(newMiddle)))
                    && (!delta.Top.HasEnteredBounds || (state.Mine.CoordinatesInBounds(newTop))))
                {
                    result = newBottom;
                }
                else
                {
                    break;
                }
            }
            return (result);
        }

        /// <summary>
        /// Finds the closest left position for delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private Coordinates FindClosestLeftPositionForDelta(JewelGroup delta, int numPositionsToMove)
        {
            Coordinates result = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                Coordinates newBottom = new Coordinates(delta.Bottom.Coordinates.X - i, delta.Bottom.Coordinates.Y);
                Coordinates newMiddle = new Coordinates(delta.Middle.Coordinates.X - i, delta.Middle.Coordinates.Y);
                Coordinates newTop = new Coordinates(delta.Top.Coordinates.X - i, delta.Top.Coordinates.Y);

                if (state.Mine.CoordinatesInBounds(newBottom) && state.Mine.CoordinatesAvailable(newBottom)
                    && (!delta.Middle.HasEnteredBounds || (state.Mine.CoordinatesInBounds(newMiddle) && state.Mine.CoordinatesAvailable(newMiddle)))
                    && (!delta.Top.HasEnteredBounds || (state.Mine.CoordinatesInBounds(newTop) && state.Mine.CoordinatesAvailable(newTop))))
                {
                    result = newBottom;
                }
                else
                {
                    break;
                }
            }
            return (result);
        }

        /// <summary>
        /// Finds the closest right position for delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        private Coordinates FindClosestRightPositionForDelta(JewelGroup delta, int numPositionsToMove)
        {
            Coordinates result = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                Coordinates newBottom = new Coordinates(delta.Bottom.Coordinates.X + i, delta.Bottom.Coordinates.Y);
                Coordinates newMiddle = new Coordinates(delta.Middle.Coordinates.X + i, delta.Middle.Coordinates.Y);
                Coordinates newTop = new Coordinates(delta.Top.Coordinates.X + i, delta.Top.Coordinates.Y);

                if (state.Mine.CoordinatesInBounds(newBottom) && state.Mine.CoordinatesAvailable(newBottom)
                    && (!delta.Middle.HasEnteredBounds || (state.Mine.CoordinatesInBounds(newMiddle) && state.Mine.CoordinatesAvailable(newMiddle)))
                    && (!delta.Top.HasEnteredBounds || (state.Mine.CoordinatesInBounds(newTop) && state.Mine.CoordinatesAvailable(newTop))))
                {
                    result = newBottom;
                }
                else
                {
                    break;
                }
            }
            return (result);
        }

        /// <summary>
        /// Moves the delta.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="movement">The movement.</param>
        /// <param name="logicUpdate">The logic update.</param>
        /// <returns></returns>
        private bool MoveJewel(Coordinates coordinates, MovementType movement, GameLogicUpdate logicUpdate)
        {
            if (coordinates == null) throw new ArgumentException("Argument cannot be null.", "coordinates");
            if (state.Mine.CoordinatesInBounds(coordinates))
            {
                Coordinates targetCoordinates = null;
                switch (movement)
                {
                    case MovementType.Down:
                        targetCoordinates = FindClosestDownPosition(coordinates);
                        break;
                    case MovementType.Left:
                        targetCoordinates = FindClosestLeftPosition(coordinates);
                        break;
                    case MovementType.Right:
                        targetCoordinates = FindClosestRightPosition(coordinates);
                        break;
                }
                if (targetCoordinates == null) return (false);
                // otherwise we can move the delta to the required or closest position
                Jewel target = (Jewel)state.Mine[coordinates];
                state.Mine[targetCoordinates] = target;
                state.Mine[coordinates] = null;
                logicUpdate.JewelMovements.Add(new JewelMovement() { Jewel = target, Original = coordinates, New = targetCoordinates });
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// Finds the closest down position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        private Coordinates FindClosestDownPosition(Coordinates target, int numPositionsToMove = 1)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            Coordinates result = null;
            int? closestY = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                if (state.Mine.CoordinatesInBounds(new Coordinates(target.X, target.Y + i))
                    && state.Mine.CoordinatesAvailable(new Coordinates(target.X, target.Y + i))) closestY = target.Y + i;
                else break;
            }
            if (closestY.HasValue) result = new Coordinates(target.X, closestY.Value);
            return (result);
        }

        /// <summary>
        /// Finds the closest left position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        private Coordinates FindClosestLeftPosition(Coordinates target, int numPositionsToMove = 1)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            Coordinates result = null;
            int? closestX = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                if (state.Mine.CoordinatesInBounds(new Coordinates(target.X - i, target.Y))
                    && state.Mine.CoordinatesAvailable(new Coordinates(target.X - i, target.Y))) closestX = target.X - i;
                else break;
            }
            if (closestX.HasValue) result = new Coordinates(closestX.Value, target.Y);
            return (result);
        }

        /// <summary>
        /// Finds the closest right position.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="numPositionsToMove">The number positions to move.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Argument cannot be null.;target</exception>
        private Coordinates FindClosestRightPosition(Coordinates target, int numPositionsToMove = 1)
        {
            if (target == null) throw new ArgumentException("Argument cannot be null.", "target");
            Coordinates result = null;
            int? closestX = null;
            for (int i = 1; i <= numPositionsToMove; i++)
            {
                if (state.Mine.CoordinatesInBounds(new Coordinates(target.X + i, target.Y))
                    && state.Mine.CoordinatesAvailable(new Coordinates(target.X + i, target.Y))) closestX = target.X + i;
                else break;
            }
            if (closestX.HasValue) result = new Coordinates(closestX.Value, target.Y);
            return (result);
        }

        /// <summary>
        /// Clears the grid position.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        private void ClearGridPosition(Coordinates coordinates)
        {
            if (coordinates != null && state.Mine.CoordinatesInBounds(coordinates))
            {
                state.Mine[coordinates] = null;
            }
        }


        /// <summary>
        /// Calculates the chance based on level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="lastLevel">The number of levels.</param>
        /// <param name="defaultChance">The default chance above.</param>
        /// <param name="floorChance">The ceiling chance above.</param>
        /// <returns></returns>
        private double CalculateChanceBasedOnLevel(int currentLevel, int lastLevel, double defaultChance, double floorChance)
        {
            double ratio = (defaultChance - floorChance) / lastLevel;
            double currentChance = defaultChance - (ratio * (currentLevel - 1));
            currentChance = Math.Min(currentChance, defaultChance);
            return (Math.Max(currentChance, floorChance));
        }

    }
}
