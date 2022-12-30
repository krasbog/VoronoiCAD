using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
namespace VoronoiCAD
{
    public partial class XDataForm : Form
    {
        List<string> _colsName;
        List<string[]> _colsData;
        public XDataForm(List<string> colsName, List<string[]> colsData)
        {
            _colsName = colsName;
            _colsData = colsData;
            InitializeComponent();
        }

        private void XDataForm_Load(object sender, EventArgs e)
        {
            for (int i =0; i < _colsName.Count; i++)
            {
                var column1 = new DataGridViewColumn();
                column1.HeaderText = _colsName[i]; //текст в шапке
                column1.Width = 100; //ширина колонки
                column1.ReadOnly = true; //значение в этой колонке нельзя править
                //column1.Name = "name"; //текстовое имя колонки, его можно использовать вместо обращений по индексу
                //column1.Frozen = true; //флаг, что данная колонка всегда отображается на своем месте
                column1.CellTemplate = new DataGridViewTextBoxCell(); //тип нашей колонки
                dataGridView1.Columns.Add(column1);

                
            }
            int rowMax = _colsData.Max(x => x.Count());
            dataGridView1.Rows.Add(rowMax);


            for (int i = 0; i < _colsData.Count; i++)
            {
                for (int j = 0; j < _colsData[i].Count(); j++)
                {
                   
                    dataGridView1[i, j].Value = _colsData[i][j];
                }
            }

        }
    }
}
