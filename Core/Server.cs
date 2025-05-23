using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NEXTCHATServ.Core
{
    public static class Server
    {
        // TCP 리스너 객체 (서버 역할)
        private static TcpListener listener;

        public static void Start(int port)
        {
            // 리스너 생성 및 시작 (모든 IP에서 포트로 접속 수락)
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"[서버] 시작됨 - 포트: {port}");

            // 클라이언트 연결을 무한히 대기
            while (true)
            {
                // 클라이언트 접속 수락 (blocking)
                TcpClient client = listener.AcceptTcpClient();

                // 클라이언트 처리를 위한 쓰레드 생성 및 시작
                Thread clientThread = new Thread(() => ClientHandler.HandleClient(client));
                clientThread.Start();
            }
        }
    }
}
