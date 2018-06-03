using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker.Generic
{
    /// <summary>
    /// Shows FPS
    /// 
    /// Attach to any gameobject and assign a Unity.UI.Text object to this.
    /// </summary>
    public class ShowFps : MonoBehaviour
    {
        public Text FpsLabel;

        private const float UpdateInterval = 0.1F;
        private float _accum; // FPS accumulated over the interval
        private int _frames; // Frames drawn over the interval
        private float _timeleft; // Left time for current interval

        void Start()
        {
            _timeleft = UpdateInterval;
        }

        void Update()
        {
            if (!FpsLabel.gameObject.activeInHierarchy) return;

            _timeleft -= Time.deltaTime;
            _accum += Time.timeScale / Time.deltaTime;
            ++_frames;

            // Interval ended - update GUI text and start new interval
            if (!(_timeleft <= 0.0)) return;

            // display two fractional digits (f2 format)
            var fps = _accum / _frames;
            var format = System.String.Format("{0}", (int)fps);
            FpsLabel.text = format;

            if (fps < 30)
                FpsLabel.color = Color.yellow;
            else
                if (fps < 10)
                    FpsLabel.color = Color.red;
                else
                    FpsLabel.color = Color.green;

            _timeleft = UpdateInterval;
            _accum = 0.0F;
            _frames = 0;
        }
    }
}