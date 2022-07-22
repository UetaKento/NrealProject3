using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//ARFoundationとARCoreExtensions関連を使用する
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
namespace AR_Fukuoka
{
    public class SampleScript : MonoBehaviour
    {
        //GeospatialAPIを用いたトラッキング情報
        public AREarthManager EarthManager;
        //GeospatialAPIとARCoreの初期化と結果
        public VpsInitializer Initializer;
        //結果表示用のUI
        public Text OutputText;

        //方位の許容精度
        public double HeadingThreshold = 25;
        //水平位置の許容精度
        public double HorizontalThreshold = 20;

        //オブジェクトを置く緯度
        public double Latitude;
        //オブジェクトを置く経度
        public double Longitude;
        //オブジェクトを置く高さ
        public double Altitude;
        //オブジェクトの向き
        public double Heading;
        //表示オブジェクトの元データ
        public GameObject ContentPrefab;
        //実際に表示するオブジェクト
        GameObject displayObject;
        //アンカー作成に使用
        public ARAnchorManager AnchorManager;

        // Start is called before the first frame update
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {
            string status = "";
            //初期化失敗またはトラッキングができたいない場合は何もしないで戻る
            if (!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
            {
                return;
            }
            //トラッキング結果を取得
            GeospatialPose pose = EarthManager.CameraGeospatialPose;
            //トラッキング精度がthresholdより悪い(値が大きい)場合
            if (pose.HeadingAccuracy > HeadingThreshold || pose.HorizontalAccuracy > HorizontalThreshold)
            {
                status = "低精度：周辺を見回してください";
            }
            else //許容誤差いないの場合
            {
                status = "高精度：High Tracking Accuracy";
                if (displayObject == null)
                {
                    //初めて高精度計測できたタイミングでオブジェクトを生成

                    //スマホの高さ-1.5mでおよそ地面の高さとする(tentatively)
                    Altitude = pose.Altitude - 1.5f;
                    //角度の補正
                    Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);
                    //指定した位置・向きのアンカーを作成
                    ARGeospatialAnchor anchor = AnchorManager.AddAnchor(Latitude, Longitude, Altitude, quaternion);
                    //アンカーが正しく作られていればオブジェクトを実体化
                    if (anchor != null)
                    {
                        displayObject = Instantiate(ContentPrefab, anchor.transform);
                    }

                }

            }
            //結果を表示(statusはのちほど使う)
            ShowTrackingInfo(status, pose);
        }
        void ShowTrackingInfo(string status, GeospatialPose pose)
        {
            OutputText.text = string.Format(
                "Latitude/Longitude: {0}°, {1}°\n" +
                "Horizontal Accuracy: {2}m\n" +
                "Altitude: {3}m\n" +
                "Vertical Accuracy: {4}m\n" +
                "Heading: {5}°\n" +
                "Heading Accuracy: {6}°\n" +
                "{7} \n"
                ,
                pose.Latitude.ToString("F6"),  //{0}
                pose.Longitude.ToString("F6"), //{1}
                pose.HorizontalAccuracy.ToString("F6"), //{2}
                pose.Altitude.ToString("F2"),  //{3}
                pose.VerticalAccuracy.ToString("F2"),  //{4}
                pose.Heading.ToString("F1"),   //{5}
                pose.HeadingAccuracy.ToString("F1"),   //{6}
                status //{7}
                );
        }
    }
}
