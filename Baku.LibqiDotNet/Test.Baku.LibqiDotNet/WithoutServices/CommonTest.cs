﻿using Baku.LibqiDotNet;
using Baku.LibqiDotNet.Path;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Test.Baku.LibqiDotNet
{
    public abstract class CommonTest
    {
        protected abstract QiSession GetConnectedSession();

        protected QiSession Session { get; private set; }

        public virtual void SetUpConnection()
        {
            PathModifier.AddEnvironmentPath("dlls", PathModifyMode.RelativeToExecutingAssembly);
            Session = GetConnectedSession();
            //接続でコケたらテストに進まない(当たり前)
            Assert.IsTrue(Session.IsConnected);
        }

        public virtual void CloseAllConnection()
        {
            Session?.Close();
        }

        public virtual void TestMotionSummary()
        {
            var motion = Session.GetService("ALMotion");
            string summary = motion["getSummary"].Call<string>();

            //"summary" must contain multi-line information string (off course NOT empty)
            Assert.IsTrue(summary.Split('\n').Length > 1);
        }

        public virtual void TestArrayResult()
        {
            double[] angles = Session.GetService("ALMotion")["getAngles"].Call<double[]>("Body", true);
            if (angles.Length == 0)
            {
                Assert.Fail("Received zero-length float[] for robot angles.");
            }
            //check if value is not default.
            for (int i = 0; i < angles.Length; i++)
            {
                if (angles[i] != 0)
                {
                    Assert.Pass("Success, non-zero value was returned as robot's angle");
                }
            }
        }

        public virtual void TestDialogEvents()
        {
            Assert.IsTrue(DialogEvent.TryDetectSignalEvent(Session));
        }

        public virtual void TestMotionPose()
        {
            var motion = Session.GetService("ALMotion");
            var posture = Session.GetService("ALRobotPosture");

            motion["wakeUp"].Call();
            posture["goToPosture"].Call("StandInit", 0.5f);
        }

        public virtual void TestMemoryData()
        {
            var memory = Session.GetService("ALMemory");

            //memory data functionality
            string memKey = "MyApplication/MyData";
            int testInput = 12345;

            //CRAD-like
            memory["insertData"].Call(memKey, testInput);
            int testOutput = memory["getData"].Call<int>(memKey);
            memory["removeData"].Call(memKey);

            Assert.AreEqual(testInput, testOutput);
        }

        public virtual void TestMemoryEvent()
        {
            int testInput = 42;
            string eventName = "MyApplication/MyEvent";

            //be true if event handler was called successfully
            bool isHandlerCorrectlyCalled = false;
            Action<IQiResult> onReceiveSignal = res =>
            {
                if (res.Count > 0 && res[0].ToInt32() == testInput)
                {
                    isHandlerCorrectlyCalled = true;
                }
            };

            var memory = Session.GetService("ALMemory");

            IQiSignal signal = memory["subscriber"].Call<IQiSignal>(eventName);

            signal.Connect(onReceiveSignal);
            memory["raiseEvent"].Call(eventName, testInput);

            //イベント発生から受信までのタイムラグがあるので待ってあげないとダメ
            Thread.Sleep(100);

            signal.Disconnect(onReceiveSignal);
            memory["removeEvent"].Call(eventName);

            Assert.IsTrue(isHandlerCorrectlyCalled);
        }

        public virtual void TestMemoryKeyList()
        {
            var memory = Session.GetService("ALMemory");

            string[] memoryDataKeys = memory["getDataListName"].Call<string[]>();
            Assert.IsTrue(memoryDataKeys.Length > 0);

            string[] memoryEventKeys = memory["getEventList"].Call<string[]>();
            Assert.IsTrue(memoryEventKeys.Length > 0);
        }

        public virtual void TestSensors()
        {
            Assert.Fail("Tread as fail, because simulator does not support sensor data I/O");
            Sensors.GetIMUData(Session);
            Sensors.GetSonarData(Session);
            Assert.Pass("Sensor data download function ends with no exceptions");
        }

        public virtual void TestHelloWorld()
        {
            Session.GetService("ALTextToSpeech")["say"].Call("Hello, World");
        }

        public virtual void TestBinaryOutput()
        {
            var vd = Session.GetService("ALVideoDevice");

            //忌々しいマジックナンバーを使っているが、パラメタについては
            //ALVideoDevice::subscribeのドキュメンテーションに載っているので参照されたく。
            //http://doc.aldebaran.com/2-1/naoqi/vision/alvideodevice-api.html?highlight=alvideodevice#ALVideoDeviceProxy::subscribeCamera__ssCR.iCR.iCR.iCR.iCR
            string idName = vd["subscribeCamera"].Call<string>("mytestimage",
                //カメラ種類 0:正面, 1:下方, 2:深度
                0,
                //解像度 1:320x240
                1,
                //色空間 11が24bit RGBなので基本コレでいいがYUVの方が速度速そうみたいな話もあるので注意
                11,
                //FPS: 1から30くらいまで
                5
                );

            //画像がデータバッファに溜まるのを確実に待機
            Thread.Sleep(500);

            try
            {
                byte[] res = vd["getImageRemote"].Call<IQiResult>(idName)[6].ToBytes();
                Assert.IsTrue(res.Length == 320 * 240 * 3);
            }
            finally
            {
                vd["unsubscribe"].Call(idName);
            }

        }

        public virtual void TestNotFoundMethod()
        {
            try
            {
                Session.GetService("ALTextToSpeech")["SomeNotExistNameFunction"].Call();
            }
            catch (KeyNotFoundException)
            {
                Assert.Pass("success to detect an invalid method name");
            }

            Assert.Fail("could not detect exception correctly");
        }

        public virtual void TestComplicatedArg()
        {
            var motion = Session.GetService("ALMotion");

            motion["wakeUp"].Call();

            var jointNames = new string[] { "HeadYaw", "HeadPitch" };
            var angles = new float[][]
            {
                new float[] { (float)(50.0 * Math.PI / 180.0), 0 },
                new float[] { (float)(-30.0 * Math.PI / 180.0), (float)(30 * Math.PI / 180.0), 0 }
            };

            var times = new float[][]
            {
                new float[] { 1, 2 },
                new float[] { 1, 2, 3 }
            };

            bool isAbsolute = true;

            motion["angleInterpolation"].Call(jointNames, angles, times, isAbsolute);
        }

    }
}