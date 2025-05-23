using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NEXTCHATServ.Core;

namespace NEXTCHATServ
{
    class Program
    {
        // TCP 서버 리스너를 위한 변수 선언
        static TcpListener server;

        // 연결된 클라이언트를 저장할 딕셔너리 (클라이언트 식별자, TcpClient 객체)
        static Dictionary<string, TcpClient> connectedClients = new Dictionary<string, TcpClient>();

        static void Main(string[] args)
        {
            // 사용할 포트 번호 설정
            int port = 9000;

            // 서버를 지정한 포트에서 모든 IP로부터의 연결을 수신하도록 설정
            server = new TcpListener(IPAddress.Any, port);
            server.Start(); // 서버 시작

            Console.WriteLine($"서버 시작됨. 포트: {port}");

            // 클라이언트의 연결을 무한히 기다림
            while (true)
            {
                // 클라이언트 연결 수락 (블로킹 호출)
                TcpClient client = server.AcceptTcpClient();

                // 각 클라이언트 처리를 위한 쓰레드 생성 및 시작
                Thread clientThread = new Thread(() => ClientHandler.HandleClient(client));
                clientThread.Start();
            }
        }
    }
}