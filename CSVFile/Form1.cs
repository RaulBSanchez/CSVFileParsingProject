using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSVFile.Properties;

namespace CSVFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void CSVButton_Click(object sender, EventArgs e)
        {
            //Makre sure user wants to upload CSV Files
            const string message = "Do you want to upload the CSV Files from the Current Directory";
            const string caption = "Upload CSV: ";

            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //Take Result of Y/N
            if (result == DialogResult.Yes)
            {
                try
                {
                    String FilePath = @"" + textBox4.Text + "";
                    //Make sure path exists
                    if (!Directory.Exists(FilePath))
                    {
                        MessageBox.Show("Directory does not exist, enter valid path");
                    }

                    else
                    {

                        if (Directory.GetFiles(FilePath).Length == 0)
                        {
                            MessageBox.Show("Folder is Empty");

                        }


                        else
                        {

                            string[] csvFiles = System.IO.Directory.GetFiles(FilePath, "*.csv").Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();



                            //Create an Object for each String in Array
                            //Set object to equal the File Information


                            SqlConnection conn = new SqlConnection(@"Connection String");
                            conn.Open();
                            SqlCommand command = new SqlCommand("SELECT * FROM TitleOfBlog", conn);


                            foreach (string item in csvFiles)
                            {
                                //local variables for each column

                                String temp = item;
                                String tempath = FilePath + item;

                               

                                //Split Title into four strings, set date to tempdate for modification
                                //Put strings into array. 
                                String[] splitTitle = temp.Split('_');
                                String tempDate = splitTitle[2];

                                //Modify Date for SQL Database. 

                                DateTime dt = DateTime.ParseExact(tempDate, "MM-dd-yyyy", null);


                                //Create an object for the tile of the blog. 
                                DataClasses1DataContext db = new DataClasses1DataContext();

                                TitleOfBlog entry = new TitleOfBlog
                                {

                                    Title = splitTitle[0],
                                    Category = splitTitle[1],
                                    Date = dt,
                                    InteractionType = splitTitle[3]

                                };


                                db.TitleOfBlogs.InsertOnSubmit(entry);

                                try
                                {
                                    db.SubmitChanges();
                                    MessageBox.Show(splitTitle[0] + ' ' + splitTitle[3] + " was submitted successfully");

                                    conn.Close();
                                    string newFolder = @"Folder to move files to";

                                    DirectoryInfo dirInfo = new DirectoryInfo(newFolder);
                                    if (dirInfo.Exists == false) Directory.CreateDirectory(newFolder);

                                    List<String> csv = Directory.GetFiles(FilePath, ".", SearchOption.AllDirectories).ToList();

                                    foreach (string file in csv)
                                    {
                                        FileInfo mFile = new FileInfo(file);

                                        //remove collisions
                                        if (new FileInfo(dirInfo + "\\" + mFile.Name).Exists == false)
                                        {
                                            mFile.MoveTo(dirInfo + "\\" + mFile.Name);
                                        }
                                    }

                                    //Show if the information and passed and files were moved. 
                                    MessageBox.Show("Files were moved to the back up folder: " + newFolder);
                                }

                                //Error Message. 
                                catch
                                {
                                    MessageBox.Show("Databse was not updated, file was not moved");
                                }



                            }

                        }

                    }

                }




                //Makes sure the files are in the correct format, or else throw an error.
                catch

                {
                    MessageBox.Show("Please make sure, the directory are just CSV files in the correct format and try again");
                }
             }

            //Do nothing if No is selected. 
            else if (result == DialogResult.No)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //button
        }
    }
}
