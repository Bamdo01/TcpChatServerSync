using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpChatServerSync.Managers;
using TcpChatServerSync.Model;
using TcpChatServerSync.Network;


namespace TcpChatServerSync.Core
{
    public static class ChatBroadcaster
    {
        public static void BroadcastChat(ChatMessage chatMsg, string senderId)
        {
            byte[] packet = PacketBuilder.BuildChatPacket(chatMsg);

            // 자신을 제외한 모든 클라이언트 정보 가져옴
            List<(string userId, TcpClient client)> others = ClientManager.GetAllClientsExcept(senderId);

            foreach ((string userId, TcpClient client) in others)
            {
                try
                {
                    client.GetStream().Write(packet, 0, packet.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{userId}에게 메시지 전송 실패] {ex.Message}");
                }
            }
        }
    }
}
