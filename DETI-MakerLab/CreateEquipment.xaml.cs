﻿using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DETI_MakerLab
{
    /// <summary>
    /// Interaction logic for CreateEquipment.xaml
    /// </summary>
    public partial class CreateEquipment : Page
    {
        private SqlConnection cn;
        private String fileName;
        private int _staffID;

        public int StaffID
        {
            get { return _staffID; }
            set { _staffID = value; }
        }

        public CreateEquipment()
        {
            InitializeComponent();
        }

        public CreateEquipment(int StaffID)
        {
            InitializeComponent();
            this.StaffID = StaffID;
        }

        public void SubmitEquipment(String imagePath)
        {
            SqlCommand cmd;
            cn = Helpers.getSGBDConnection();
            if (!Helpers.verifySGBDConnection(cn))
                return;

            cmd = new SqlCommand("INSERT INTO ElectronicResource (ProductName, Manufacturer, Model, ResDescription, EmployeeNum, PathToImage) " +
                "VALUES (@ProductName, @Manufacturer, @Model, @ResDescription, @EmployeeNum, @PathToImage)", cn);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@ProductName", equipment_name.Text);
            cmd.Parameters.AddWithValue("@Manufacturer", equipment_manufacturer.Text);
            cmd.Parameters.AddWithValue("@Model", equipment_model.Text);
            cmd.Parameters.AddWithValue("@ResDescription", equipment_description.Text);
            cmd.Parameters.AddWithValue("@EmployeeNum", StaffID);
            cmd.Parameters.AddWithValue("@PathToImage", imagePath == null ? null : imagePath);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            
        }

        private void upload_image_button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "All Image Files | *.*";
            if (dlg.ShowDialog() == true)
            {
                fileName = dlg.FileName;
                equipment_image.Text = fileName.ToString();
            }
        }

        private void create_equipment_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Copy image to project file and produce its path
                String RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                String name = equipment_name.Text + "_" + equipment_manufacturer.Text + "_" + equipment_model.Text;
                String imagePath = string.Format("{0}images\\", System.IO.Path.GetFullPath(System.IO.Path.Combine(RunningPath, @"..\..\"))) + name + System.IO.Path.GetExtension(fileName);
                System.IO.File.Copy(fileName, imagePath, true);
                SubmitEquipment(imagePath);
                MessageBox.Show("Equipment has been added!");
                StaffWindow window = (StaffWindow)Window.GetWindow(this);
                // TODO : create object and pass it to equipment page
                //window.goToEquipmentPage(equipment);
            }
            catch (SqlException exc)
            {
                Helpers.ShowCustomDialogBox(exc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
