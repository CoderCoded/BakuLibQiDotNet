using System;
using SocketIOClient.Messages;
using Newtonsoft.Json.Linq;
using Baku.Websocket.Client;

namespace SocketIOClient
{
    /// <summary>C# Socket.IO client interface</summary>
    public interface IClient
	{
        /// <summary>�ʐM���m������Ɣ������܂��B</summary>
		event EventHandler Opened;
        /// <summary>���b�Z�[�W����M����Ɣ������܂��B</summary>
		event EventHandler<MessageEventArgs> ReceivedMessage;
        /// <summary>�ʐM���ؒf����Ɣ������܂��B</summary>
		event EventHandler Closed;
        /// <summary>�ʐM���ɃG���[���N����Ɣ������܂��B</summary>
		event EventHandler<ErrorEventArgs> Error;

        /// <summary>�n���h�V�F�[�N�����ݒ���擾���܂��A</summary>
		SocketIOHandshake HandShake { get; }
        /// <summary>�ڑ��ς݂��ǂ������擾���܂��B</summary>
		bool IsConnected { get; }
        /// <summary>�ڑ����̃p�X���擾�A�ݒ肵�܂��B</summary>
        string Path { get; set; }

        /// <summary>URL���w�肵�Đڑ����܂��B</summary>
        /// <param name="url">�ڑ���URL</param>
        /// <param name="ws">�ڑ��̌��ƂȂ�WebSocket�̎���</param>
		void Connect(string url, IWebSocket ws);

        /// <summary>�ڑ���ؒf���܂��B</summary>
		void Close();
        /// <summary>�ڑ���ؒf���A���\�[�X��������܂��B</summary>
		void Dispose();

        /// <summary>�C�x���g�������̃n���h��������o�^���܂��B</summary>
        /// <param name="eventName">�C�x���g��</param>
        /// <param name="action">�R�[���o�b�N����</param>
		void On(string eventName, Action<IMessage> action);
        /// <summary>�C�x���g�������̃n���h��������o�^���܂��B</summary>
        /// <param name="eventName">�C�x���g��</param>
        /// <param name="endPoint">�G���h�|�C���g</param>
        /// <param name="action">�R�[���o�b�N����</param>
		void On(string eventName, string endPoint, Action<IMessage> action);

        /// <summary>�C�x���g�𔭐������܂��B</summary>
        /// <param name="eventName">�C�x���g��</param>
        /// <param name="payload">�C�x���g�ɕt��������f�[�^</param>
		void Emit(string eventName, object payload);
        /// <summary>�C�x���g�𔭐������܂��B</summary>
        /// <param name="eventName">�C�x���g��</param>
        /// <param name="payload">�C�x���g�ɕt��������f�[�^</param>
        /// <param name="endPoint">�G���h�|�C���g</param>
        /// <param name="callBack">�R�[���o�b�N����</param>
		void Emit(string eventName, object payload, string endPoint, Action<JToken> callBack);

		/// <summary>���b�Z�[�W�𑗐M���܂��B</summary>
        /// <param name="msg">���b�Z�[�W</param>
		void Send(IMessage msg);

	}
}
