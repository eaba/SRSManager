using System;
using System.Collections.Generic;
using System.Threading;
using SrsManageCommon;
using Common = SrsManageCommon.Common;

namespace SRSWeb
{
    /// <summary>
    /// session class structure
    /// </summary>
    public class Session
    {
        private string _allowKey = null!;
        private long _expires; //expire date
        private string _refreshCode = null!;
        private string _sessionCode = null!;

        /// <summary>
        /// authorization key
        /// </summary>
        public string AllowKey
        {
            get => _allowKey;
            set => _allowKey = value;
        }

        /// <summary>
        /// session refresh code
        /// </summary>
        public string RefreshCode
        {
            get => _refreshCode;
            set => _refreshCode = value;
        }

        /// <summary>
        /// session code
        /// </summary>
        public string SessionCode
        {
            get => _sessionCode;
            set => _sessionCode = value;
        }

        /// <summary>
        /// expire date
        /// </summary>
        public long Expires
        {
            get => _expires;
            set => _expires = value;
        }
    }

    /// <summary>
    /// session management
    /// </summary>
    public class SessionManager
    {
        private List<Session> _sessionList = new List<Session>();

        private byte addMin = 50;

        /// <summary>
        /// Session management constructor
        /// </summary>
        /// <exception cref="Exception"></exception>
        public SessionManager()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    ClearExpires();
                }
                catch (Exception ex)
                { 
                    LogWriter.WriteLog("Session management thread starts abnormally...system exit", ex.Message+"\r\n"+ex.StackTrace);
                    Common.KillSelf();
                }
            })).Start();
        }

        /// <summary>
        /// session list
        /// </summary>
        public List<Session> SessionList
        {
            get => _sessionList;
            set => _sessionList = value;
        }

        /// <summary>
        /// Clear expired sessions
        /// </summary>
        private void ClearExpires()
        {
            while (true)
            {
                lock (this)
                {
                    foreach (var session in _sessionList)
                    {
                        if (session.Expires <= Program.CommonFunctions.GetTimeStampMilliseconds())
                        {
                            _sessionList.Remove(session);
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// refresh session
        /// </summary>
        /// <param name="session">old session</param>
        /// <returns></returns>
        public Session RefreshSession(Session session)
        {
            bool found = false;
            int i = 0;
            lock (this)
            {
                for (i = 0; i <= _sessionList.Count - 1; i++)
                {
                    if (_sessionList[i].AllowKey.Trim().ToLower().Equals(session.AllowKey.Trim().ToLower()) &&
                        _sessionList[i].RefreshCode.Trim().ToLower().Equals(session.RefreshCode.Trim().ToLower())
                    )
                    {
                        _sessionList[i].SessionCode = Common.CreateUuid()!;
                        _sessionList[i].RefreshCode = Common.CreateUuid()!;
                        _sessionList[i].Expires =
                            Program.CommonFunctions.GetTimeStampMilliseconds() + (addMin * 1000 * 60);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    return _sessionList[i];
                }
                else
                {
                    return null!;
                }
            }
        }

        /// <summary>
        /// Create a new session
        /// </summary>
        /// <param name="allowKey"></param>
        /// <returns></returns>
        public Session NewSession(string allowKey)
        {
            if (_sessionList != null)
            {
                Session s = _sessionList.FindLast(x => x.AllowKey.Trim().ToLower().Equals(allowKey.Trim().ToLower()))!;
                if (s != null)
                {
                    return s;
                }
            }

            Session session = new Session()
            {
                AllowKey = allowKey,
                SessionCode = Common.CreateUuid()!,
                RefreshCode = Common.CreateUuid()!,
                Expires = Program.CommonFunctions.GetTimeStampMilliseconds() + (addMin * 1000 * 60),
            };
            lock (this)
            {
                _sessionList!.Add(session);
            }

            return session;
        }
    }
}