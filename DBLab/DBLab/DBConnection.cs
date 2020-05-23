using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace DBLabs
{
    public class DBConnection : DBLabsDLL.DBConnectionBase
    {
        private string Datasource = "www4.idt.mdh.se";
        private string OurDb = "DVA234_2020_G23_db";
        List<string> Phonenumbers = new List<string>();
        List<string> Phonetypes = new List<string>();

        private string connectionString;
        private SqlConnection con;

        ///*
        // * The constructor
        // */
        public DBConnection()
        {
            
        }

        public override bool login(string username, string password)
        {
            try
            {
                connectionString = $"Data Source={Datasource};" + $"Initial Catalog={OurDb};" + $"User Id={username};" + $"Password={password};";
                this.con = new SqlConnection(connectionString);
            
                Console.WriteLine("Connection successfully established");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error during connection!" + e);
                return false;
            }

            //DVA234_2020_G23
        }


        public DataTable GetStudentTypes()
        {
            try
            {
                con.Open();
                string studentQuery = "select StudentType from dbo.StudentType";
                SqlDataAdapter da = new SqlDataAdapter(studentQuery, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                con.Close();
                Console.WriteLine(e);
                return null;
            }
           
        }

        public DataTable GetPhoneTypes()
        {
            try
            {
                con.Open();
                string phoneQuery = "select PhoneTypeName from dbo.PhoneTypeCollection";
                SqlDataAdapter da = new SqlDataAdapter(phoneQuery, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                con.Close();
                Console.WriteLine(e);
                return null;
            }
        }

        public bool SubmitStudent(string studentId, string firstName, string lastName, string gender, 
            string streetAdress, string zipCode, string city, string country, string birthdate, string studentType)
        {

            if (Phonenumbers.Count == 0 || Phonetypes.Count == 0)
            {
                MessageBox.Show("No number or type entered!");
                ClearNumbers();
                return false;

            }
            if (Phonenumbers.Count != Phonetypes.Count)
            {
                MessageBox.Show("You must enter both number and type!");
                ClearNumbers();
                return false;
            }
            con.Open();
            SqlDataAdapter daAddStudent = new SqlDataAdapter("addStudents", con);
            daAddStudent.SelectCommand.CommandType = CommandType.StoredProcedure;
            daAddStudent.SelectCommand.Parameters.Add("@StudentID", SqlDbType.VarChar).Value = studentId;
            daAddStudent.SelectCommand.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = firstName;
            daAddStudent.SelectCommand.Parameters.Add("@LastName", SqlDbType.VarChar).Value = lastName;
            daAddStudent.SelectCommand.Parameters.Add("@Gender", SqlDbType.VarChar).Value = gender;
            daAddStudent.SelectCommand.Parameters.Add("@StreetAdress", SqlDbType.VarChar).Value = streetAdress;
            daAddStudent.SelectCommand.Parameters.Add("@Zipcode", SqlDbType.VarChar).Value = zipCode;
            daAddStudent.SelectCommand.Parameters.Add("@City", SqlDbType.VarChar).Value = city;
            daAddStudent.SelectCommand.Parameters.Add("@Country", SqlDbType.VarChar).Value = country;
            daAddStudent.SelectCommand.Parameters.Add("@Birthdate", SqlDbType.VarChar).Value = birthdate;
            daAddStudent.SelectCommand.Parameters.Add("@StudentType", SqlDbType.VarChar).Value = studentType;
            //TO BE ABLE TO RECEIVE SUCCESS OUTPUT
            SqlParameter studentSuccess = new SqlParameter();
            studentSuccess.Direction = ParameterDirection.ReturnValue;
            daAddStudent.SelectCommand.Parameters.Add(studentSuccess);
            try
            {
                daAddStudent.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return false;
            }
            if ((int)studentSuccess.Value == 1)
            {
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return false;
            }
            MessageBox.Show("Student added");


            for (int i = 0; i < Phonenumbers.Count; i++)
            {
                SqlDataAdapter daPhonenumbers = new SqlDataAdapter("addStudentPhoneNo", con);
                daPhonenumbers.SelectCommand.CommandType = CommandType.StoredProcedure;
                daPhonenumbers.SelectCommand.Parameters.Add("@studentID", SqlDbType.VarChar).Value = studentId;
                daPhonenumbers.SelectCommand.Parameters.Add("@number", SqlDbType.VarChar).Value = Phonenumbers[i];
                daPhonenumbers.SelectCommand.Parameters.Add("@phoneType", SqlDbType.VarChar).Value = Phonetypes[i];

                SqlParameter phoneSuccess = new SqlParameter();
                phoneSuccess.Direction = ParameterDirection.ReturnValue;
                daPhonenumbers.SelectCommand.Parameters.Add(phoneSuccess);
                try
                {
                    daPhonenumbers.SelectCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    MessageBox.Show("Error, numbers not added...");
                    ClearNumbers();
                    con.Close();
                    return false;
                }
            }
            MessageBox.Show("Numbers added");

            con.Close();
            ClearNumbers();
            return true;
        }

        public bool AddNumber(string number, string type)
        {
            if (number.Length > 15 || number.Length <= 0 || type.Length <= 0)
            {
                MessageBox.Show("Invalid phonenumber");
                return false;
            }

            if (Phonenumbers.Contains(number))
            {
                MessageBox.Show("Phonenumber already exists");
                return false;
            }
            Phonenumbers.Add(number);
            Phonetypes.Add(type);
            return true;
        }

        public void ClearNumbers()
        {
            Phonenumbers.Clear();
            Phonetypes.Clear();
        }


        /*
         --------------------------------------------------------------------------------------------
         STUB IMPLEMENTATIONS TO BE USED IN LAB 3. 
         --------------------------------------------------------------------------------------------
        */


        /********************************************************************************************
         * DATABASE UPDATING METHODS
         *******************************************************************************************/

        /*
         * Add a prerequisite for a course
         * 
         * Parameters:
         *              cc          CourseCode of the course on which to add a prerequisite
         *              preReqcc    CourseCode of the course that is the prerequisite
         *              
         * Return value:
         *              1           Prerequisite added
         *              Any other   Error
         */
        public override int addPreReq(string cc, string preReqcc)
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("spAddPreReq", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            da.SelectCommand.Parameters.Add("@preReqcc", SqlDbType.VarChar).Value = preReqcc;
            SqlParameter success = new SqlParameter();
            success.Direction = ParameterDirection.ReturnValue;
            da.SelectCommand.Parameters.Add(success);
            try
            {
                da.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return 0;
            }
            if ((int)success.Value == 1)
            {
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return 0;
            }
            else
            {
                MessageBox.Show("Prerequisite course " + preReqcc + " added for " + cc);

            }
            con.Close();
            return 1;
        }

        /*
         * Add a course instance for a course
         * 
         * Parameters:
         *              cc          CourseCode of the course on which to add a course instance
         *              year        The year for the course instance
         *              period      The period for the course instance
         *              
         * Return value:
         *              1           Course instance added
         *              Any other   Error
         */
        public override int addInstance(string cc, int year, int period)
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("spAddCourseInstance", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            da.SelectCommand.Parameters.Add("@year", SqlDbType.Int).Value = year;
            da.SelectCommand.Parameters.Add("@period", SqlDbType.Int).Value = period;

            SqlParameter success = new SqlParameter();
            success.Direction = ParameterDirection.ReturnValue;
            da.SelectCommand.Parameters.Add(success);
            try
            {
                da.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return 0;
            }
            if ((int)success.Value == 1)
            {
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return 0;
            }
            else
            {
                MessageBox.Show("Instance added of course: " + cc);
            }
            con.Close();
            return 1;
        }

        /*
         * Add a teacher staffing for a course
         * 
         * Parameters:
         *              pnr         "Personnummer" for the teacher to staff
         *              cc          CourseCode of the course on which to add a teacher
         *              year        The year for the course instance
         *              period      The period for the course instance
         *              hours       The number of hours to staff the teacher
         *              
         * Return value:
         *              1           Teacher staffing added
         *              Any other   Error
         */
        public override int addStaff(string pnr, string cc, int year, int period, int hours)
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("spAddStaff", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@pnr", SqlDbType.VarChar).Value = pnr;
            da.SelectCommand.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            da.SelectCommand.Parameters.Add("@year", SqlDbType.Int).Value = year;
            da.SelectCommand.Parameters.Add("@period", SqlDbType.Int).Value = period;
            da.SelectCommand.Parameters.Add("@hours", SqlDbType.Int).Value = hours;

            SqlParameter success = new SqlParameter();
            success.Direction = ParameterDirection.ReturnValue;
            da.SelectCommand.Parameters.Add(success);
            try
            {
                da.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return 0;
            }
            if ((int)success.Value == 1)
            {
                MessageBox.Show("Error, something went wrong...");
                ClearNumbers();
                con.Close();
                return 0;
            }
            else
            {
                MessageBox.Show("Staff " + pnr + " added");
            }
            con.Close();
            return 1;
        }

        /*
         * Add a labassistant staffing for a course
         * 
         * Parameters:
         *              studid      StudentID for the student to staff
         *              cc          CourseCode of the course on which to add a labassistant
         *              year        The year for the course instance
         *              period      The period for the course instance
         *              hours       The number of hours to staff the student
         *              
         * Return value:
         *              1           Labassistant staffing added
         *              Any other   Error
         */
        public override int addLabass(string studid, string cc, int year, int period, int hours, int salary)
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("spAddLabbass", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@studid", SqlDbType.VarChar).Value = studid;
            da.SelectCommand.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            da.SelectCommand.Parameters.Add("@year", SqlDbType.Int).Value = year;
            da.SelectCommand.Parameters.Add("@period", SqlDbType.Int).Value = period;
            da.SelectCommand.Parameters.Add("@hours", SqlDbType.Int).Value = hours;
            da.SelectCommand.Parameters.Add("@salary", SqlDbType.Int).Value = salary;


            SqlParameter success = new SqlParameter();
            success.Direction = ParameterDirection.ReturnValue;
            da.SelectCommand.Parameters.Add(success);
            try
            {
                da.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("Error, something went wrong...");
                con.Close();
                return 0;
            }
            if ((int)success.Value == 1)
            {
                MessageBox.Show("Error, something went wrong...");
                con.Close();
                return 0;
            }
            else
            {
                MessageBox.Show("Labbass " + studid + " added");
            }
            con.Close();
            return 1;
        }


        /*
         * Add a new course
         * 
         * Parameters:
         *              cc          CourseCode of the course on which to add a labassistant
         *              name        The name of the course
         *              credits     The number of credits for the course
         *              responsible The "personnummer" of the course responsible staff
         *              
         * Return value:
         *              1           Course added
         *              Any other   Error
         */

        ////////////
        /// ////////////
        /// ////////////
        public override int addCourse(string cc, string name, double credits, string responsible)
        {
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("spAddCourse", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            da.SelectCommand.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            da.SelectCommand.Parameters.Add("@credits", SqlDbType.Float).Value = credits;
            da.SelectCommand.Parameters.Add("@responsible", SqlDbType.VarChar).Value = responsible;

            SqlParameter returnParam = da.SelectCommand.Parameters.Add("@Success", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;
            int Success;
            try
            {
                da.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                con.Close();
                return 0;
            }
            Success = (int)returnParam.Value;

            if (Success == 1)
            {
                con.Close();
                return 0;
            }
           
            con.Close();
            return 1;
        }


        /********************************************************************************************
         * DATABASE QUERYING METHODS
         *******************************************************************************************/

        /*
         * Get student data for all students
         * 
         * Parameters
         *              None
         * 
         * Return value:
         *              DataTable with the following columns:
         *                  StudentID       VARCHAR     StudentID for Students
         *                  FirstName       VARCHAR     Students First Name
         *                  LastName        VARCHAR     Students Last Name
         *                  Gender          VARCHAR     Students Gender
         *                  StreetAdress    VARCHAR     Students StreetAdress
         *                  ZipCode         VARCHAR     Students "PostNummer"
         *                  BirthDate       DATETIME    Students BirthDate
         *                  StudentType     VARCHAR     Student type (Program Student, Exchange Student etc)
         *                  City            VARCHAR     Students City
         *                  Country         VARCHAR     Students Country
         *                  program         VARCHAR     Name of the program the student is enrolled to
         *                  PgmStartYear    INTEGER     Year the student enrolled to the program
         *                  credits         FLOAT       The number of credits that the student has completed
         */
        public override DataTable getStudentData()
        {
            //Dummy code - Remove!
            //Please note that you do not use DataTables like this at all when you are using a database!!
            try
            {
                con.Open();
                string query = "select * from VwGetStudents";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("CATCHBLOCK");
                con.Close();

                return null;
            }
        }

        /*
         * Get list of staff
         * 
         * Parameters
         *              None
         *
         * Return value
         *              DataTable with the following columns:
         *                  pnr             VARCHAR     "Personnummer" for the staff
         *                  fullname        VARCHAR     Full name (First name and Last Name) of the staff.
         */
        public override DataTable getStaff()
        {
            try
            {
                con.Open();
                string query = "select * from VwGetStaff";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("CATCHBLOCK");
                con.Close();
                return null;
            }
        }

        /*
         * Get list of Potential Labasses (i.e. students)
         * 
         * Parameters
         *              None
         *
         * Return value
         *              DataTable with the following columns:
         *                  StudentID       VARCHAR     StudentID for all students
         *                  fullname        VARCHAR     Full name (First name and Last Name) of the students.
         */
        public override DataTable getLabasses()
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "select * from VwGetLabb";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("CATCHBLOCK");
                con.Close();
                return null;
            }
        }

        /*
         * Get course data
         * 
         * Parameters
         *              None
         * 
         * Return value
         *              DataTable with the following columns:
         *                  coursecode      VARCHAR     Course Code
         *                  name            VARCHAR     Name of the Course
         *                  credits         FLOAT       Credits of the course
         *                  courseresponsible VARCHAR   "Personnummer" for the course responsible teacher
         */
        public override DataTable getCourses()
        {
            try
            {
                con.Open();
                string query = "select * from VwGetCourses";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("CATCHBLOCK");
                con.Close();
                return null;
            }
        }
        /*
         * Returns the salary costs for a course instance based on the teacher and lab assistent staffing.
         * 
         * Parameters:
         *              cc          CourseCode to the course to calculate the cost
         *              year        The year for the course instance
         *              period      The period for the course instance
         *              
         * Return value:
         *              integer     The cost in currency (SEK)
         */
        public override int getCourseCost(string cc, int year, int period)
        {
            //SCALAR FUNCTION THAT RETURNS ONLY ONE VALUE
            ////NEED TO FIX CASTINGBUG

            var functionquery = $"Select dbo.Get_Course_Cost('{cc}', {year}, {period})";
            SqlCommand com = new SqlCommand(functionquery, con);
            con.Open();
            com.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            com.Parameters.Add("@year", SqlDbType.Int).Value = year;
            com.Parameters.Add("@period", SqlDbType.Int).Value = period;

            try
            {
                var cost  = (int)com.ExecuteScalar();
                con.Close();
                return cost;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                con.Close();
                return 0;
            }
        }

        /*
         * Returns the staffed persons (both teachers and lab assistants) for a course instance
         * 
         * Parameters:
         *              cc          CourseCode to the course to show staffing for
         *              year        The year for the course instance
         *              period      The period for the course instance
         *              
         * Return value:
         *              DataTable with the relevant information
         *                  The table should show name, number of hours, the Task in the course and the hourly salary
         */
        public override DataTable getCourseStaffing(string cc, string year, string period)
        {
            //Table inline function

            con.Open();
            var functionquery = $"SELECT * FROM Get_Course_Staffing('{cc}', '{year}', '{period}')";
            SqlCommand com = new SqlCommand(functionquery, con);
            com.CommandType = CommandType.Text;
            com.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;
            com.Parameters.Add("@year", SqlDbType.VarChar).Value = year;
            com.Parameters.Add("@period", SqlDbType.VarChar).Value = period;

            SqlDataReader datareader = com.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(datareader);
            con.Close();
            return datatable;
        }

        /*
         * Returns the student course transcript ("Ladokudrag")
         * 
         * Parameters:
         *              studId      StudentID for student to show transcript for
         *              
         * Return value:
         *              DataTable with the relevant information
         *                  See lab-instructions for more information about this DataTable
         */
        public override DataTable getStudentRecord(string studId)
        {
            //Table inline function

            con.Open();
            var functionquery = $"SELECT * FROM Get_Student_Records({studId})";
            SqlCommand com = new SqlCommand(functionquery, con);
            com.CommandType = CommandType.Text;
            com.Parameters.Add("@StudID", SqlDbType.VarChar).Value = studId;

            SqlDataReader datareader = com.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(datareader);
            con.Close();
            return dt;

        }

        /*
         * Returns the a list of all courses that are prerequisites to a course.
         * 
         * Parameters:
         *              cc      Course Code for the course to list prerequisites
         *              
         * Return value:
         *              DataTable with the relevant information
         *                  The Table should show at least coursecode and course name for all prerequisite courses
         */
        public override DataTable getPreReqs(string cc)
        {
            //Table inline function
            con.Open();
            var functionquery = $"SELECT * FROM Get_Course_Prereq('{cc}')";
            SqlCommand com = new SqlCommand(functionquery, con);
            com.CommandType = CommandType.Text;
            com.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;

            SqlDataReader datareader = com.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(datareader);
            con.Close();
            return dt;
        }


        /*
         * Get course instances for a course
         * 
         * Parameters
         *              cc      Course Code for the course to list course instances
         * 
         * Return value
         *              DataTable with the following columns:
         *                  year            INTEGER     The year of the course instance
         *                  period          INTEGER     The period of the course instance
         *                  instance        VARCHAR     The "Display text" for the instance, e.g. year(period) or similar
         */
        public override DataTable getInstances(string cc)
        {
            //Table inline function
            con.Open();
            var functionquery = $"SELECT * FROM Get_Course_Instance('{cc}')";
            SqlCommand com = new SqlCommand(functionquery, con);
            com.CommandType = CommandType.Text;
            com.Parameters.Add("@cc", SqlDbType.VarChar).Value = cc;

            SqlDataReader datareader = com.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(datareader);
            con.Close();
            return datatable;
        }

        /*
        * Get list of telephone numbers for a student
        * 
        * Parameters
        *              studId      StudentID for the student
        * 
        * Return value
        *              DataTable with the following columns:
        *                  Type            VARCHAR     The type of telephone number (e.g., Home, Work, Cell etc.)
        *                  Number          VARCHAR     The telephone number
        */
        public override DataTable getStudentPhoneNumbers(string studId)
        {
            //Table inline function
            con.Open();
            var functionquery = $"SELECT * FROM Get_Student_PhoneNrs('{studId}')";
            SqlCommand com = new SqlCommand(functionquery, con);
            com.CommandType = CommandType.Text;
            com.Parameters.Add("@StudID", SqlDbType.VarChar).Value = studId;

            SqlDataReader datareader = com.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(datareader);
            con.Close();
            return datatable;
        }

        /*
        --------------------------------------------------------------------------------------------
         STUB IMPLEMENTATIONS TO BE USED IN LAB 4. 
        --------------------------------------------------------------------------------------------
        */


        /*
        * Get list years which have course instances
        * 
        * Parameters
        *              None      
        * 
        * Return value
        *              DataTable with the following column:
        *                  Year            INTEGER     A unique (no duplicates) list of all years which has course instances
        */
        public override DataTable getStaffingYears()
        {
            //Dummy code - Remove!
            //Please note that you do not use DataTables like this at all when you are using a database!!
            DataTable dt = new DataTable();
            dt.Columns.Add("Year");
            dt.Rows.Add(2000);
            return dt;
        }

        /*
        * Get a matrix of all staffing for a year
        * 
        * Parameters
        *              year     The year to show staffings for      
        * 
        * Return value
        *              DataTable with suitable format
        *                  For more information about the format, see Lab instructions for lab 4
        */
        public override DataTable getStaffingGrid(string year)
        {
            //Dummy code - Remove!
            //Please note that you do not use DataTables like this at all when you are using a database!!
            DataTable dt = new DataTable();
            dt.Columns.Add("StaffingGrid");
            dt.Rows.Add("All will be revealed in lab 4.. :)");
            return dt;
        }
    }
}
