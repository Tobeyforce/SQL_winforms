using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBLabs
{
    public partial class AddStudentControl : UserControl
    {
        private DBConnection dbconn;

        public AddStudentControl()
        {
            /*
             * Constructor the control
             * 
             * You DONT need to edit this constructor.
             * 
             */
            InitializeComponent();
        }

        public void AddStudentControlSettings(ref DBConnection dbconn)
        {
            /*
             * Since UserControls cannot take arguments to the constructor 
             * this function is called after the constructor to perform this.
             * 
             * You DONT need to edit this function.
             * 
             */
            this.dbconn = dbconn; 
        }

        private void LoadAddStudentControl(object sender, EventArgs e)
        {
            ////ADD CODE HERE WHICH MAKES SURE STUDFENTTYPE AND PHONETYPE IS ADDED DYNAMICALLY
            /*
             * This function contains all code that needs to be executed when the control is first loaded
             * 
             * You need to edit this code. 
             * Example: Population of Comboboxes and gridviews etc.
             * 
             */

            DataTable phoneTypeDt = dbconn.GetPhoneTypes();
            if (phoneTypeDt != null)
            {
                PhoneType.DataSource = phoneTypeDt;
                PhoneType.DisplayMember = "PhoneTypeName";
                //PhoneType.ValueMember = phoneTypeDt;
            }
            else
            {
                PhoneType.Text = "No data found";
            }
            DataTable studentTypeDt = dbconn.GetStudentTypes();
            
            if (studentTypeDt != null)
            {
                StudentType.DataSource = studentTypeDt;
                StudentType.DisplayMember = "StudentType";
            }
            else
            {
                StudentType.Text = "No data found";
            }
        }
        public void ResetAddStudentControl()
        {
  
            StudentID.Clear();
            LastName.Clear();
            FirstName.Clear();
            Number.Clear();
            PhoneType.SelectedIndex = -1;
            StudentType.SelectedIndex = -1;
            Country.Clear();
            City.Clear();
            ZipCode.Clear();
            StreetAdress.Clear();
            Gender.Clear();
        }


        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (dbconn.SubmitStudent(StudentID.Text, FirstName.Text, LastName.Text, Gender.Text, StreetAdress.Text,
                ZipCode.Text, City.Text, Country.Text, Birthdate.Value.ToString("yyyy-MM-dd"), StudentType.Text))
            {
                ResetAddStudentControl();
            }
        }

        private void AddNumberButton_Click(object sender, EventArgs e)
        {
            dbconn.AddNumber(Number.Text, PhoneType.Text);
            PhoneType.SelectedIndex = -1;
            Number.Clear();
        }

    }
}
