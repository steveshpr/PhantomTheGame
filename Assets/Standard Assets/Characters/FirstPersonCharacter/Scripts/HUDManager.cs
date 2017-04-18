using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Phantom.Utility.MessageBus;
using System;

namespace Phantom.HUD
{
    public class HUDManager : MonoBehaviour, ISubscriber<HUDSetText>, ISubscriber<HUDSetIcon>
    {
        [SerializeField]private GameObject text;
        [SerializeField]private GameObject bow;
        [SerializeField]private GameObject sword;
        [SerializeField]private GameObject arrow;

        [SerializeField]private GameObject arrowCount;
        [SerializeField]private GameObject arrowArea;

        [SerializeField]private Sprite arrowOrg;
        [SerializeField]private Sprite arrowGreen;
        [SerializeField]private Sprite arrowRed;
        [SerializeField]private Sprite swordOrg;
        [SerializeField]private Sprite swordGreen;
        [SerializeField]private Sprite swordRed;
        [SerializeField]private Sprite bowOrg;
        [SerializeField]private Sprite bowGreen;
        [SerializeField]private Sprite bowRed;

        void Start()
        {
            MainBus.Instance.Subscribe(this);
        }

        // Update is called once per frame
        void Update()
        {
            arrowCount.GetComponent<Text>().text = arrowArea.GetComponent<RHDrawWeapon>().count.ToString();
        }

        public void OnEvent(HUDSetText evt)
        {
            text.GetComponent<Text>().text = evt.name;
        }

        public void OnEvent(HUDSetIcon evt)
        {
            switch (evt.name) {
                case "bow":
                    Image targetIcon = bow.GetComponent<Image>();
                    switch (evt.color)
                    {
                        case "red":
                            targetIcon.sprite = bowRed;
                    break;
                        case "org":
                            targetIcon.sprite = bowOrg;
                            break;
                        case "green":
                            targetIcon.sprite = bowGreen;
                            break;
                    }
                    break;
                case "arrow":
                    targetIcon = arrow.GetComponent<Image>();
                    switch (evt.color)
                    {
                        case "red":
                            targetIcon.sprite = arrowRed;
                            break;
                        case "org":
                            targetIcon.sprite = arrowOrg;
                            break;
                        case "green":
                            targetIcon.sprite = arrowGreen;
                            break;
                    }
                    break;
                case "sword":
                    targetIcon = sword.GetComponent<Image>();
                    switch (evt.color)
                    {
                        case "red":
                            targetIcon.sprite = swordRed;
                            break;
                        case "org":
                            targetIcon.sprite = swordOrg;
                            break;
                        case "green":
                            targetIcon.sprite = swordGreen;
                            break;
                    }
                    break;
            }
        }
    }
}
