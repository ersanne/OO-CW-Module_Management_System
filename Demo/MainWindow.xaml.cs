using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using BusinessObjects;

namespace Demo
{
    /// Author: Erik Sanne
    /// Desc: Code for MainWindow Interaction.
    /// Last modified by Erik Sanne on 24/10/2017
    /// During the presentation Delete would not remove the student from dataList thus the ListAll was not working properly.
    /// I have since fixed that issue.
    public partial class MainWindow : Window
    {
        private ModuleList store = new ModuleList(); //Creating store
        private double _cwMark; //Coursework Mark as double for higher accurace
        private double _examMark; //Exam Mark as double for higher accurace
        private ObservableCollection<Student> dataList = new ObservableCollection<Student>(); //ObservableCollection to Display All students in a DataGrid
        
        //Creating main window
        public MainWindow()
        {
            InitializeComponent();
        }

        //Event trigger for Add Button
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            //Creating new student
            Student st = new Student();

            //Setting Matriculation Number
            //The matric will only be auto generated if the text box is empty
            try
            {

                int matric;

                if (!string.IsNullOrEmpty(matricBox.Text))//Check if matric box is empty
                {
                    matric = GetMatricInt(); //Get matric number as int

                    //Check if matric already exists
                    if (store.find(matric) == null)
                    {
                        st.Matric = matric; //If matric is available, set matric
                    }
                    else
                    {
                        //Error if the Matric number already exists
                        throw new ArgumentException("Matric Number already exists");
                    }

                }
                else
                {
                    //Auto generation 

                    //Base matric value
                    matric = 10001;

                    //Comparing to ListBox Items as these are sorted. This will find the first free matric number
                    //If studen details were kept after deletion this would not work
                    foreach (int tmpmatric in matricListBox.Items)
                    {
                        if (tmpmatric == matric) matric = matric + 1; //If the value already exists iterate by 1
                    }

                    //Add the new matric num
                    st.Matric = matric;
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Setting First Name
            try
            {
                st.FName = fNameBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }
            //Setting Last Name
            try
            {
                st.SName = sNameBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Setting Coursework Mark
            try
            {
                //Checking that value is numeric and can be parsed to double
                if (!double.TryParse(cwMarkBox.Text, out _cwMark))
                {
                    MessageBox.Show("Please enter a numeric value."); //Error if cwMark is not numeric
                    cwMarkBox.Text = string.Empty; //Reset cwMark field
                    return; //Stop execution
                } 
                
                st.CwMark = _cwMark;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Setting Exam Mark
            try
            {

                //Checking that value is numeric and can be parsed to double
                if (!double.TryParse(examMarkBox.Text, out _examMark))
                {
                    MessageBox.Show("Please enter a numeric value."); //Error if examMark is not numeric
                    examMarkBox.Text = string.Empty; //Reset examMark field
                    return; //Stop execution
                } 

                st.ExamMark = _examMark;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Setting Birth Date
            try
            {
                st.BirthDate = birthDatePicker.SelectedDate.GetValueOrDefault(); //Get value of DatePicker if empty set to default which will trigger the blank error
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            store.add(st); //Add student to store
            int i = GetListIndex(st.Matric); //Get the index for current matric num
            matricListBox.Items.Insert(i, st.Matric); //Add Matric to the ListBox at its index (For numerical sorting)
            dataList.Insert(i, st); //Add student to ObservableCollection for List all

            //Reset fields after adding student
            ResetFields();
            

            
        }

        //Finding the student with the given Matric number
        private void findButton_Click(object sender, RoutedEventArgs e)
        {

            //Check that matriculation number was entered
            if (string.IsNullOrEmpty(matricBox.Text))
            {
                MessageBox.Show("Please enter a Matriculation Number.");
                return;
            }

            try
            {   
                //Find student, if not found error message will be displayed.
                FindAndSet(GetMatricInt());
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
            }
            
                     
            
        }

        //Delete the sudent with the given Matric number
        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            int matric; //Matric to be deleted
            int index; //Index for ListAll

            try
            {
                //Check that matric was entered
                if (string.IsNullOrEmpty(matricBox.Text))
                {
                    throw new ArgumentException("Please enter a Matric number.");
                }

                matric = GetMatricInt(); //Get matric from text field
                index = dataList.IndexOf(store.find(matric)); //Save current index of student in the ListAll collection (so if deletion was successful it can be removed)

                store.delete(matric);//Try deleting the student, if unsuccessful an error message will be displayed, matric will not be deleted from ListBox/ListAll

                if (store.find(matric) != null)
                {
                    throw new ArgumentException("Deletion not successful.");
                }

            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return; //Return so elements are not deleted from ListAll and ListBox
            }

            //Removing the matric from the ListBox
            matricListBox.Items.Remove(matric);
            //Removing the student from ListAll Observable Collection
            dataList.RemoveAt(index);
            //Reset field
            ResetFields();
        }

        //Listing all the Students
        private void listAllButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayAll display = new DisplayAll(); //Generate DisplayAll Window
            display.displayDataGrid.DataContext = dataList; //Set the DataContext for the DataGrid
            display.Show(); //Show the Window
                       
        }

        //Showing info for currently selected matric entry
        private void matricListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int matric;

            if (matricListBox.SelectedItem == null) return; //Check that item still exists (to avoid error on deletion)
            if (!int.TryParse(matricListBox.SelectedItem.ToString(), out matric)) return; //Try parsing matric to int, if failed and error will be displayed

            try
            {
                FindAndSet(matric); //Finding and setting info for the entry
            }
            catch (Exception excep) //Error if the student could not be found
            {
                MessageBox.Show(excep.Message); //Show error
            }

        }

        //Getting the index at which the matric should be placed, this way my ListBox stays sorted
        private int GetListIndex(int matric)
        {
            if (matricListBox.Items.Count == 0) return 0; //For empty list enter at first entry
            int index = 0;

            //If matric is smaller than any entry in the ListBox index will be at that point
            foreach (int tmp in matricListBox.Items)
            {

                if (matric > tmp)
                {
                    index = index + 1;
                }
                else
                {

                    return index;
                }

            }
            //Else return the index after the last ListBox element
            return index;
        }

        //Finding student and setting text fields to display the student's info
        private void FindAndSet(int matric)
        {
            Student st = store.find(matric); //Find Student

            if (st != null) //Check that student was found
            {
                matricBox.Text = st.Matric.ToString(); //Set Matric
                fNameBox.Text = st.FName; //Set First Name
                sNameBox.Text = st.SName; //Set Last Name
                cwMarkBox.Text = st.CwMark.ToString(); //Set Coursework Mark
                examMarkBox.Text = st.ExamMark.ToString(); //Set Exam Mark
                birthDatePicker.SelectedDate = st.BirthDate; //Set Birth Date
            }
            else
            {
                throw new ArgumentException("Can't find student."); //Error if student wasn't found
            }
            
        }

        //Resetting all text box fields
        private void ResetFields()
        {
            matricBox.Text = string.Empty; //Clear matric box
            fNameBox.Text = string.Empty; //Clear first name box
            sNameBox.Text = string.Empty; //Clear surname box
            birthDatePicker.SelectedDate = null; //Clear birthdate picker
            cwMarkBox.Text = string.Empty; //Clear coursework mark box
            examMarkBox.Text = string.Empty; //Clear exam mark box
        }

        //Get matric number as int
        private int GetMatricInt()
        {
            int matric;

            if (!int.TryParse(matricBox.Text, out matric)) //Try parsing matric to int
            {
                matricBox.Text = string.Empty; //Reset matric box
                throw new ArgumentException("Please enter a numeric Matriculation number."); //Error if matric isn't numeric;
                
            }

            return matric; //Return valid matric number
        }
      
    }

}
