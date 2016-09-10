using Baku.Websocket.Client;
using Baku.LibqiDotNet.WebSocketImpl;

namespace Baku.LibqiDotNet
{
    /// <summary>���ۂ�WebSocket�������������ꂽQiSession�̃t�@�N�g����\���܂��B</summary>
    public class QiSessionFactory : QiSessionFactoryBase
    {
        /// <summary>���ۂ�WebSocket�������擾���܂��B</summary>
        /// <returns>�������ꂽWebSocket</returns>
        protected override IWebSocket GetWebSocket()
            => new WebSocket();
    }

}
