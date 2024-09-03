﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Payazob.FRM.Aid
{
    public partial class frmAidPersonList : Form
    {
        string Sec_ = "";
        /// <summary>
        /// نام قسمت به عنوان ورودی 
        /// INV انبار
        /// SUP سرپرست
        /// MAN مدیر
        /// </summary>
        /// <param name="Sec"></param>
        public frmAidPersonList(string Sec)
        {
            InitializeComponent();
            Sec_ = Sec;
            dataGridView1.Visible = false;
            dt_NameAndFamily = new BLL.authentication().NameOfUser();
            DataGridViewComboBoxColumn combobox_xMaterialType_ = new DataGridViewComboBoxColumn()
            {
                DisplayIndex = 4,
                HeaderText = "نوع جنس",
                DataSource = new BLL.Aid.csAid().SlSectionAidMaterialAllowList(-1),
                DisplayMember = "xMaterialName",
                ValueMember = "x_",
                DataPropertyName = "xMaterial_",
                Name = "cmb_Material",
                Width = 150,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
            };

            dataGridView1.Columns.Add(combobox_xMaterialType_);

            DataGridViewComboBoxColumn combobox_xSupplier_ = new DataGridViewComboBoxColumn()
            {
                DisplayIndex = 4,
                HeaderText = "ثبت کننده",
                DataSource = dt_NameAndFamily,
                DisplayMember = "NameAndFamily",
                ValueMember = "x_",
                DataPropertyName = "xSupplier_",
                Name = "cmb_xSupplier_",
                Width = 150,
                ReadOnly = true,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
            };
            dataGridView1.Columns.Add(combobox_xSupplier_);


            DataGridViewComboBoxColumn combobox_xApprove_ = new DataGridViewComboBoxColumn()
            {
                DisplayIndex = 4,
                HeaderText = " انبار دار",
                DataSource = dt_NameAndFamily,
                DisplayMember = "NameAndFamily",
                ValueMember = "x_",
                DataPropertyName = "xApprove_",
                Name = "cmb_xApprove_",
                Width = 150,
                ReadOnly = true,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
            };
            dataGridView1.Columns.Add(combobox_xApprove_);

            dt_A = new DAL.Aid.DataSet_Aid.SlSectionAidDeliveryNewDataTable();
            bindingSource1.DataSource = dt_A;
            dataGridView1.DataSource = bindingSource1;
            bindingNavigator1.BindingSource = bindingSource1;
            Generate();


            SectionSpliatorFix();
        }
        void SectionSpliatorFix()
        {
            if (Sec_ == "INV")
            {
                //  splitContainer4.Panel1Collapsed = true;
                //splitContainer3.SplitterDistance = 150;
                splitContainer3.FixedPanel = FixedPanel.Panel2;
            }
            else if (Sec_ == "SUP")
            {
                splitContainer4.Panel2Collapsed = true;
                splitContainer3.SplitterDistance = 150;
            }
            else if (Sec_ == "MAN")
            {
                splitContainer4.Panel1Collapsed = true;
                //splitContainer3.SplitterDistance = 150;
                splitContainer3.FixedPanel = FixedPanel.Panel2;

            }
        }
        DAL.Aid.DataSet_Aid.SlSectionAidDeliveryNewDataTable dt_A;
        DataTable dt_NameAndFamily;
        private void btn_Show_Click(object sender, EventArgs e)
        {
            ShowListPerson();
            (dataGridView1.Columns["cmb_Material"] as DataGridViewComboBoxColumn).DataSource
                 = new BLL.Aid.csAid().SlSectionAidMaterialAllowList(User_);
            ShowData();
        }
        void ShowListPerson()
        {
            Form frm = new Form();
            ControlLibrary.uc_SearchData uc = new ControlLibrary.uc_SearchData();
            uc.ColumnFilter = "Name";
            uc.ResultCustom = "xHSEWarning";
            uc.DataGridViewHeaderText("Name", "نام و نام خانوادگی");
            uc.DataGridViewClmHide("x_");
            uc.GenDataGridView(new BLL.Person.csPersonInfo().mPersonName());
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            frm.Controls.Add(uc);
            frm.Size = uc.Size;
            frm.Height += 30;
            frm.Width += 30;
            frm.StartPosition = FormStartPosition.CenterParent;
            uc.Dock = DockStyle.Fill;
            frm.ShowDialog();
            User_ = int.Parse(uc.ResultID);
            if (uc.ResultCustomValue == "True")
            {
                MessageBox.Show("خواهشمندم جهت دریافت اقلام ایمنی به واحد ایمنی و بهداشت مراجعه بفرمایید");
                User_ = -1;
            }
            uc.Dispose();
            frm.Dispose();
            lbl_Name.Text = uc.ResultName;
        }
        int User_ = -1;
        void ShowData()
        {

            dataGridView1.Visible = true;
            dt_A = new BLL.Aid.csAid().mSectionAidDelivery(uc_Filter_Date1.DateFrom, uc_Filter_Date1.DateTo, User_);
            bindingSource1.DataSource = dt_A;
            dt_A.xPerson_Column.DefaultValue = User_;
            dt_A.xDateColumn.DefaultValue = BLL.csshamsidate.shamsidate;
            dt_A.xSupplier_Column.DefaultValue = BLL.authentication.x_;
            dataGridView1.DataSource = bindingSource1;
            bindingNavigator1.BindingSource = bindingSource1;
            dataGridView2.DataSource = new BLL.Aid.csAid().SlSectionAidNeed(User_);
            CreateCheckBox();
            Generate();
            GenerateDVG2();

        }
        void Generate()
        {
            dataGridView1.Columns["xDate"].HeaderText = "تاریخ";
            dataGridView1.Columns["xComment"].HeaderText = "توضیحات";
            dataGridView1.Columns["xApprove"].HeaderText = "تایید انبار";

            if (Sec_ != "INV")
            {
                dataGridView1.Columns["xApprove"].ReadOnly = true;
                // dataGridView1.Columns["cmb_Material"].ReadOnly = true;
            }
            if (Sec_ == "INV")
            {
                // dataGridView1.Columns["xApprove"].ReadOnly = true;
                dataGridView1.Columns["cmb_Material"].ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
            }
            if (Sec_ == "USER")
            {
                dataGridView1.ReadOnly = true;
                bindingNavigator1.Enabled = false;
                panel1.Enabled = false;
                dataGridView2.ReadOnly = true;
            }
            if (Sec_ == "MAN")
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells["xApprove"].Value != DBNull.Value 
                        && (bool?)item.Cells["xApprove"].Value == true)
                    {
                        item.Cells["cmb_Material"].ReadOnly = true;
                    }
                }
            }
            dataGridView1.Columns["xApprove"].HeaderText = "تایید انبار";
            dataGridView1.Columns["xCount"].HeaderText = "تعداد";
            dataGridView1.Columns["x_"].Visible = false;
            dataGridView1.Columns["xPerson_"].Visible = false;
            dataGridView1.Columns["xDate"].ReadOnly = true;
            dataGridView1.Columns["cmb_xSupplier_"].ReadOnly = true;
            dataGridView1.Columns["cmb_xApprove_"].ReadOnly = true;




        }
        void GenerateDVG2()
        {
            dataGridView2.Columns["xMaterial"].HeaderText = "نام لوازم ایمنی";
            dataGridView2.Columns["xPeriod"].HeaderText = "دوره مصرف";
            dataGridView2.Columns["Date1"].HeaderText = "تاریخ اخرین دریافت";
            dataGridView2.Columns["Date2"].HeaderText = "موعد دریافت";

            dataGridView2.Columns["Accept"].Visible = false;
            dataGridView2.Columns["xMaterial_"].Visible = false;

            foreach (DataGridViewRow item in dataGridView2.Rows)
            {
                if (item.Cells["Accept"].Value != null && item.Cells["Accept"].Value != DBNull.Value && item.Cells["Accept"].Value.ToString() == "1")
                {
                    item.DefaultCellStyle.BackColor = Color.MediumSpringGreen;
                }
            }
        }
        void CreateCheckBox()
        {
            panel1.Controls.Clear();
            DAL.Aid.DataSet_Aid.SlSectionAidNeedDataTable dt = new BLL.Aid.csAid().SlSectionAidNeed(User_);
            int y = 3;
            foreach (DataRow item in dt)
            {
                if ((int?)item["Accept"] == 1)
                {
                    CheckBox chb = new CheckBox();
                    chb.AutoSize = true;
                    chb.Location = new System.Drawing.Point(30, y += 17);
                    chb.Name = "chb" + item["xMaterial_"].ToString();
                    chb.Size = new System.Drawing.Size(77, 17);
                    chb.TabIndex = 0;
                    chb.Dock = DockStyle.Top;
                    chb.Text = item["xMaterial"].ToString();
                    chb.UseVisualStyleBackColor = true;
                    chb.Tag = item["xMaterial_"];
                    panel1.Controls.Add(chb);
                }
                else 
                {
                    if (Sec_ == "INV")
                    {
                        CheckBox chb = new CheckBox();
                        chb.AutoSize = true;
                        chb.Location = new System.Drawing.Point(30, y += 17);
                        chb.Name = "chb" + item["xMaterial_"].ToString();
                        chb.Size = new System.Drawing.Size(77, 17);
                        chb.TabIndex = 0;
                        chb.Dock = DockStyle.Top;
                        chb.Text = item["xMaterial"].ToString() + "-" + "خارج از نوبت";
                        chb.UseVisualStyleBackColor = true;
                        chb.Tag = item["xMaterial_"];
                        chb.BackColor = Color.LightPink;
                        panel1.Controls.Add(chb);
                    }
                }
            }
        }
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (new CS.csMessage().ShowMessageSaveYesNo() && User_ > 0)
            {
                if (Sec_ == "SUP" || Sec_ == "INV")
                    foreach (CheckBox item in panel1.Controls.OfType<CheckBox>())
                    {
                        if (item.Checked == true)
                        {
                            DAL.Aid.DataSet_Aid.SlSectionAidDeliveryNewRow dr = dt_A.NewSlSectionAidDeliveryNewRow();
                            dr["xDate"] = BLL.csshamsidate.shamsidate;
                            dr["xMaterial_"] = (int)item.Tag;
                            // dr["xCount"] = 0;
                            dr["xPerson_"] = User_;
                            dr["xSupplier_"] = BLL.authentication.x_;
                            dr["xApprove"] = 0;
                            dr["xComment"] = "";
                            dt_A.AddSlSectionAidDeliveryNewRow(dr);
                        }
                    }

                this.Validate();
                dataGridView1.EndEdit();
                BLL.Aid.csAid cs = new BLL.Aid.csAid();
                MessageBox.Show(cs.UdSectionAidDelivery(dt_A));
                ShowData();

            }
        }

        private void frmAidPersonList_Load(object sender, EventArgs e)
        {

            ShowListPerson();

            (dataGridView1.Columns["cmb_Material"] as DataGridViewComboBoxColumn).DataSource = new BLL.Aid.csAid().SlSectionAidMaterialAllowList(User_);

            ShowData();
            if (User_ == -1)
            {
                this.Close();
            }


        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (dataGridView1.Columns[e.ColumnIndex].Name == "xApprove")
            {
                if ((bool)dataGridView1[e.ColumnIndex, e.RowIndex].FormattedValue == true)
                {
                    dataGridView1["cmb_xApprove_", e.RowIndex].Value = BLL.authentication.x_;
                    dataGridView1["xDate", e.RowIndex].Value = BLL.csshamsidate.shamsidate;
                }
                else
                {
                    dataGridView1["cmb_xApprove_", e.RowIndex].Value = DBNull.Value;
                    //  dataGridView1["xDate", e.RowIndex].Value = DBNull.Value;

                }
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {

            DataTable dt = new BLL.Aid.csAid().SlSectionAidDelivery(uc_Filter_Date1.DateFrom, uc_Filter_Date1.DateTo, User_);

            foreach (DataGridViewRow item1 in dataGridView1.Rows)
            {
                if (dataGridView1.SelectedRows.Count < 1)
                    break;

                if (!item1.Selected)
                {

                    foreach (DataRow item2 in dt.Rows)
                    {
                        if ((int)item1.Cells["x_"].Value == (int)item2["x_"])
                        {
                            item2.Delete();
                            break;
                        }
                    }
                }
                dt.AcceptChanges();
            }


            Report.SendReport cs = new Report.SendReport();
            cs.SetParam("DateNow", BLL.csshamsidate.shamsidate);
            frmReport r = new frmReport();
            r.GetReport = cs.GetReport(dt, "crsAidPerson");
            r.ShowDialog();
            r.Dispose();

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Validation.VTranslateException v = new Validation.VTranslateException();

            if (e.Exception.Message.Contains("DataGridViewComboBoxCell"))
            {
                MessageBox.Show("اقلام ایمنی داده شده هماهنگ با لیست تنظیم با ایمنی نمیباشد");

                (dataGridView1.Columns["cmb_Material"] as DataGridViewComboBoxColumn).DataSource
                    = new BLL.csMaterial().SlMaterial("", (int)CS.csEnum.MaterilaType.lavazememeni);

            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ControlLibrary.uc_ExportExcelFile uc = new ControlLibrary.uc_ExportExcelFile();
            uc.Fildvg = dataGridView1;
            uc.RunExcel();
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

        }
    }
}
