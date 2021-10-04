using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace prelimthing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool fileLocked = false; // this flag is false when there are no files open, it is true when there is a file open
        CheckBox dynamicCB = new CheckBox();
        CheckBox[] cblist = new CheckBox[] { };
        string[] words = new string[] { };
        int iclick = 0;
        int uclick = 0;
        int dclick = 0;
        ToolTip t1 = new ToolTip();
        ToolTip t2 = new ToolTip();
        ToolTip t3 = new ToolTip();
        string columnset;
        string columnwhere;
        string a = "";
        string tempset = "";
        string tempwhere = "";

        public MainWindow()
        {
            InitializeComponent();
            t1.Content = "Add Insert script";
            insertbtn.ToolTip = t1;

            t2.Content = "Add Update script";
            updatebtn.ToolTip = t2;

            t3.Content = "Add Delete script";
            deletebtn.ToolTip = t3;
        }

        private void btnCSVSelector_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Comma Separated Values (*.csv;)|*.csv;";

            if (ofd.ShowDialog() == true)
            {
                tbDisplayPath.Text = ofd.FileName;
            }

            if (fileLocked) // file is about to be released
            {

            }
            else // file is about to be open
            {
                if (tbDisplayPath.Text.Length > 0) // file opening and reading should only occur if there is a file path on display
                {
                    FileManipulator.fileReader(tbDisplayPath.Text);
                    MessageBox.Show(FileManipulator.fileStatus()[1]);
                    if (FileManipulator.fileStatus()[0] == "1") // file reading success, proceed to next step
                    {
                        fileLocked = true;
                        btnCSVSelector.IsEnabled = false;
                        btnGenerateScript.IsEnabled = true;
                        insertbtn.IsEnabled = true;
                        updatebtn.IsEnabled = true;
                        deletebtn.IsEnabled = true;
                        lblDBName.Content = "Database Name: " + FileManipulator.fileGetDBName();
                        lblTableName.Content = "Table Name: " + FileManipulator.fileGetTableName();
                        FileManipulator.fileExtractColumnNames();
                        lblColumnCount.Content = "Column Count: " + FileManipulator.getColumnCount();
                        FileManipulator.fileCreator();

                        string[] temp = FileManipulator.stringSplitter(FileManipulator.lines[2], new char[] { ',' });

                        int num = 10;
                        cblist = new CheckBox[temp.Length];
                        for (int i = 0; i < temp.Length; i++)
                        {
                            dynamicCB = new CheckBox();
                            dynamicCB.FlowDirection = FlowDirection.LeftToRight;
                            dynamicCB.Content = temp[i];
                            //dynamicCB.Margin = new System.Windows.Thickness { Left = 224, Top = 180 + num, Right = 0, Bottom = 0 };
                            dynamicCB.HorizontalAlignment = HorizontalAlignment.Left;

                            dynamicCB.IsChecked = true;

                            cblist[i] = dynamicCB;
                            setv.Items.Add(cblist[i].Content);
                            wherev.Items.Add(cblist[i].Content);
                            //DisplayGrid.Children.Add(dynamicCB);
                            dynamicCB.Click += new RoutedEventHandler(dynamicCB_Checked);
                            LB.Items.Add(dynamicCB);

                            num += 20;
                        }

                    }
                }
            }
        }

        private void dynamicCB_Checked(object sender, EventArgs e)
        {
            dynamicCB = (sender as CheckBox);
            //MessageBox.Show(dynamicCB.Content.ToString());
            string[] sample = new string[] { };
            FileManipulator.samplescriptthingy();

            string b = "";
            string c = "";

            sample = FileManipulator.stringSplitter(FileManipulator.TRY[0], new char[] { ',' });
            b = "";
            for (int j = 0; j < sample.Length; j++)
            {
                if ((bool)cblist[j].IsChecked)
                {
                    b += "'" + sample[j] + "'" + ",";
                    c = b.TrimEnd(',');
                }
                else
                {
                    b += sample[j] + ",";
                    c = b.TrimEnd(',');
                }
            }
            a = "INSERT INTO [" + FileManipulator.fileGetDBName() + "].dbo."
                + FileManipulator.fileGetTableName() + "\nVALUES (" + c + ")";

            insertsample.Text = "";
            if (iclick % 2 != 0)
            {
                insertsample.Text = a;
            }
            else
            {
                insertsample.Text = "";
            }
        }

        private void btnGenerateScript_Click(object sender, RoutedEventArgs e)
        {
            // Pre Diagnostic scripts
                //FileManipulator.fileWriter(true, "-- PRE DIAGNOSTIC SCRIPTS");
            FileManipulator.fileWriter(true, "SELECT COUNT(*) FROM [" + FileManipulator.fileGetDBName() + "].dbo." + FileManipulator.fileGetTableName());
            FileManipulator.fileWriter(true, "SELECT * FROM [" + FileManipulator.fileGetDBName() + "].dbo." + FileManipulator.fileGetTableName());
            FileManipulator.fileWriter(true, "");

            // Generated scripts
            FileManipulator.thescriptthingy();

            string[] sample = new string[] { };
            string var1 = "";
            string var2 = "";
            string beans = "";

            for (int i = 0; i < FileManipulator.plswork.Count; i++)
            {
                words = FileManipulator.stringSplitter(FileManipulator.plswork[i], new char[] { ',' });
                var2 = "";
                for (int j = 0; j < words.Length; j++)
                {
                    if ((bool)cblist[j].IsChecked)
                    {
                        var2 += "'" + words[j] + "'" + ",";
                        beans = var2.TrimEnd(',');
                    }
                    else
                    {
                        var2 += words[j] + ",";
                        beans = var2.TrimEnd(',');
                    }
                }
                var1 += "INSERT INTO [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\nVALUES (" + beans + ")\n";
            }

            if (iclick % 2 != 0)
            {
                FileManipulator.fileWriter(true, var1);
            }

            if (uclick % 2 != 0)
            {
                //if(whereval.Text != "")
                //{
                //    FileManipulator.fileWriter(true, "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                //    + FileManipulator.fileGetTableName() + "\n" + columnset + tempset + "\n"
                //    + columnwhere + tempwhere + "\n");
                //}
                //else
                //{
                //    FileManipulator.fileWriter(true, "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                //    + FileManipulator.fileGetTableName() + "\n" + columnset + tempset + "\n");
                //}

                FileManipulator.fileWriter(true, updatesample.Text);
            }

            if (dclick % 2 != 0)
            {
                FileManipulator.fileWriter(true, "DELETE FROM [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n");
            }

            // Post Diagnostic scripts
            //FileManipulator.fileWriter(true, "-- POST DIAGNOSTIC SCRIPTS");
            FileManipulator.fileWriter(true, "SELECT COUNT(*) FROM [" + FileManipulator.fileGetDBName() + "].dbo." + FileManipulator.fileGetTableName());
            FileManipulator.fileWriter(true, "SELECT * FROM [" + FileManipulator.fileGetDBName() + "].dbo." + FileManipulator.fileGetTableName());
            FileManipulator.fileWriter(true, "");

            MessageBox.Show("File Writing successfully done!");
        }

        private void insertbtn_Click(object sender, RoutedEventArgs e)
        {
            iclick += 1;

            if(iclick % 2 == 0)
            {
                insertbtn.Background = Brushes.White;
                t1.Content = "Add Insert script";
                insertbtn.ToolTip = t1;

                //insertsample.Text = "";
            }
            else
            {
                insertbtn.Background = new SolidColorBrush(Color.FromRgb(251, 247, 213));
                t1.Content = "Remove Insert script";
                insertbtn.ToolTip = t1;

                //insertsample.Text = a;
            }
        }

        private void updatebtn_Click(object sender, RoutedEventArgs e)
        {
            uclick += 1;

            if (uclick % 2 == 0)
            {
                updatebtn.Background = Brushes.White;
                t2.Content = "Add Update script";
                updatebtn.ToolTip = t2;

                setval.Text = "";
                whereval.Text = "";
            }
            else
            {
                updatebtn.Background = new SolidColorBrush(Color.FromRgb(251, 247, 213));
                t2.Content = "Remove Update script";
                updatebtn.ToolTip = t2;
            }
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            dclick += 1;

            if (dclick % 2 == 0)
            {
                deletebtn.Background = Brushes.White;
                t3.Content = "Add Delete script";
                deletebtn.ToolTip = t3;

                deletesample.Text = "";
            }
            else
            {
                deletebtn.Background = new SolidColorBrush(Color.FromRgb(251, 247, 213));
                t3.Content = "Remove Delete script";
                deletebtn.ToolTip = t3;

                deletesample.Text = "DELETE FROM [" + FileManipulator.fileGetDBName() + "].dbo." + FileManipulator.fileGetTableName();
            }
        }

        private void setv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.setv.SelectedIndex >= 0)
            {
                string entry = setv.SelectedItem.ToString();
                columnset = "SET " + entry + " = ";
            }

            if (uclick % 2 != 0)
            {
                if (whereval.Text != "")
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + setval.Text + "\n"
                    + columnwhere + whereval.Text + "\n";
                }
                else
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + setval.Text + "\n";
                }
            }
            else
            {
                updatesample.Text = "";
            }
        }

        private void wherev_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.wherev.SelectedIndex >= 0)
            {
                string entry = wherev.SelectedItem.ToString();
                columnwhere = "WHERE " + entry + " = ";
            }
            else
            {
                columnwhere = "\n";
            }

            if (uclick % 2 != 0)
            {
                if (whereval.Text != "")
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + setval.Text + "\n"
                    + columnwhere + whereval.Text + "\n";
                }
                else
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + setval.Text + "\n";
                }
            }
        }

        private void setval_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((bool)cblist[setv.SelectedIndex].IsChecked)
            {
                tempset = "'" + setval.Text + "'";
            }
            else
            {
                tempset = setval.Text;
            }

            if (uclick % 2 != 0)
            {
                if (whereval.Text != "")
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + tempset + "\n"
                    + columnwhere + tempwhere + "\n";
                }
                else
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + tempset + "\n";
                }
            }
            else
            {
                updatesample.Text = "";
            }
        }

        private void whereval_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((bool)cblist[wherev.SelectedIndex].IsChecked)
            {
                tempwhere = "'" + whereval.Text + "'";
            }
            else
            {
                tempwhere = whereval.Text;
            }

            if (uclick % 2 != 0)
            {
                if (whereval.Text != "")
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + tempset + "\n"
                    + columnwhere + tempwhere + "\n";
                }
                else
                {
                    updatesample.Text = "UPDATE [" + FileManipulator.fileGetDBName() + "].dbo."
                    + FileManipulator.fileGetTableName() + "\n" + columnset + tempset + "\n";
                }
            }
            else
            {
                updatesample.Text = "";
            }
        }

        private void resetbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
