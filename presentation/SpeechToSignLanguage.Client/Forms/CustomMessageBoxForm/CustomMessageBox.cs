using System;
using System.Windows.Forms;

namespace SpeechToSignLanguage.Client.Forms
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox()
        {
            InitializeComponent();
        }

        //first we will create a static method to be able to call the custom message box
        //the method/function should return dialogresult
        public static DialogResult Show(String Text, string label1, string label2)
        {
            //create an instance
            CustomMessageBox messageBox = new CustomMessageBox();

            //lets set the text
            messageBox.lblText.Text = Text;
            messageBox.label1.Text = label1;
            messageBox.label2.Text = label2;
            //lets show as a dialog
            messageBox.ShowDialog();

            return messageBox.DialogResult;
        }
        //It's done, now lets apply on our form and test

        //lets set the value that will return once the buttons have clicked
        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
