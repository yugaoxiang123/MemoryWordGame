//using UnityEditor.Recorder;
//using UnityEngine;
//using System.IO;
//using System.Reflection; // ��ӷ��������ռ�

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

//        // ʹ�÷������ m_RecorderSettings
//        var fieldInfo = typeof(RecorderControllerSettings).GetField("m_RecorderSettings", BindingFlags.NonPublic | BindingFlags.Instance);
//        if (fieldInfo != null)
//        {
//            var recorderSettingsList = fieldInfo.GetValue(settings) as System.Collections.IList;
//            recorderSettingsList?.Clear();
//        }

//        // ����Ψһ�ļ���
//        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
//        string directory = "D:\\ygx\\Recordings";
//        if (!Directory.Exists(directory))
//        {
//            Directory.CreateDirectory(directory);
//        }
//        string outputFile = $"{directory}\\WordRecording_{timestamp}";

//        // �����µ�¼������
//        var videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
//        videoRecorder.OutputFile = outputFile;
//        videoRecorder.FrameRate = 30;
//        settings.AddRecorderSettings(videoRecorder);

//        // ��ʼ¼��
//        recorderController.PrepareRecording();
//        recorderController.StartRecording();
//        isRecording = true;
//        Debug.Log($"��ʼ¼�������浽 {outputFile}");
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
//        Debug.Log("ֹͣ¼��");
//    }
//}