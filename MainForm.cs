using System;
using System.ComponentModel;
using System.Windows.Forms;
namespace PdfVerticalDuplicator
{
    public partial class MainForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();


        public MainForm()
        {
            InitializeComponent();


            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PDF files (*.pdf)|*.pdf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtInput.Text = ofd.FileName;
                    txtOutput.Text = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(ofd.FileName),
                    System.IO.Path.GetFileNameWithoutExtension(ofd.FileName) + "_vertical.pdf");


                    // Reset progress and status when a new file is selected
                    progressBar.Value = 0;
                    lblStatus.Text = "Ready";
                }
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF files (*.pdf)|*.pdf";
                sfd.FileName = System.IO.Path.GetFileName(txtOutput.Text);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txtOutput.Text = sfd.FileName;
                }
            }
        }


        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text) || string.IsNullOrEmpty(txtOutput.Text))
            {
                MessageBox.Show("Choose input and output files first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            btnProcess.Enabled = false;
            lblStatus.Text = "Processing...";
            progressBar.Value = 0;


            worker.RunWorkerAsync(new string[] { txtInput.Text, txtOutput.Text });
        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (string[])e.Argument;
            string input = args[0];
            string output = args[1];


            PdfProcessor.Process(input, output, (p) => worker.ReportProgress(p));
        }


        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = $"{e.ProgressPercentage}%";
        }


        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnProcess.Enabled = true;
            lblStatus.Text = "Done";
            MessageBox.Show("Finished.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }


        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                txtInput.Text = files[0];
                txtOutput.Text = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(files[0]),
                System.IO.Path.GetFileNameWithoutExtension(files[0]) + "_vertical.pdf");


                // Reset progress and status when a new file is selected via drag & drop
                progressBar.Value = 0;
                lblStatus.Text = "Ready";
            }
        }
    }
}