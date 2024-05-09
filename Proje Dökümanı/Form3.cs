using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace YılanOyunu
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public void dosyadanYazdır(string dosyayolu)
        {
            StreamReader sw = File.OpenText(dosyayolu);
            //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
            string yazi;
            while ((yazi = sw.ReadLine()) != null)
            {
                listBox1.Items.Add(yazi);
            }
            //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
            //Son satır okunduktan sonra okuma işlemini bitirdik
            sw.Close();

        }     //İşimiz bitince kullandığımız nesneleri iade ettik.
    }
}
