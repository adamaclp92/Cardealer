using System;
using Grpc.Core;
using Cardealer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        const string target = "127.0.0.1:50051";
        static Channel channel = new Channel(target, ChannelCredentials.Insecure);

        static CarDealing.CarDealingClient client = new CarDealing.CarDealingClient(channel);

        Session_Id guid = new Session_Id();
        static List<string> cars = new List<string>();

        public Form1()
        {
            InitializeComponent();
            button2.Hide();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            label17.Text = "";
            label18.Text = "";
            label19.Text = "";
            label20.Text = "";
            label21.Text = "";

            var response = client.ListCars(new ListCarsRequest() { Uid = guid.Id });

            while (await response.ResponseStream.MoveNext())
            {
                listBox1.Items.Add(response.ResponseStream.Current.Car.Numberplate.ToString());

            }

            var balance = client.Balance(new BalanceRequest() { Uid = guid.Id });
            label6.Text = balance.Balance.ToString() + " Ft";
        }

        /// <summary>
        /// login kész!
        /// </summary>

        private void button1_Click(object sender, EventArgs e)
        {
            User user = new User() { Username = textBox1.Text,
                                     Password = textBox2.Text};
            guid = client.Login(user);

            if (String.IsNullOrWhiteSpace(textBox1.Text) || String.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show(guid.Message, "Sikertelen bejelentkezés", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else {
                if (guid.Id == "")
                    MessageBox.Show(guid.Message, "Sikertelen bejelentkezés", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    label16.Text = "Bejelentkezve: " + textBox1.Text;
                    textBox1.Hide();
                    textBox2.Hide();
                    label3.Hide();
                    label4.Hide();
                    button1.Hide();
                    button2.Show();
                    MessageBox.Show(guid.Message, "Üdv " + textBox1.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form1_Load(sender, e);
                }
            }
        }

        /// <summary>
        /// login kész!
        /// </summary>

        private void button2_Click(object sender, EventArgs e)
        {
            var res = client.Logout(guid);
            label16.Text = "";
            textBox1.Show();
            textBox2.Show();
            button1.Show();
            label3.Show();
            label4.Show();
            button2.Hide();
            MessageBox.Show(res.Success, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Form1_Load(sender, e);
        }

        /// <summary>
        /// Selectedindex kész!
        /// </summary>

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var response = client.ActualCar(new ActualCarRequest() { Numberplate = listBox1.SelectedItem.ToString() });
                label17.Text = response.Car.Numberplate;
                label18.Text = response.Car.Brand;
                label19.Text = response.Car.Vintage.ToString();
                label20.Text = response.Car.Boughtprice.ToString() + " Ft"; ;
                label21.Text = response.Car.Currentvalue.ToString() + " Ft"; ;
            }
            
        }

        /// <summary>
        /// Autó vásárlás kész!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            int a;
            int b;
        
                var response = client.PurchaseCar(new PurchaseCarRequest()

                {
                    Uid = guid.Id,

                    Car = new Car()
                    {
                        Numberplate = textBox3.Text,
                        Brand = textBox4.Text,
                        Vintage = int.TryParse(textBox5.Text, out a) ? a : 0,
                        Boughtprice = int.TryParse(textBox6.Text, out b) ? b : 0,
                        Currentvalue = b

                    }
                });

                if (guid.Id == "")
                    MessageBox.Show(response.Message, "Sikertelen autóvásárlás!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    if (cars.Contains(textBox3.Text))
                        MessageBox.Show(response.Message, "Sikertelen autóvásárlás!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        if (textBox3.Text == "" || textBox4.Text == "" || (!int.TryParse(textBox5.Text, out a)) || (!int.TryParse(textBox6.Text, out b)))
                            MessageBox.Show(response.Message, "Sikertelen autóvásárlás!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                         {
                            MessageBox.Show(response.Car.Numberplate + " megvéve.", response.Message, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form1_Load(sender, e);
                            cars.Add(response.Car.Numberplate);
                         }
                       
                    }
                }
        }

            /// <summary>
            /// Autó eladás kész!
            /// </summary>

         private void button5_Click(object sender, EventArgs e)
         {
            var response = client.SellCar(new SellCarRequest() { Numberplate = listBox1.SelectedItem == null ? "" : listBox1.SelectedItem.ToString() });

            if (listBox1.SelectedItem == null)
                MessageBox.Show(response.Message, "Sikertelen autóeladás!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                MessageBox.Show(response.Message, "Sikeres autóeladás!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cars.Remove(listBox1.SelectedItem.ToString());
                Form1_Load(sender, e);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var response = client.RepairCar(new RepairCarRequest() { Numberplate = listBox1.SelectedItem == null ? "" : listBox1.SelectedItem.ToString() });

            if (listBox1.SelectedItem == null)
                MessageBox.Show(response.Message, "Sikertelen autójavítás!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                MessageBox.Show(response.Message, "Sikeres autójavítás!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form1_Load(sender, e);

            }
        }
    }
}

