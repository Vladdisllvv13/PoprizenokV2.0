using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using Poprizenok.model;

namespace Poprizenok
{
    /// <summary>
    /// Логика взаимодействия для ChangePriorityWindow.xaml
    /// </summary>
    public partial class ChangePriorityWindow : Window
    {
        private Agent[] _currentaAgents;
        public ChangePriorityWindow(int pr, Agent[] agents)
        {
            _currentaAgents = agents;
            InitializeComponent();
            priorety.Text = pr.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(Agent agent in _currentaAgents)
            {
                agent.Priority = Convert.ToInt32(priorety.Text);
            }
            PoprizenokEntities.GetContext().SaveChanges();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
