using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

// ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

// Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;
using Bricscad.Ribbon;

// com
//using BricscadDb;
//using BricscadApp;

// alias
using _AcRx = Teigha.Runtime;
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcGe = Teigha.Geometry;
using _AcEd = Bricscad.EditorInput;
using _AcGi = Teigha.GraphicsInterface;
using _AcClr = Teigha.Colors;
using _AcWnd = Bricscad.Windows;

namespace VoronoiCAD
{
    public partial class MainDialog : Form
    {
        public string GeoFileName = null;
        public string LiraNewPileFileName = null;
        public string RzFileName = null;
        public string OrigFileName = null;
        public string ResultFileName = null;
        public MainDialog()
        {
            InitializeComponent();
        }

        private void buttonSelectGeoFile_Click(object sender, EventArgs e)
        {
           
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //Document doc =

            // _AcAp.Application.DocumentManager.MdiActiveDocument;

            //HostApplicationServices hs =

            //  HostApplicationServices.Current;

            string path = Properties.Settings.Default.xlsGeoFilePuth;

            //hs.FindFile(

            //  doc.Name,

            //  doc.Database,

            //  FindFileHint.Default

            //);
            if (path.Length > 0)
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            openFileDialog1.Filter = "xls files (*.xls)|*.xls|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxGeoFile.Text = openFileDialog1.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            GeoFileName = textBoxGeoFile.Text;
            LiraNewPileFileName = textBoxLiraPileFile.Text;
            RzFileName = textBoxRzFile.Text;
            OrigFileName = textBoxOrigTxtFile.Text;
            ResultFileName = textBoxResultTxtFile.Text;

        Properties.Settings.Default.xlsGeoFilePuth = textBoxGeoFile.Text;
            Properties.Settings.Default.txtLiraNewPileFilePuth = textBoxLiraPileFile.Text;
            Properties.Settings.Default.xlsRzFilePuth = textBoxRzFile.Text;
            Properties.Settings.Default.txtLiraOrigFilePuth= textBoxOrigTxtFile.Text;
            Properties.Settings.Default.txtLiraResultFilePuth= textBoxResultTxtFile.Text;
            Properties.Settings.Default.Save();
           Close();
        }

        private void MainDialog_Load(object sender, EventArgs e)
        {
            textBoxGeoFile.Text = Properties.Settings.Default.xlsGeoFilePuth;
            textBoxLiraPileFile.Text = Properties.Settings.Default.txtLiraNewPileFilePuth;
            textBoxRzFile.Text = Properties.Settings.Default.xlsRzFilePuth;
            textBoxOrigTxtFile.Text = Properties.Settings.Default.txtLiraOrigFilePuth;
            textBoxResultTxtFile.Text = Properties.Settings.Default.txtLiraResultFilePuth;
        }

        private void buttonSelectLiraNewPileFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
          

            string path = Properties.Settings.Default.txtLiraNewPileFilePuth;

            
            if(path.Length>0)
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxLiraPileFile.Text = openFileDialog1.FileName;
            }

        }

        private void buttonSelectRzFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            string path = Properties.Settings.Default.xlsRzFilePuth;


            if (path.Length > 0)
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            openFileDialog1.Filter = "xls files (*.xls)|*.xls|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxRzFile.Text = openFileDialog1.FileName;
            }

        }

        private void buttonSelectOrigTxtFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();


            string path = Properties.Settings.Default.txtLiraOrigFilePuth;


            if (path.Length > 0)
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxOrigTxtFile.Text = openFileDialog1.FileName;
                
            }


        }

        private void buttonSelectResultTxtFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();


            string path = Properties.Settings.Default.txtLiraResultFilePuth;


            if (path.Length > 0)
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxResultTxtFile.Text = openFileDialog1.FileName;

            }

        }
    }
}
