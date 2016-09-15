using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayLib_Phone
{
    public enum PlayLevelTypes { Beginner, Intermediate, Advanced, Fuggedaboutit, NoChancesLeft }

    public enum PlayStatuses { Started, Stopped, Finished }

    public enum PlayFinishingTypes { Success, NoChancesLeft, TimeExpired, StoppedByUser }

    public delegate void DELPlayStatusChanged(PlayStatuses p_NewPlayStatus, PlayStatuses p_OldPlayStatus, bool p_bSuccess);
    public delegate void DELGuessHappened(Figure p_Figure, int p_GuessCount, bool p_IsCorrect);
    public delegate void DELPlayFinished(PlayFinishingTypes p_PlayFinishingType, DateTime p_DateTime, int p_NumberOfRightGuesses, int p_NumberOfWrongGuesses);

    public partial class Play
    {
        public Sequence oSequence { get; protected set; }
        public int NumberOfChancesPlayed { get; protected set; }
        public FactorySequences FacSequences { get; protected set; }
        public List<Guid> WrongGuesses { get; protected set; }

        public int NumberOfCorrectGuessess
        {
            get { return FacSequences.FigureList.Count(f => f.IsMarked); }
        }

        #region Events

        public event DELPlayStatusChanged PlayStatusChanged;
        public event DELGuessHappened GuessHappened;
        public event DELPlayFinished PlayFinished;

        #endregion Events

        #region Constructors

        protected Play()
        {
            NumberOfChancesPlayed = 0;
            WrongGuesses = new List<Guid>();

        }  // of parameterless constructor

        public Play(Sequence p_oSequence, FactorySequences p_FacSequences)
            : this()
        {
            oSequence = p_oSequence;
            FacSequences = p_FacSequences;

        }  // of overloaded constructor

        public async Task<bool> StartPlayingAsync()
        {
            bool l_bSuccess = true;

            NumberOfChancesPlayed = 0;

            return l_bSuccess;

        }  // of StartPlayingAsync()

        public void MakeGuess(Guid p_FigureId)
        {
            bool l_IsRightGuess = IsRightGuess(p_FigureId);

            if (l_IsRightGuess)
            {
                Figure l_Figure = FacSequences.FigureList.Find(f => f.Id == oSequence.FigureIDList.Find(fid => fid.Equals(p_FigureId)));

                if (!l_Figure.SetMarkedStatus(true))
                    throw new Exception("issue when setting up figure in sequence as marked");

                FireGuessHappened(p_FigureId, NumberOfCorrectGuessess, true);

                if (NumberOfCorrectGuessess == oSequence.FigureIDList.Count)
                {
                    // end play
                    FirePlayFinished(PlayFinishingTypes.Success);
                }
            }
            else
            {
                WrongGuesses.Add(p_FigureId);

                FireGuessHappened(p_FigureId, WrongGuesses.Count, false);

                if (WrongGuesses.Count >= FacSequences.oSettingsForPlay.oGoal.MaxPLayChances)
                {
                    // end play
                    FirePlayFinished(PlayFinishingTypes.NoChancesLeft);
                }

            }  // of if/else

        }  // of MakeGuess()

        #endregion Constructors

        #region Fire Methods

        protected void FirePlayStatusChanged(PlayStatuses p_NewPlayStatus, PlayStatuses p_OldPlayStatus, bool p_bSuccess)
        {
            if (PlayStatusChanged != null)
                PlayStatusChanged(p_NewPlayStatus, p_OldPlayStatus, p_bSuccess);
        }  // of FirePlayStatusChanged()

        protected void FireGuessHappened(Guid p_FigureId, int p_GuessCount, bool p_IsCorrect)
        {
            if (GuessHappened != null)
            {
                Figure l_Figure = FacSequences.FigureList.Find(f => f.Id.Equals(p_FigureId));
                GuessHappened(l_Figure, p_GuessCount, p_IsCorrect);
            }
        }  // of FireGuessHappened()

        protected void FirePlayFinished(PlayFinishingTypes p_PlayFinishingType)
        {
            if (PlayFinished != null)
                PlayFinished(p_PlayFinishingType, DateTime.Now, NumberOfCorrectGuessess, WrongGuesses.Count);

        }  // of FirePlayFinished()

        #endregion Fire Methods

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_FigureId">guessed figure's id</param>
        /// <returns>true if the guessed figure is the next unmarked in the sequence, false otherwise</returns>
        public bool IsRightGuess(Guid p_FigureId)
        {
            bool l_IsRightGuess = true;
            Figure l_Figure;

            int l_Index = oSequence.FigureIDList.FindIndex(fid => fid.Equals(p_FigureId));

            if (l_Index != -1)
            {
                l_Figure = FacSequences.FigureList.Find(f => f.Id.Equals(p_FigureId));

                if (!l_Figure.IsMarked)
                {
                    l_Figure = null;
                    for (int i = 0; i < l_Index; i++)
                    {
                        l_Figure = FacSequences.FigureList.Find(f => f.Id.Equals(oSequence.FigureIDList[i]));
                        if (!l_Figure.IsMarked)
                        {
                            l_IsRightGuess = false;
                            break;
                        }  // of if
                        l_Figure = null;
                    }  // of loop over figures in sequence
                }
                else
                {
                    l_IsRightGuess = false;

                }  // of if/else
            }
            else
            {
                l_IsRightGuess = false;
            }  // of if/else

            return l_IsRightGuess;

        }  // of IsRightGuess()

        #endregion Private Methods

    }  // of class Play

}  // of namespace PlayLib_Phone
