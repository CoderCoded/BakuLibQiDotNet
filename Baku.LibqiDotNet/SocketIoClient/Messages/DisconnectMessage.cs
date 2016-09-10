
namespace SocketIOClient.Messages
{
    /// <summary>
    /// Signals disconnection. If no endpoint is specified, disconnects the entire socket.
    /// </summary>
    /// <remarks>Disconnect a socket connected to the /test endpoint:
    ///		0::/test
    /// </remarks>
    public class DisconnectMessage : Message
	{
        /// <summary>�C�x���g�����擾���܂��B</summary>
        public override string Event => "disconnect";

        /// <summary>����̐ݒ�ŃC���X�^���X�����������܂��B</summary>
		public DisconnectMessage() : base()
		{
            MessageType = SocketIOMessageTypes.Disconnect;
		}
        /// <summary>�G���h�|�C���g���w�肵�ăC���X�^���X�����������܂��B</summary>
        /// <param name="endPoint">�G���h�|�C���g</param>
		public DisconnectMessage(string endPoint) : this()
		{
            Endpoint = endPoint;
		}

        /// <summary>��M�������b�Z�[�W����Ή�����ؒf���b�Z�[�W���擾���܂��B</summary>
        /// <param name="rawMessage">��M����socket.io�v���g�R�������ȃ��b�Z�[�W</param>
        /// <returns>�Ή�����ؒf���b�Z�[�W</returns>
		public static DisconnectMessage Deserialize(string rawMessage)
		{
            //  0::
            //  0::/test
            var msg = new DisconnectMessage() { RawMessage = rawMessage };

			string[] args = rawMessage.Split(SPLITCHARS, 3);
			if (args.Length == 3 && !string.IsNullOrEmpty(args[2]))
            {
                msg.Endpoint = args[2];
            }
			return msg;
		}

        /// <summary>�ؒf���b�Z�[�W���擾���܂��B</summary>
        public override string Encoded => $"0::{Endpoint}";

    }
}
