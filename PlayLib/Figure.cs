using System;

namespace PlayLib
{
    public partial class Figure
    {
        protected bool m_IsMarked;

        public Guid Id { get; protected set; }
        public string ImageFilename { get; protected set; }

        /// <summary>
        /// positions in the xaml grid
        /// </summary>
        public int RowPosition { get; set; }
        public int ColumnPosition { get; set; }

        /// <summary>
        /// when is successfuly identified 
        /// </summary>
        public bool IsMarked
        {
            get
            {
                return m_IsMarked;
            }
        }

        protected Figure()
        {
            Id = Guid.NewGuid();
            m_IsMarked = false;
            RowPosition = -1;
            ColumnPosition = -1;

        }  // of parameterless constructor

        public Figure(string p_ImageFilename)
            : this()
        {
            ImageFilename = p_ImageFilename;

        }  // of overloaded constructor

        public bool SetMarkedStatus(bool p_IsMarked)
        {
            bool l_bSuccess = true;

            if (m_IsMarked == p_IsMarked)
                l_bSuccess = false;
            else
                m_IsMarked = p_IsMarked;

            return l_bSuccess;

        }  // of SetMarkedStatus()

    }  // of class Figure

}  // of namespace PlayLib
