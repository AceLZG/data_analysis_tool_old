using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DataAnalysisTool
{
    public partial class frmJMPScriptSelect : Form
    {
        public frmJMPScriptSelect()
        {
            InitializeComponent();
        }
        public delegate void ReturnDataEventHandler(object sender, ReturnEventArgs e);
        public event ReturnDataEventHandler ReturnMsg;        

        private void btnOK_Click(object sender, EventArgs e)
        {
            ReturnEventArgs rea = new ReturnEventArgs();
            Application.DoEvents();

            //if (rbnCX.Checked)
            //{
            //    rea.strMsg = "CX";
            //    rea.intMsg = 1;
            //}
            //if (rbnPAx.Checked)
            //{
            //    rea.strMsg = "PAx";
            //    rea.intMsg = 2;
            //}
            //if (rbnGiga.Checked)
            //{
            //    rea.strMsg = "Giga";
            //    rea.intMsg = 3;
            //}

            string[] content = cbxScript.SelectedItem.ToString().Split(',');
            rea.strMsg = content[0].Substring(1);
            rea.intMsg = Convert.ToInt32(content[1].Substring(0, 2));
            
            if (ReturnMsg != null)//加此判断的作用是为了在此事件未实现的时候不会出现错误
            {
                this.Hide();
                ReturnMsg(this, rea);
            }

            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ReturnEventArgs rea = new ReturnEventArgs();
            Application.DoEvents();

            rea.strMsg = "";
            rea.intMsg = 0;
            if (ReturnMsg != null)//加此判断的作用是为了在此事件未实现的时候不会出现错误
            {
                this.Hide();
                ReturnMsg(this, rea);
            }
            this.Dispose();
        }

        private void frmJMPScriptSelect_Load(object sender, EventArgs e)
        {
            int i = 1;
            DirectoryInfo dir = new DirectoryInfo(@".\Script\");
            if (!dir.Exists) dir.Create();

            Dictionary<string, int> items = new Dictionary<string, int>();
            foreach (FileInfo fChild in dir.GetFiles("*.jsl"))
            {
                string[] content = fChild.Name.Split('.');
                items.Add(content[0], i);
                i++;
            }
            cbxScript.DataSource = new BindingSource(items, null);
            cbxScript.DisplayMember = "Key";
            cbxScript.ValueMember = "Value";
        }

    }

    public class ReturnEventArgs : System.EventArgs
    {
        public string strMsg;
        public int intMsg;
        public ReturnEventArgs()//编写构造函数，可以使用参数的形式直接构造字段或属性
        { }
    }

}
