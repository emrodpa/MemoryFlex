
namespace PlayLib
{
    public partial class Goal
    {
        public int MaxPLayChances { get; protected set; }
        public int MaxPlayingTime { get; protected set; }

        public Goal(int p_MaxPLayChances, int p_MaxPlayingTime)
        {
            MaxPLayChances = p_MaxPLayChances;
            MaxPlayingTime = p_MaxPlayingTime;
        
        }  // of constructor

        /*



        protected int m_NumberOfRightGuessess;
        protected int m_NumberOfWrongGuessess;
        protected DispatcherTimer m_Timer; 

        public int MaxNumberOfTries { get; protected set; }
        public int SequenceLength { get; protected set; }
        public int MaxTimeInMilliseconds { get; protected set; }

        public int NumberOfGuessess
        {
            get
            {
                return m_NumberOfRightGuessess + m_NumberOfWrongGuessess;
            }
        }

        public event DELActionEnded ActionEnded;

        protected Goal()
        {
            m_NumberOfRightGuessess = 0;
            m_NumberOfWrongGuessess = 0;
        
        }  // of parameterless constructor

        public Goal(int p_MaxNumberOfTries, int p_SequenceLength, int p_MaxTimeInMilliseconds)
            : this()
        {
            MaxNumberOfTries = p_MaxNumberOfTries;
            SequenceLength = p_SequenceLength;
            MaxTimeInMilliseconds = p_MaxTimeInMilliseconds;

            m_Timer = new DispatcherTimer();
            m_Timer.Interval = TimeSpan.FromMilliseconds(MaxTimeInMilliseconds);
            m_Timer.Tick += m_Timer_Tick;
        
        }  // of overloaded constructor

        public void Start()
        {
            m_Timer.Start();

        }  // of Start()

        void m_Timer_Tick(object sender, object e)
        {
            StopTimer();

            FireActionEnded(ActionEndTypes.TimeIsUp);

        }  // of m_Timer_Tick()

        public void StopTimer()
        {
            m_Timer.Stop();
            m_Timer.Tick -= m_Timer_Tick;

        }  // of StopTimer()

        protected void FireActionEnded(ActionEndTypes p_ActionEndType)
        {
            if (ActionEnded != null)
                ActionEnded(p_ActionEndType, this);
        
        }  // of FireActionEnded()

        public void AddWrongGuess()
        {
            m_NumberOfWrongGuessess++;

            if (NumberOfGuessess > MaxNumberOfTries)
                FireActionEnded(ActionEndTypes.ReachedMaximumGuessess);

        }  // of AddWrongGuess()

        public void AddRightGuess()
        {
            m_NumberOfRightGuessess++;

            if (m_NumberOfRightGuessess == SequenceLength)
                FireActionEnded(ActionEndTypes.ReachedSequenceLength);
        
        }  // of AddRightGuess()

        public void Dispose()
        {
            if (m_Timer.IsEnabled)
            {
                StopTimer();
            }
        


        }  // of Dispose()
        */

    }  // of class Goal

}  // of namespace PlayLib
