using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phantom.Utility.MessageBus;
using System;
using UnityEngine.SceneManagement;

namespace Phantom.Enviroment
{
    public class EnviromentSettings : MonoBehaviour, ISubscriber<SpottedEvent>, ISubscriber<LostSightEvent>
    {
        public bool instantLose;

        private bool spotted = false;

        

        public void Start() {
            MainBus.Instance.Subscribe(this);
        }

        public void OnGUI()
        {
            if (spotted)
            {
                if (instantLose)
                {
                    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "You or a dead body is spotted by the enemy!\nRestarting game in 5 secons!");
                    StartCoroutine("ExecuteAfter", 5f);
                }
                else {
                    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "You or a dead body is spotted by the enemy!\nRUN!!!");
                }
            }
        }

        private IEnumerator ExecuteAfter(float time)
        {
            yield return new WaitForSeconds(time);

            SceneManager.LoadScene("Demo");

        }

        public void OnEvent(SpottedEvent evt)
        {
            if (!spotted)
            {
                spotted = true;
            }
        }

        public void OnEvent(LostSightEvent evt)
        {
            if (!instantLose) {
                spotted = false;
            }
        }
    }
    
}
