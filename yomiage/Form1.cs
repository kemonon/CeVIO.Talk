using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CeVIO.Talk.RemoteService;

namespace yomiage
{
    class MyClass
    {
        Talker talker;

        public MyClass()
        {
            // 【CeVIO Creative Studio】起動
            ServiceControl.StartHost(false);
            
        }

        ~MyClass()
        {
            // 【CeVIO Creative Studio】終了
            ServiceControl.CloseHost();
        }

        public void SetCast(string cast)
        {
            // Talkerインスタンス生成
            talker = new Talker();

            // キャスト設定
            talker.Cast = cast;
        }

        public void SetParmetor(string text,uint vol=100,uint tonescale=50)
        {

            talker.Volume = vol;// （例）音量設定

            // 抑揚を使う場合のバージョンによる処理分け例
            try
            {
                // HostVersionが取得できるのは4.0.7.0以降。
                //string version = CeVIO.Talk.RemoteService.ServiceControl.HostVersion;

                talker.ToneScale = tonescale;// （例）抑揚設定

            }
            catch (Exception ex)
            {
                // 存在しないプロパティにアクセスするとRemotingExceptionが発生
            }

            // （例）再生

            SpeakingState state = talker.Speak(text);

            state.Wait();
            talker.OutputWaveToFile(text, "C:\\Users\\tanak\\Documents\\CeVIO\\Audio\\SnappedTracks\\" +talker.Cast + "_" + text + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + "_" + DateTime.Now.Hour.ToString("00") + "_"+DateTime.Now.Minute.ToString("00") + "_" + DateTime.Now.Second.ToString("00")+".wav");


            // （例）音素データをトレース出力
/*
            PhonemeData[] phonemes = talker.GetPhonemes("はじめまして");

            foreach (var phoneme in phonemes)

            {

                System.Diagnostics.Trace.WriteLine("" + phoneme.Phoneme + " " + phoneme.StartTime + " " + phoneme.EndTime);

            }
*/
        }


    }

    public partial class Form1 : Form
    {
        


        public Form1()
        {
            InitializeComponent();
            
        }

        async public void aa()
        {

            MyClass a = new MyClass();
            //a.SetCast("さとうささら");
            a.SetCast("IA");
            //a.SetParmetor("読み上げ開始します");

            //初期化
            var contSpeechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            await contSpeechRecognizer.CompileConstraintsAsync();

            //認識中の処理定義
            contSpeechRecognizer.HypothesisGenerated += (s, ee) =>
            {
                //a.SetCast("さとうささら");
                //a.SetParmetor(ee.Hypothesis.Text);
                  
            };

            contSpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += (s, ee) =>//認識完了後に実行されるラムダ関数 入力があった場合
            {
                a.SetParmetor(ee.Result.Text,50,50);//テキスト読み上げ
            };

            contSpeechRecognizer.ContinuousRecognitionSession.Completed += (s, ee) =>//認識完了後に実行されるラムダ関数 タイムアウト時？
            {
                aa();
            };

            //認識開始
            await contSpeechRecognizer.ContinuousRecognitionSession.StartAsync();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            aa();
        }
    }
}
