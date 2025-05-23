using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NEXTCHATServ.Model;
using NEXTCHATServ.Network;
using NEXTCHATServ.Database;
using NEXTCHATServ.Managers;
using static System.Net.Mime.MediaTypeNames;

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
                    return HandleRegister(parser);

                case CommandCode.LOGIN:
                    // 로그인 처리
                    return HandleLogin(parser, client);

                case CommandCode.FINDID:
                    // ID 찾기 처리
                    return HandleIdSearch(parser);

                case CommandCode.CHATMSG:
                    // 채팅 처리
                    return HandleChatMessage(parser);

                default:
                    // 알 수 없는 명령 처리
                    return new byte[] { (byte)ResponseCode.IdNotFound };
            }
        }

        
        private static byte[] HandleRegister(PacketParser parser)
        {
            byte genderCodeByte = parser.ReadByte();      // 성별 (1 byte)
            byte birthMonth = parser.ReadByte(); // 예시: 5월
            byte birthDay = parser.ReadByte();  // 예시: 26일
            short birthYear = parser.ReadInt16(); // 예시: 1995년
            string userId = parser.ReadString();          // ID
            string password = parser.ReadString();        // Password                    
            string userPhone = parser.ReadString();       // 전화번호 (11자리 문자열)

            //바이트 타입인 genderCodeByte을 GenderCode로 형변환
            GenderCode gender = (GenderCode)genderCodeByte;

            //벌스 데이트에 저장
            DateTime birthDate = new((short)birthYear, birthMonth, birthDay);


            Console.WriteLine($"[회원가입 요청] ID: {userId}, PW: {password}, PN: {userPhone} 성별: {gender} 생일: {birthDate:yyyy-MM-dd}");

            // TODO: 실제 회원가입 처리 구현 필요
            bool a = UserRepository.IsUserIdExists(userId);

            if (!a)
            {
                //회원가입 로직
                User user = new User
                {
                    UserId = userId,
                    Password = password,
                    Phone = userPhone,
                    Gender = gender,
                   BirthDate = birthDate,
                };

                bool isInserted = UserRepository.InsertUser(user);

                if (!isInserted)
                {
                    //TODO: DB 오류 ENUM 추가한뒤 올려주기
                    Console.WriteLine("회원가입 실패 (DB 오류)");
                   // return new byte[] { (byte)ResponseCode.InternalError };
                }
                else
                {
                    Console.WriteLine("회원가입 성공");
                    return new byte[] { (byte)ResponseCode.Success };
                }


            }
            //실패 반환
            return new byte[] { (byte)ResponseCode.DuplicateId};

        }

        //로그인 요청 처리
        private static byte[] HandleLogin(PacketParser parser, TcpClient client)
        {
            string userId = parser.ReadString();       // ID
            string password = parser.ReadString();     // PW
            

            Console.WriteLine($"[로그인 요청] ID: {userId}, PW: {password}");

            //클라이언트 딕셔너리에 추가
            ClientManager.TryAddClient(userId, client);

            // TODO: 로그인 검증 로직 필요
            return new byte[] { (byte)ResponseCode.Success };
        }

        
        //아이디 검색 처리
        private static byte[] HandleIdSearch(PacketParser parser)
        {
            string userId = parser.ReadString();       // ID

            Console.WriteLine($"[ID 검색 요청] ID: {userId}");

            // TODO: ID 존재 여부 확인 로직 추가 필요
            return new byte[] { (byte)ResponseCode.Success }; // 또는 "IdNotFound"
        }


        //나중에 시간 추가하기
        //채팅 요청 처리
        private static byte[] HandleChatMessage(PacketParser parser)
        {
            string userId = parser.ReadString();    // 보낸 사람 ID
            string message = parser.ReadString();   // 채팅 내용

            ChatMessage chatMsg = new ChatMessage
            {
                FromUserId = userId,
                Content = message,
                Timestamp = DateTime.Now
            };

            //목표 chatMsg를 DB에 저장후 프로토콜에 맞춰서 바이트코드로 변환한뒤 클라이언트 매니져에있는 클라이언트 딕셔너리에서 자신의 아이디를 제외한후 브로드 캐스트 하기
            //1. UserRepository.SaveChatMessage(chatMsg); DB 저장완료
            //2. public static byte[] BuildChatPacket(ChatMessage chatMsg) 하면 프로토콜에 맞춰서 바이트 코드로 변환
            //3. 바이트코드와, userId를 매개변수로 삼는 브로드 캐스트 함수가 필요
            //4. 락을 걸어야 하기 떄문에 클라이언트 매니져에서 딕셔너리 접근할것 나를 제외한 아이디와 클라이언트 정보를 반환하는 로직을 어디에? 클라이언트 매니져에 생성하기
            UserRepository.SaveChatMessage(chatMsg);
            ChatBroadcaster.BroadcastChat(chatMsg, userId);

            return new byte[] { (byte)ResponseCode.Success }; // 단순히 수신 성공 응답
        }
    }
}
