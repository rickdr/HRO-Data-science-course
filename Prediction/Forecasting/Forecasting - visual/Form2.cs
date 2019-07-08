using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forecasting___visual
{
    public partial class Form2 : Form
    {
        public Form2 ()
        {
            InitializeComponent();
        }

        public void setDataSource (DataTable datasrc)
        {
            dataGridView1.DataSource = datasrc;
        }

        private void dataGridView1_CellContentClick (object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
