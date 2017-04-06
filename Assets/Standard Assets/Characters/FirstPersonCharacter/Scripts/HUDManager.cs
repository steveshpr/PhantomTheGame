using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Phantom.Utility.MessageBus;
using System;

namespace Phantom.HUD
{
    public class HUDManager : MonoBehaviour, ISubscriber<HUDSetText>
    {
        [SerializeField]private GameObject text;

        void Start()
        {
            MainBus.Instance.Subscribe(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEvent(HUDSetText evt)
        {
            text.GetComponent<Text>().text = evt.name;
        }
    }
}
