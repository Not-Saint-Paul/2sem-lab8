using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab8
{
    public partial class Form1 : Form, IView
    {
        public Form1()
        {
            InitializeComponent();
            Presenter programPresenter = new Presenter(this);
        }
        public event EventHandler<EventArgs> SyncronizeDirectoriesEvent;
        public event EventHandler<EventArgs> Inversion;
        public event EventHandler<EventArgs> ShowFilesOfFirstDirectoryEvent;
        public event EventHandler<EventArgs> ShowFilesOfSecondDirectoryEvent;
        string IView.FirstDirectory() { return textBox1.Text; }
        string IView.SecondDirectory() { return textBox2.Text; }
        void IView.ShowFilesOfFirstDirectory(List<string> list)
        {
            List<string> outputList = list;
            foreach (string output in outputList)
            {
                listBox1.Items.Add(output);
            }
        }

        void IView.ShowFilesOfSecondDirectory(List<string> list)
        {
            List<string> outputList = list;
            foreach (string output in outputList)
            {
                listBox2.Items.Add(output);
            }
        }

        void IView.TrySynchronize(List<string> message)
        {

            List<string> outputList = message;

            foreach (string output in outputList)
            {
                listBox3.Items.Add(output);
            }

        }

        private void button1_Click(object sender, EventArgs inputEvent)
        {
            SyncronizeDirectoriesEvent(sender, inputEvent);
            ShowFilesOfFirstDirectoryEvent(sender, inputEvent);
            ShowFilesOfSecondDirectoryEvent(sender, inputEvent);
        }
        private void radioButton1_Click(object sender, EventArgs inputEvent)
        {
            Inversion(sender, inputEvent);
        }
    }
}
