using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clustering
{
    public partial class Form1 : Form
    {
        public Form1 ()
        {
            InitializeComponent();
        }

        public void setDataSource (List<DataTable> datasrc)
        {
            dataGridView1.DataSource = datasrc[0];
            dataGridView2.DataSource = datasrc[1];
            dataGridView3.DataSource = datasrc[2];
            dataGridView4.DataSource = datasrc[3];
            dataGridView5.DataSource = datasrc[4];
        }

        private void dataGridView1_CellContentClick (object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick (object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
