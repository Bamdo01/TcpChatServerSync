using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NEXTCHATServ.Model;
using NEXTCHATServ.Network;

namespace NEXTCHATServ.Core
{
    /// <summary>
    /// 클라이언트로부터 수신된 명령을 처리하는 클래스
    /// </summary>
    public static class CommandProcessor
    {
        /// <summary>
        /// 수신된 바이트 데이터를 해석하고 명령에 따라 적절히 처리
        /// </summary>
        public static byte[] ProcessCommand(byte[] buffer, TcpClient client)
        {
            // 버퍼 파싱 객체 생성
            PacketParser parser = new PacketParser(buffer);

            // 명령 코드 읽기 (첫 번째 바이트)
            byte commandCode = parser.ReadByte();

            // 명령어 enum으로 캐스팅
            CommandCode command = (CommandCode)commandCode;

            // 명령 코드에 따라 분기 처리
            switch (command)
            {
                case CommandCode.REGISTER:
                    // 회원가입 처리
                    return ConvertResponseCodeToBytes(HandleRegister(parser));

                case CommandCode.LOGIN:
                    // 로그인 처리
                    return ConvertResponseCodeToBytes(HandleLogin(parser));

                case CommandCode.FINDID:
                    // ID 찾기 처리
                    return ConvertResponseCodeToBytes(HandleIdSearch(parser));

                default:
                    // 알 수 없는 명령 처리
                    return ConvertResponseCodeToBytes(ResponseCode.IdNotFound);
            }
        }

        /// <summary>
        /// 0x20 - 회원가입 요청 처리
        /// </summary>
        private static ResponseCode HandleRegister(PacketParser parser)
        {
            string userId = parser.ReadString();       // ID
            string password = parser.ReadString();     // PW
            string userPn = parser.ReadString();       // 전화번호

            Console.WriteLine($"[회원가입 요청] ID: {userId}, PW: {password}, PN: {userPn}");

            // TODO: 실제 회원가입 처리 구현 필요

            User user = new User
            {
                UserId = userId,
                Password = password,
                Phone = userPn
            };

            //UserRepository.Users[userId] = user;

            return ResponseCode.Success;
        }

        /// <summary>
        /// 0x21 - 로그인 요청 처리
        /// </summary>
        private static ResponseCode HandleLogin(PacketParser parser)
        {
            string userId = parser.ReadString();       // ID
            string password = parser.ReadString();     // PW



            Console.WriteLine($"[로그인 요청] ID: {userId}, PW: {password}");

            // TODO: 로그인 검증 로직 필요
            return ResponseCode.Success;
        }

        /// <summary>
        /// 0x22 - ID 검색 요청 처리
        /// </summary>
        private static ResponseCode HandleIdSearch(PacketParser parser)
        {
            string userId = parser.ReadString();       // ID

            Console.WriteLine($"[ID 검색 요청] ID: {userId}");

            // TODO: ID 존재 여부 확인 로직 추가 필요
            return ResponseCode.Success; // 또는 "IdNotFound"
        }
    }
}
