using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingLib_Phone
{
    public partial class ErrorItem : BindableBase
    {
        protected Guid m_Id;
        protected string m_Description;
        protected string m_Location;
        protected DateTime m_TimeStamp;
        protected ErrorTypes m_Type;
        protected Dictionary<string, string> m_FirstParamsKeyValueHolder;

        public Guid Id
        {
            get { return this.m_Id; }
            set { this.SetProperty(ref this.m_Id, value); }
        }

        public string Description
        {
            get { return this.m_Description; }
            set { this.SetProperty(ref this.m_Description, value); }
        }

        public string Location
        {
            get { return this.m_Location; }
            set { this.SetProperty(ref this.m_Location, value); }
        }

        public DateTime TimeStamp
        {
            get { return this.m_TimeStamp; }
            set { this.SetProperty(ref this.m_TimeStamp, value); }
        }

        public ErrorTypes Type
        {
            get { return this.m_Type; }
            set { this.SetProperty(ref this.m_Type, value); }
        }

        public Type CurrentPage { get; protected set; }

        public Type PreviousPage { get; protected set; }

        public Dictionary<string, string> FirstParamsKeyValueHolder
        {
            get { return m_FirstParamsKeyValueHolder; }
        }

        public ErrorItem()
        {
            m_Id = Guid.NewGuid();
            m_Description = string.Empty;
            m_Location = string.Empty;
            m_TimeStamp = DateTime.Now;
            m_Type = ErrorTypes.Error;
            CurrentPage = null;
            PreviousPage = null;
            m_FirstParamsKeyValueHolder = null;

        }  // of ErrorItem()

        public ErrorItem(string p_Description, string p_Location, DateTime p_TimeStamp, ErrorTypes p_Type)
            : this()
        {
            m_Description = p_Description;
            m_Location = p_Location;
            m_TimeStamp = p_TimeStamp;
            m_Type = p_Type;

        }  // of ErrorItem()

        public ErrorItem(string p_Description, string p_Location, DateTime p_TimeStamp)
            : this(p_Description, p_Location, p_TimeStamp, ErrorTypes.Error)
        {
        }  // of ErrorItem()

        public ErrorItem(string p_Description, string p_Location)
            : this(p_Description, p_Location, DateTime.Now)
        {
        }  // of ErrorItem()

        public ErrorItem(string p_Description, string p_Location, ErrorTypes p_Type)
            : this(p_Description, p_Location, DateTime.Now, p_Type)
        {
        }  // of ErrorItem()

        public ErrorItem(string p_Description, string p_Location, DateTime p_TimeStamp, ErrorTypes p_Type, Type p_CurrentPage, Type p_PreviousPage, Dictionary<string, string> p_FirstParamsKeyValueHolder)
            : this(p_Description, p_Location, p_TimeStamp, p_Type)
        {
            CurrentPage = p_CurrentPage;
            PreviousPage = p_PreviousPage;
            m_FirstParamsKeyValueHolder = p_FirstParamsKeyValueHolder;

        }  // of ErrorItem()

        public ErrorItem(string p_Description, string p_Location, ErrorTypes p_Type, Type p_CurrentPage, Type p_PreviousPage, Dictionary<string, string> p_FirstParamsKeyValueHolder)
            : this(p_Description, p_Location, DateTime.Now, p_Type, p_CurrentPage, p_PreviousPage, p_FirstParamsKeyValueHolder)
        {

        }  // of overloaded ErrorItem()

        public override string ToString()
        {
            return string.Format("{0}, in {1}, at {2}", Description, Location, TimeStamp);

        }  // of ToString()

    }  // of class ErrorItem

}  // of namespace ErrorHandlingLib_Phone
