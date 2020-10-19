using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Z.EntityFramework.Plus;

namespace Ef_Crud_App
{
    public partial class Form1 : Form
    {
        Customer customer_model = new Customer();
        List<Customer> mainListCustomer = new List<Customer>();
        List<Customer> cloneListCustomer = new List<Customer>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txt_FirstName.Text = txt_LastName.Text = txt_City.Text = txt_Address.Text = "";
            //btnSave.Text                = "Save";
            //btnDelete.Enabled           = false;
            customer_model.CustomerID   = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            PopulateDataGridView();
        }

        private void PopulateDataGridView()
        {
            dgvCustomer.AutoGenerateColumns = false;
            using(EFDBEntities db = new EFDBEntities())
            {
                mainListCustomer    = db.Customers.ToList<Customer>();
            }
            cloneListCustomer       = mainListCustomer;
            dgvCustomer.DataSource  = cloneListCustomer;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            customer_model.FirstName = txt_FirstName.Text.Trim();
            customer_model.LastName = txt_LastName.Text.Trim();
            customer_model.City = txt_City.Text.Trim();
            customer_model.Address = txt_Address.Text.Trim();

            using (EFDBEntities db = new EFDBEntities())
            {
                if (customer_model.CustomerID == 0) //Insert
                    db.Customers.Add(customer_model);
                else //Update current row
                    db.Entry(customer_model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            Clear();
            PopulateDataGridView();
            MessageBox.Show("Add Successfully");
        }

        private void dgvCustomer_DoubleClick(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow.Index != -1)
            {
                customer_model.CustomerID   = Convert.ToInt32(dgvCustomer.CurrentRow.Cells["CustomerID"].Value);
                using (EFDBEntities db = new EFDBEntities())
                {
                    customer_model = db.Customers.Where(x => x.CustomerID == customer_model.CustomerID).FirstOrDefault();
                    txt_FirstName.Text  = customer_model.FirstName;
                    txt_LastName.Text   = customer_model.LastName;
                    txt_City.Text       = customer_model.City;
                    txt_Address.Text    = customer_model.Address;
                }
                //btnSave.Text            = "Update";
                //btnDelete.Enabled       = true;
            }    
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Update all gridview?","Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (EFDBEntities db = new EFDBEntities())
                {
                    var query = from t in db.Customers.AsEnumerable()
                                join r in cloneListCustomer.AsEnumerable()
                                     on t.CustomerID equals r.CustomerID
                                select t;
                    //Remove
                    db.Customers.RemoveRange(query);
                    //Add
                    db.Customers.AddRange(cloneListCustomer);
                    db.SaveChanges();
                    PopulateDataGridView();
                    Clear();
                    MessageBox.Show("Deleted Successfully");
                }
            }
        }
    }
}
