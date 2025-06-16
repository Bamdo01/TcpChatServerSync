﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServerSync.Model.Enums
{
    public enum CommandCode
    {
        REGISTER = 20,
        LOGIN,
        FINDID,
        FINDPW,
        CHATMSG,
        TOCHATMSG
    }
}
