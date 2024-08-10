using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public class SubjectMarks
        {
            public string Subject { get; set; }
            public int Marks { get; set; }
        }

        private List<SubjectMarks> subjectMarksList = new List<SubjectMarks>();

        public Form1()
        {
            InitializeComponent();

            dataGridView1.Columns.Add("Subject", "Subject");
            dataGridView1.Columns.Add("Marks", "Marks");
        }

        private readonly string connectionString = "Data Source=VISHWAS\\SQLEXPRESS;Initial Catalog=MaskiriSchool;Integrated Security=True;";

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.SelectedItem == null)
                {
                    MessageBox.Show("Please select a subject.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string subject = comboBox2.SelectedItem.ToString();
                if (!int.TryParse(textBox3.Text, out int marks))
                {
                    MessageBox.Show("Please enter a valid number for marks.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                var existingSubject = subjectMarksList.Find(sm => sm.Subject == subject);

                if (existingSubject != null)
                {
                   
                    existingSubject.Marks = marks;
                }
                else
                {
                    
                    subjectMarksList.Add(new SubjectMarks { Subject = subject, Marks = marks });
                }

                
                dataGridView1.Rows.Clear();
                foreach (var item in subjectMarksList)
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["Subject"].Value = item.Subject;
                    row.Cells["Marks"].Value = item.Marks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      private void button2_Click(object sender, EventArgs e)
{
    try
    {
        
        string Gender = radioButton1.Checked ? "Male" : "Female";

       
        List<string> Skillset = new List<string>();
        if (checkBox1.Checked) Skillset.Add("Communication Skills");
        if (checkBox2.Checked) Skillset.Add("Leadership");
        if (checkBox3.Checked) Skillset.Add("Operating System");
        if (checkBox4.Checked) Skillset.Add("Maths");
        string Skills = string.Join(",", Skillset);

        
        int mathsMarks = 0, cLangMarks = 0, physicsMarks = 0;

       
        foreach (var subjectMarks in subjectMarksList)
        {
           
            switch (subjectMarks.Subject)
            {
                case "Engineering Maths":
                    mathsMarks = subjectMarks.Marks;
                    break;
                case "C Language":
                    cLangMarks = subjectMarks.Marks;
                    break;
                case "Physics":
                    physicsMarks = subjectMarks.Marks;
                    break;
            }
        }

        
        

        
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("Sp_Insert", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

               
                cmd.Parameters.AddWithValue("@Name", textBox1.Text);
                cmd.Parameters.AddWithValue("@Age", float.Parse(textBox2.Text));
                cmd.Parameters.AddWithValue("@Gender", Gender);
                cmd.Parameters.AddWithValue("@Department", comboBox1.Text);
                cmd.Parameters.AddWithValue("@DOB", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@Skillset", Skills);
                foreach (var subjectMarks in subjectMarksList)
                {
                     string parameterName = $"@{subjectMarks.Subject.Replace(" ", "")}Marks";
                     cmd.Parameters.AddWithValue(parameterName, subjectMarks.Marks);
                }
                        cmd.ExecuteNonQuery();
            }
        }

       
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex("^[a-zA-Z]*$");


            if (!regex.IsMatch(textBox1.Text))
            {
                MessageBox.Show("Please enter only alphabets.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);


                textBox1.SelectionStart = textBox1.Text.Length;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out int age))
            {
                MessageBox.Show("Please enter only numeric values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Text = string.Empty;
                return;
            }


            if (textBox2.Text.Length > 3)
            {
                MessageBox.Show("Age should not be more than 3 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Text = string.Empty;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //if (!int.TryParse(textBox3.Text, out int age))
            //{
            //    MessageBox.Show("Please enter only numeric values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    textBox3.Text = string.Empty;
            //    return;
            //}


            //if (textBox3.Text.Length > 3)
            //{
            //    MessageBox.Show("Age should not be more than 3 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    textBox3.Text = string.Empty;
            //}
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Today;


            DateTime selectedDate = dateTimePicker1.Value.Date;


            if (selectedDate > currentDate)
            {

                dateTimePicker1.Value = currentDate;


                MessageBox.Show("Please select a date up to the current date only.", "Invalid Date Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}