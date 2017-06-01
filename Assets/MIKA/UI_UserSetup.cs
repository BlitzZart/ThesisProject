using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MIKA {
    public class UI_UserSetup : MonoBehaviour {
        public Text u1Status, u2Status;
        public Text u1Input, u2Input;

        private UserManager userManager;

        private string onText = "online";
        private string offText = "offline";
        private Color onColor = Color.green;
        private Color offColor = Color.white;

        void Start() {
            userManager = UserManager.Instance;
        }
        void Update() {
            UpdateUserStatus();
        }

        private void UpdateUserStatus() {
            u1Status.text = offText;
            u1Status.color = offColor;
            u2Status.text = offText;
            u2Status.color = offColor;
            if (userManager.nwPlayer1 != null) {
                u1Status.text = onText;
                u1Status.color = onColor;
            }
            else {
                u1Status.text = offText;
                u1Status.color = offColor;
            }
            if (userManager.nwPlayer2 != null) {
                u2Status.text = onText;
                u2Status.color = onColor;
            }
            else {
                u2Status.text = offText;
                u2Status.color = offColor;
            }
        }


        public void User1ChangeMapping() {
            userManager.AssignUserToEntity(1, int.Parse(u1Input.text));
        }
        public void User2ChangeMapping() {
            userManager.AssignUserToEntity(2, int.Parse(u2Input.text));
        }
        public void SwitchAssignement() {
            int u1 = int.Parse(u1Input.text);
            int u2 = int.Parse(u2Input.text);

            //userManager.AssignUserToEntity(1, u2);
            //userManager.AssignUserToEntity(2, u1);

            u1Input.text = u2.ToString();
            u2Input.text = u1.ToString();

            User1ChangeMapping();
            User2ChangeMapping();

        }
    }
}