using System;
using Baku.Websocket.Client;
using SuperSocket.ClientEngine;

namespace Baku.LibqiDotNet.WebSocketImpl
{
    /// <summary>WebSocket4Net�ɂ��WebSocket�N���C�A���g�̎�����\���܂��B</summary>
    public class WebSocket : IWebSocket
    {
        //default constructor is enough
        //public WebSocket() { }

        private WebSocket4Net.WebSocket _ws;

        /// <summary>�M������Ă��Ȃ��ڑ���������邩���擾�A�ݒ肵�܂��B</summary>
        public bool AllowUnstrustedCertificate
        {
            get { return _ws?.AllowUnstrustedCertificate ?? false; }
            set
            {
                if (_ws != null)
                {
                    _ws.AllowUnstrustedCertificate = value;
                }
            }
        }

        /// <summary>������ping�𑗐M���邩���擾�A�ݒ肵�܂��B</summary>
        public bool EnableAutoSendPing
        {
            get { return _ws?.EnableAutoSendPing ?? false; }
            set
            {
                if (_ws != null)
                {
                    _ws.EnableAutoSendPing = value;
                }
            }
        }

        //NOTE: �񋓌^�̐��l��WebSocket4Net�̂��̂ƍ��킹�Ă���̂ŃR���ōςނƂ������Z
        /// <summary>WebSocket�̌��݂̐ڑ���Ԃ��擾���܂��B</summary>
        public WebSocketState State
            => (WebSocketState)((int)(_ws?.State ?? WebSocket4Net.WebSocketState.None));

        /// <summary>�ڑ����m�������Ɣ������܂��B</summary>
        public event EventHandler Opened;
        /// <summary>���b�Z�[�W����M����Ɣ������܂��B</summary>
        public event EventHandler Closed;
        /// <summary>�ʐM�ɃG���[��������Ɣ������܂��B</summary>
        public event EventHandler<WebSocketErrorEventArgs> Error;
        /// <summary>�ڑ����ؒf����Ɣ������܂��B</summary>
        public event EventHandler<WebSocketMessageReceivedEventArgs> MessageReceived;

        /// <summary>�ڑ����URL���w�肵�ď��������܂��B</summary>
        /// <param name="url">�ڑ���URL</param>
        public void InitializeUrl(string url)
        {
            _ws = new WebSocket4Net.WebSocket(url, "", WebSocket4Net.WebSocketVersion.Rfc6455);
            SubscribeWebSocket();
        }

        /// <summary>���������Ɏw�肵��URL�֐ڑ����܂��B</summary>
        public void Open()
        {
            if (_ws == null)
            {
                throw new InvalidOperationException("Websocket is not initialized");
            }
            _ws.Open();
        }

        /// <summary>���b�Z�[�W�𑗐M���܂��B</summary>
        /// <param name="msg">���M���郁�b�Z�[�W</param>
        public void Send(string msg)
        {
            if (_ws == null)
            {
                throw new InvalidOperationException("Websocket is not initialized");
            }
            _ws.Send(msg);
        }

        /// <summary>�ڑ���ؒf���܂��B</summary>
        public void Close()
        {
            if (_ws != null)
            {
                _ws.Close();
                UnsubscribeWebSocket();
                _ws = null;
            }
        }

        private void SubscribeWebSocket()
        {
            _ws.Opened += OnWebSocketOpened;
            _ws.Closed += OnWebSocketClosed;
            _ws.Error += OnWebSocketError;
            _ws.MessageReceived += OnWebSocketMessageReceived;
        }

        private void UnsubscribeWebSocket()
        {
            _ws.Opened -= OnWebSocketOpened;
            _ws.Closed -= OnWebSocketClosed;
            _ws.Error -= OnWebSocketError;
            _ws.MessageReceived -= OnWebSocketMessageReceived;
        }

        private void OnWebSocketMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
            => MessageReceived?.Invoke(this, new WebSocketMessageReceivedEventArgs(e.Message));

        private void OnWebSocketError(object sender, ErrorEventArgs e)
            => Error?.Invoke(this, new WebSocketErrorEventArgs(e.Exception));

        private void OnWebSocketOpened(object sender, EventArgs e)
            => Opened?.Invoke(this, e);

        private void OnWebSocketClosed(object sender, EventArgs e)
            => Closed?.Invoke(this, e);

    }
}