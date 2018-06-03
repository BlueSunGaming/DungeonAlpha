using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class InfoBoxMono : MonoBehaviour
    {
        
        private float _timeCounter = 0;

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        private AudioSource AudioSource ;

        //TODO: Handle messages with GUI/Audio
        void Update()
        {
            _timeCounter += Time.deltaTime;

            if (InfoBox.Logs.Count > 0 && _timeCounter > 1.2f)
            {
                var entry = InfoBox.Logs[0];
                if (entry != null)
                {
                    if (entry.Type == InfoEntryType.Info)
                    {
                        Debug.Log(entry.Message);
                    }
                    else
                    {
                        Debug.Log("<color=red>" + entry.Message + "</color>");
                    }

                    if (entry.Audio != null)
                    {
                        AudioSource.PlayOneShot(entry.Audio);
                    }

                    InfoBox.Logs.Remove(entry);
                }
                _timeCounter = 0;
            }
        }
    }
}