using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NEXTCHATServ.Managers
{
    public static class ClientManager
    {
        // 채팅방에 접속한 클라이언트 전용 딕셔너리
        private static readonly Dictionary<string, TcpClient> connectedClients = new Dictionary<string, TcpClient>();

        //readonly 선언 시 또는 생성자에서만 값을 한 번 설정할 수 있고, 그 후에는 변경 불가능하다
        private static readonly object lockObj = new();

        // 클라이언트 추가 (중복 로그인 방지)
        public static bool TryAddClient(string username, TcpClient client)
        {
            lock (lockObj)
            {
                if (!connectedClients.ContainsKey(username))
                {
                    connectedClients.Add(username, client);
                    Console.WriteLine($"{username} 접속됨");
                    return true;
                }

                Console.WriteLine($"{username} 이미 접속 중");
                return false;
            }
        }

        // 클라이언트 제거 나중에 관리자 권한으로 주기
        public static bool TryRemoveClient(string username)
        {
            lock (lockObj)
            {
                if (connectedClients.Remove(username))
                {
                    Console.WriteLine($"{username} 연결 해제됨");
                    return true;
                }

                return false;
            }
        }

        //Client 정보로 userID 찾아서 제거 (오버로드)
        public static bool TryRemoveClient(TcpClient client)
        {
            lock (lockObj)
            {
                foreach (var kvp in connectedClients)
                {
                    if (kvp.Value == client)
                    {
                        connectedClients.Remove(kvp.Key);
                        Console.WriteLine($"{kvp.Key} 연결 해제됨");
                        return true;
                    }
                }

                return false;
            }
        }



        // 현재 접속 중인 사용자 목록 조회
        public static List<string> GetAllUsernames()
        {
            lock (lockObj)
            {
                return new List<string>(connectedClients.Keys);
            }
        }


        // 나(senderId)를 제외한 (userId, TcpClient) 목록 반환
        public static List<(string userId, TcpClient client)> GetAllClientsExcept(string senderId)
        {
            List<(string userId, TcpClient client)> result = new List<(string, TcpClient)>();

            lock (lockObj)
            {
                foreach (var kvp in connectedClients)
                {
                    string userId = kvp.Key;
                    TcpClient client = kvp.Value;

                    if (userId != senderId)
                    {
                        result.Add((userId, client));
                    }
                }
            }

            return result;
        }
    }
}
