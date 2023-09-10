using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
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
using Poprizenok.model;

namespace Poprizenok
{
    /// <summary>
    /// Логика взаимодействия для EditAgentPage.xaml
    /// </summary>
    public partial class EditAgentPage : Page
    {
        private Agent agent;
        private int curSelPr = 0;
        private int curTypAg = 0;

        public EditAgentPage(Agent ag)
        {
            InitializeComponent();
            try
            {
                Type.ItemsSource = PoprizenokEntities.GetContext().AgentType.ToList();
                product.ItemsSource = PoprizenokEntities.GetContext().Product.ToList();
            }
            catch { };
            if (ag != null)
            {
                agent = ag;
                Type.SelectedItem = ag.AgentType;
                this.Title.Text = ag.Title;
                this.Adress.Text = ag.Address;
                this.Inn.Text = ag.INN;
                this.Kpp.Text = ag.KPP;
                this.Director.Text = ag.DirectorName;
                this.Phone.Text = ag.Phone;
                this.Prioritet.Text = ag.Priority.ToString();
                historyGrid.ItemsSource = PoprizenokEntities.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == ag.ID).ToList();
            }
            else
            {
                agent = new Agent();
                btnDelAg.IsEnabled = false;
                btnWritHi.IsEnabled = false;
                btnDelHi.IsEnabled = false;
            }
            this.DataContext = agent;

        }

        private void btnWriteAg_Click(object sender, RoutedEventArgs e)
        {
            if (curTypAg == 0) return;
            if (this.Title.Text == "") return;
            if (!(new Regex(@"\d{10}|\d{12}")).IsMatch(this.Inn.Text)) return;
            if (!(new Regex(@"\d{4}[\dA-Z][\dA-Z]\d{3}")).IsMatch(this.Kpp.Text)) return;
            if ((this.Email.Text != "") && (!(new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)")).IsMatch(this.Email.Text))) return;
            agent.Title = this.Title.Text;
            agent.AgentTypeID = curTypAg;
            agent.Address = this.Adress.Text;
            agent.INN = this.Inn.Text;
            agent.KPP = this.Kpp.Text;
            agent.Phone = this.Phone.Text;
            agent.DirectorName = this.Director.Text;
            agent.Phone = this.Phone.Text;
            agent.Email = this.Email.Text;
            try
            {
                agent.Priority = Convert.ToInt32(this.Prioritet.Text);
            }
            catch
            {
                return;
            }
            try
            {
                if (agent.ID > 0)
                {
                    PoprizenokEntities.GetContext().Entry(agent).State = EntityState.Modified;
                    PoprizenokEntities.GetContext().SaveChanges();
                    MessageBox.Show("Обновление информации об агенте завершено");
                }
                else
                {
                    PoprizenokEntities.GetContext().Agent.Add(agent);
                    PoprizenokEntities.GetContext().SaveChanges();
                    MessageBox.Show("Добавление информации об агенте завершено");
                }
            }
            catch { };
            btnDelAg.IsEnabled = true;
            btnWritHi.IsEnabled = true;
            btnDelHi.IsEnabled = true;

        }

        private void btnDelAg_Click(object sender, RoutedEventArgs e)
        {
            if (agent.ProductSale.Count > 0)
            {
                MessageBox.Show("Удаление не возможно!");
                return;
            }
            foreach (Shop shop in agent.Shop)
            {
                PoprizenokEntities.GetContext().Shop.Remove(shop);
            }
            foreach (AgentPriorityHistory apr in agent.AgentPriorityHistory)
            {
                PoprizenokEntities.GetContext().AgentPriorityHistory.Remove(apr);
            }
            PoprizenokEntities.GetContext().Agent.Remove(agent);
            PoprizenokEntities.GetContext().SaveChanges();
            MessageBox.Show("Удаление информации об агенте завешено!");
            this.NavigationService.GoBack();

        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curTypAg = ((AgentType)Type.SelectedItem).ID;
        }

        private void historyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curSelPr = ((Product)product.SelectedItem).ID;
        }

        private void btnWritHi_Click(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = Convert.ToInt32(count.Text);
            }
            catch
            {
                return;
            }
            string dt = date.ToString();
            if (curSelPr > 0 && dt != "" && cnt > 0)
            {
                ProductSale pr = new ProductSale();
                pr.AgentID = agent.ID;
                pr.ProductID = curSelPr;
                pr.SaleDate = (DateTime)date.SelectedDate;
                pr.ProductCount = cnt;
                try
                {
                    PoprizenokEntities.GetContext().ProductSale.Add(pr);
                    PoprizenokEntities.GetContext().SaveChanges();
                    historyGrid.ItemsSource = PoprizenokEntities.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == agent.ID).ToList();
                }
                catch
                {
                    return;
                }
            }
        }
        private void btnDelHi_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < historyGrid.SelectedItems.Count; i++)
            {
                ProductSale prs = historyGrid.SelectedItems[i] as ProductSale;
                if (prs != null)
                {
                    PoprizenokEntities.GetContext().ProductSale.Remove(prs);
                }
            }
            try
            {
                PoprizenokEntities.GetContext().SaveChanges();
                historyGrid.ItemsSource = PoprizenokEntities.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == agent.ID).ToList();
            }
            catch { return; };

        }

        private void mask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fnd = ((TextBox)sender).Text;
            try
            {
                product.ItemsSource = PoprizenokEntities.GetContext().Product.Where(Product => Product.Title.Contains(fnd)).ToList();
            }
            catch { };

        }
    }
}
