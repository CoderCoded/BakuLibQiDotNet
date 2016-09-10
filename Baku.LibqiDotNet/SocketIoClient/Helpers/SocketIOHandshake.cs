using System;
using System.Collections.Generic;

namespace SocketIOClient
{
    /// <summary>�ʐM�J�n���̃n���h�V�F�[�N�����ݒ��\���܂��B</summary>
    public class SocketIOHandshake
    {
        /// <summary>����̏�ԂŃn���h�V�F�[�N�C���X�^���X�����������܂��B</summary>
        public SocketIOHandshake()
        {
            //do nothing
        }

        /// <summary>�ڑ�ID���擾�A�ݒ肵�܂��B</summary>
        public string SID { get; set; }

        /// <summary>�ڑ��^�C���A�E�g���擾�A�ݒ肵�܂��B</summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>Heart beat�̃^�C���A�E�g���擾�A�ݒ肵�܂��B</summary>
        public int HeartbeatTimeout { get; set; }

        /// <summary>The Interval will be approxamately 20% faster than the Socket.IO service indicated was required</summary>
        public TimeSpan HeartbeatInterval => new TimeSpan(0, 0, HeartbeatTimeout);

        /// <summary>HTTP���X�|���X������̈ꕔ���擾���܂��B</summary>
        public List<string> Transports { get; } = new List<string>();

        /// <summary>�G���[���b�Z�[�W���擾�A�ݒ肵�܂��B</summary>
        public string ErrorMessage { get; set; }

        /// <summary>�G���[�̗L�����擾���܂��B</summary>
        public bool HadError => !string.IsNullOrEmpty(ErrorMessage);
        /// <summary>HTTP�̃��X�|���X�����񂩂�n���h�V�F�[�N�C���X�^���X�𐶐����܂��B</summary>
        /// <param name="value">���X�|���X������</param>
        /// <returns>���X�|���X�Ɋ�Â��ď��������ꂽ�n���h�V�F�[�N�C���X�^���X</returns>
        public static SocketIOHandshake LoadFromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            string[] items = value.Split(':');
            if (items.Length != 4)
            {
                return null;
            }

            var result = new SocketIOHandshake();
            int hb = 0;
            int ct = 0;
            result.SID = items[0];

            if (int.TryParse(items[1], out hb))
            { 
                // setup client time to occure 25% faster than needed
                var pct = (int)(hb * .75);  
                result.HeartbeatTimeout = pct;
            }
            if (int.TryParse(items[2], out ct))
            {
                result.ConnectionTimeout = ct;
            }
            result.Transports.AddRange(items[3].Split(','));
            return result;
        }
    }
}
