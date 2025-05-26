using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEXTCHATServ.Model
{
    public enum ResponseCode
    {
        Success = 30,
        DuplicateId,         // 이미 존재하는 ID
        WeakPassword,         // 안전하지 않은 암호
        IdNotFound,           // 존재하지 않은 ID
        PasswordMismatch      // 비밀번호 인증 실패
    }

}