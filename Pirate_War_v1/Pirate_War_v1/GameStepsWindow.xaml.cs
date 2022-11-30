using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Pirate_War_v1
{
    /// <summary>
    /// Interaction logic for GameStepsWindow.xaml
    /// </summary>
    public partial class GameStepsWindow : Window
    {
        public static GameStepsWindow instance;
        public GameStepsWindow()
        {
            InitializeComponent();
            instance = this;
            //listViewer.Items.Add("2: Ai - C2");
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = 0;
            foreach(var step in listViewer.Items)
            {
                if (sender.ToString().Contains(step.ToString()))
                {
                    break;
                }
                index++;
            }
            Torpedo_v_ai.instance.selectedTurnIndex = index;
            Debug.WriteLine(index);
        }
    }
}
