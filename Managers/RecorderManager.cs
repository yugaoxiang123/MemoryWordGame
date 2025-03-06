//using UnityEditor.Recorder;
//using UnityEngine;
//using System.IO;
//using System.Reflection; // 添加反射命名空间

//public class RecorderManager : Singleton<RecorderManager>
//{
//    internal RecorderController recorderController;
//    internal bool isRecording = false;
//    private RecorderControllerSettings settings;

//    public void Initialized()
//    {
//        settings = new RecorderControllerSettings();
//        recorderController = new RecorderController(settings);
//        settings.SetRecordModeToManual();
//    }

//    public void StartRecording()
//    {
//        if (isRecording || recorderController.IsRecording())
//        {
//            Debug.LogWarning("Already recording or previous recording not stopped.");
//            recorderController.StopRecording();
//            isRecording = false;
//        }

//        // 使用反射清空 m_RecorderSettings
//        var fieldInfo = typeof(RecorderControllerSettings).GetField("m_RecorderSettings", BindingFlags.NonPublic | BindingFlags.Instance);
//        if (fieldInfo != null)
//        {
//            var recorderSettingsList = fieldInfo.GetValue(settings) as System.Collections.IList;
//            recorderSettingsList?.Clear();
//        }

//        // 生成唯一文件名
//        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
//        string directory = "D:\\ygx\\Recordings";
//        if (!Directory.Exists(directory))
//        {
//            Directory.CreateDirectory(directory);
//        }
//        string outputFile = $"{directory}\\WordRecording_{timestamp}";

//        // 创建新的录制设置
//        var videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
//        videoRecorder.OutputFile = outputFile;
//        videoRecorder.FrameRate = 30;
//        settings.AddRecorderSettings(videoRecorder);

//        // 开始录制
//        recorderController.PrepareRecording();
//        recorderController.StartRecording();
//        isRecording = true;
//        Debug.Log($"开始录屏，保存到 {outputFile}");
//    }

//    public void StopRecording()
//    {
//        if (!isRecording)
//        {
//            Debug.LogWarning("Not recording.");
//            return;
//        }

//        recorderController.StopRecording();
//        isRecording = false;
//        Debug.Log("停止录屏");
//    }
//}