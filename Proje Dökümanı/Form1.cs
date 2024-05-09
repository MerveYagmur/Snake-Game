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
using System.Security.Permissions;

namespace YılanOyunu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true; //Form1 de KeyPreview özelliğini etkinleştirdim.
        }
        // Tüm değişken tanımları burada tanımlandı.

        private int yilanHiziİndex = 1;//Yılanın Hızını belirleyen değişken.
        private float Puan = 0;//Kulanıcının puanını tutan değişken.
        private int yemYemeSuresi = 0;//Kullanıcının puanını hesaplamak için hangi sürede yemi yediğini tutan değişken.
        private string isim;//Kullanıcının ismini tutan değişken.
        private YonEnum yilaninYonu;//Yön tespiti için yılanın son yönünü tutan değişken.
        private int pozX = 250;//Yılanın son konumunu tutan değişkenler.
        private int pozY = 200;
        private const int xMax = 538;//Panelin Sınırlarını tutan değişkenler.
        private const int xMin = 5;
        private const int yMax = 338;
        private const int yMin = 5;
        public int saniye = 0, dakika = 0;//Süre tutma timer ı için kullanılan değişkenler.
        private string süreNot;//Dosyaya yazdırmak için kullanacağım süre değişkeni.
        private bool OyunDevamEdiyorMu;//Oyunun devam edip etmediği tutan değişken.
        public List<Point> yilaninKonumu = new List<Point>();//Yılanın Konumunu tutan liste.
        private Point yem;//Yemi tutan değişken.
        private Pen Kalem;//Çizim metodu için kullanılan değişkenler.
        private Point nokta;
        private Size boyut;
        public static List<string> isimler = new List<string>();//Skorları görüntüle için kullanılacak listeler.
        public static List<string> süreler = new List<string>();
        public static List<float> skorlar = new List<float>();
        private static string dosya_yolu = @"C:\Skorlar\skorlar.txt";//Skorları yazdıracağımız dosyanın yolu.
        private static int satirSayisi = 0;//Dosyaya satır satır yazdirabilmek için satır sayısını tutan değişken.
        private void button3_Click(object sender, EventArgs e)//Kullanıcı Skorları Görüntüle butonuna tıklarsa.
        {
            Form3 skorlarıGörüntüle=new Form3();//Dosyanın görüntüsü Form3 de gözükecek.
            skorlarıGörüntüle.dosyadanYazdır(dosya_yolu);
            skorlarıGörüntüle.Show();
        }
        private void button2_Click(object sender, EventArgs e)//Kullanıcı yardım butonuna tıklarsa.
        {
            Form2 yardım = new Form2();//oyun hakkında bilgi veren bir form açılacak.
            yardım.Show();
        }
        private void button1_Click(object sender, EventArgs e)//Kullanıcı Kişiyi kaydet butonun tıklarsa.
        {
            isim = Convert.ToString(isimtxt.Text);
            MessageBox.Show(isim);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);//KeyEventHandler'i Form1in KeyDown olayıyla ilişkilendirdim.
            radioButton1.Enabled = true;// Kişiyi kaydet buttonuna tıklandığı için radio buttonları etkinleştirdim.
            radioButton2.Enabled = true;
        }
       
        //Ana metotlar burada tanımlandı.
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B && OyunDevamEdiyorMu==false)// Kullanıcı eğer B tuşuna basarsa oyun başlayacak.
            {
                if (radioButton2.Checked == true)// Kullanıcı eğer zor seviyeyi seçerse yılan hızı değişir.
                {
                    yilanHiziİndex = 2;
                }
                YilanHizi();
                EkraniAyarla();//Kullanıcı B tuşuna bastığı için ekranı oyun oynanacak şekilde ayarlıyoruz.
                OyunDevamEdiyorMu =true;//Oyun başladığı için true yaptım.
                OyunZamanlayıcı.Enabled = true;//OyunZamanlayıcıyı başlattım.2 zamanlayıcı kullandım
            }                                  //biri yılanın hızı için biri süreyi tutması için.

            if (e.KeyCode == Keys.D)//Kullanıcı eğer D tuşuna basarsa oyun duracak
            {                       //durmuşsa yeniden başlayacak.
                OyunZamanlayıcı.Enabled = !OyunZamanlayıcı.Enabled;
                SüreTutma.Enabled = !SüreTutma.Enabled;
            }
            YonTespitEt(e.KeyCode);//Kullanıcı her tuşa bastığında yılanın yönünü tespit eder.
        }
        private void OyunZamanlayıcı_Tick(object sender, EventArgs e)//Oyunzamanlayıcının tekrarlayacağı olay.
        {
            OyunuOynama();//Kullanıcı oyunu durdurmadığı ya da kaybetmediği sürece yılan hareket edecek.
        }

        //Yardımcı metotlar burada tanımlandı.
        private void EkraniAyarla()//Oyunun oynanacağı ekranını ayarlayan metot.
        {
            richTextBox1.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            isimtxt.Visible = false;
            Yılan();
            YılanCizimi();
            YemCizimi();
        }
        private void YilanHizi()//Seçilen index e göre yılanın hızını ayarlayan metot.
        {
            switch (yilanHiziİndex)
            {
                case 1://Yavaş.
                    OyunZamanlayıcı.Interval = 60;
                    break;
                case 2://Hızlı.
                    OyunZamanlayıcı.Interval = 25;
                    break;
            }
        }
        private bool OyunuKaybetme()//Oyunun kaybedilip kaybedilmediğini kontrol eden metot.
        {
            if (pozX > xMax || pozX < xMin || pozY > yMax || pozY < yMin)//Eğer yılan panel sınırlarını aşarsa.
            {
                return true;
            }
            Point konum = yilaninKonumu[0];
            if (konum.X == pozX && konum.Y == pozY)//Eğer yılan kendini yediyse.
            {
                return true;
            }
            return false;
        }
        private void SkorTutma(Point konum)//Skoru tutan metot.
        {   //Yılanın konumunu istiyorum çünkü eğer yemi köşe noktalarda yerse ek puan alacak.
            Point solÜst = new Point(xMin, yMax);
            Point sagÜst = new Point(xMax, yMax);
            Point solAlt = new Point(xMin, yMin);
            Point sagAlt = new Point(xMax, yMin);
            if (yemYemeSuresi <= 100)//Yılan yemi 100 saniye içerisinde yerse
            {
                Puan += 100 / yemYemeSuresi;

                if ((konum == solÜst) || (konum == sagÜst) || (konum == solAlt) || (konum == sagAlt))//Yılan yemi köşelerde yerse
                {
                    Puan += 10;
                }
            }
            skorLabel.Text = "Puan:" + Puan.ToString();
        }
        private void YemYeme()//Yılan yemi yerse çağrılacak metot.
        {
            Point sonKonum = yilaninKonumu[yilaninKonumu.Count - 1];
            yilaninKonumu.Add(new Point(sonKonum.X, sonKonum.Y));
            yilaninKonumu.Add(new Point(sonKonum.X + 1, sonKonum.Y + 1));//Yılanın büyüdüğünü daha rahat 
            yilaninKonumu.Add(new Point(sonKonum.X + 2, sonKonum.Y + 2));//anlayabilmek için kuyruğa bu
            yilaninKonumu.Add(new Point(sonKonum.X + 3, sonKonum.Y + 3));//şekilde ekleme yaptım.
            yilaninKonumu.Add(new Point(sonKonum.X + 4, sonKonum.Y + 4));
            YılanCizimi();
            YemCizimi();
            SkorTutma(sonKonum);
            yemYemeSuresi = 0;//Yılan yemi yediği için yediği süre sıfırlandı.
        }
        private void YeniOyun()//Yeni Oyun Ayarlar.
        {
            pozX = 250;
            pozY = 200;
            Puan = saniye = dakika = 0;
            süreLabel.Text = "Geçen Süre:";
            skorLabel.Text = "Puan:";
            yilaninYonu = YonEnum.Sag;
            foreach (Point index in yilaninKonumu)//Son oyundaki yılanı ekrandan siliyor.
            {
                boyut = new Size(6, 6);
                int x = index.X;
                int y = index.Y;
                nokta = new Point(x, y);
                Silme(nokta, boyut);
            }
            boyut = new Size(5, 5);//Son oyundaki yemi ekrandan siliyor.
            Silme(yem, boyut);
            yilaninKonumu.RemoveRange(0, yilaninKonumu.Count);//Yılanın konumunu tutan listeyi sıfırlıyoruz.
        }
        private void AnaEkran()//Oyunun başlangıç ekranını ayarlayan metot.
        {
            richTextBox1.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            isimtxt.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            pozX = 250;
            pozY = 200;
            saniye = dakika = 0;
            süreLabel.Text = "Geçen Süre:";
            skorLabel.Text = "Puan:";
            isimtxt.Text = "";//Kullanıcı yeni isim girebilsin diye textbox u temizliyoruz.
            yilaninYonu = YonEnum.Sag;
            foreach (Point index in yilaninKonumu)//Son oyundaki yılanı ve yemi temizliyoruz.
            {
                boyut = new Size(4, 4);
                int x = index.X;
                int y = index.Y;
                nokta = new Point(x, y);
                Silme(nokta, boyut);
            }
            boyut = new Size(4, 4);
            Silme(yem, boyut);
            yilaninKonumu.RemoveRange(0, yilaninKonumu.Count);//Yılanın konumunu tutan listeyi sıfırlıyoruz.
        }
        private void OyunuOynama()//Oyunun temel fonksiyonudur.Asıl olay burada gerçekleşir.
        {
            YilanHareketi();//Yılanın hareketini belirliyoruz. 
            SüreTutma.Start();
            SüreTutma.Interval = 1000;
            bool OyununSonu = OyunuKaybetme();//Kullanıcı oyunu kaybettiyse true değilse false olur.
            if (OyununSonu)//Kullanıcı oyunu kaybettiyse.
            {
                dosyayaYaz(isim,süreNot,Puan);
                satirSayisi++;//Dosyaya satır satır yazdırabilmek için satır sayisini tutuyor.
                OyunZamanlayıcı.Enabled = false;//Oyun oynanmadığı için zamanlayıcıları fase ladık.
                SüreTutma.Enabled = false;
                OyunDevamEdiyorMu = false;
                switch (MessageBox.Show("Yeni Oyun", "Kaybettiniz", MessageBoxButtons.YesNo))
                {                         //Kullanıcıya kaybettiği bilgisi verilir ve yeni oyun sorulur.
                    case DialogResult.Yes://Evet derse.
                        YeniOyun();
                        break;
                    case DialogResult.No://Hayır derse.
                        this.KeyDown -= new KeyEventHandler(Form1_KeyDown);//kullanıcı kaydet tuşuna basmadan
                        AnaEkran();                                        //oyun başlamasın diye.
                        break;
                }
                return;
            }
            yilaninKonumu.Insert(0, new Point(pozX, pozY));//Yılanın ilerlemesi işlemleri
            int eleman = yilaninKonumu.Count - 1;
            Point kuyruk = yilaninKonumu[eleman];
            boyut = new Size(4, 4);
            Silme(kuyruk, boyut);//Yılanın her hareketinde kuyruk kısmı silinmeli ve baş kısmı çizilmeli.
            yilaninKonumu.RemoveAt(yilaninKonumu.Count - 1);
            YılanCizimi();
            if ((yem.X == pozX) && (yem.Y == pozY))//Yılan yemi yerse.
            {
                YemYeme();
            }
        }
        private void Cizim(Pen Kalem, Point nokta, Size boyut)//Genel Çizim metodu.
        {
            using (Graphics g = this.panel1.CreateGraphics())
            {
                g.DrawRectangle(Kalem, new Rectangle(nokta,boyut));//Çağırıldığı metottaki kalem rengini
                Kalem.Dispose();                                   // ve boyutunu kullanır.
            }
        }
        private void Silme(Point nokta, Size boyut)//Genel Silme metodu.
        {
            Kalem = new Pen(BackColor, 10);//Silinmiş efektini vermek için Formun arka plan rengini kullanıyoruz.
            Cizim(Kalem, nokta, boyut);
        }
        private void Yılan()//Yılanın baştaki konumu.
        {
            yilaninKonumu.Add(new Point(250, 200));
            yilaninKonumu.Add(new Point(251, 200));
            yilaninKonumu.Add(new Point(252, 200));
            yilaninKonumu.Add(new Point(253, 200));
            yilaninKonumu.Add(new Point(254, 200));
            yilaninKonumu.Add(new Point(255, 200));
            yilaninKonumu.Add(new Point(256, 200));
            yilaninKonumu.Add(new Point(257, 200));
            yilaninKonumu.Add(new Point(258, 200));
            yilaninKonumu.Add(new Point(259, 200));
            yilaninKonumu.Add(new Point(260, 200));
        }
        private void YılanCizimi()//Yılanın çizimini yapan metot.
        {
            foreach (Point index in yilaninKonumu)//Listenin her elemanı için aynı işlemi yapar.
            {
                Kalem = new Pen(Color.DeepSkyBlue,8);//her eleman yılanın bir parçasını gösteriyor.
                boyut = new Size(4, 4);              //Yılanı nokta nokta çizdiriyoruz.
                int x = index.X;
                int y = index.Y;
                nokta = new Point(x, y);
                Cizim(Kalem, nokta, boyut);
            }
        }
        private void YemCizimi()//Yemin Çizimini yapan metot.
        {
            Kalem = new Pen(Color.DarkOrange,8);
            Random rand = new Random();
            int randX = rand.Next(xMin, xMax);//Rastgele bir konum belirler.
            int randY = rand.Next(yMin, yMax);
            nokta = new Point(randX, randY);
            yem = nokta;
            boyut = new Size(4,4);
            Cizim(Kalem, nokta, boyut);
        }
        public void YilanHareketi()//Yılanın hareketini kontrol eden metot.
        {
            switch (yilaninYonu)//Yılan hangi yöne gittiyse 
            {
                case YonEnum.Asagi:
                    pozY++;
                    break;
                case YonEnum.Yukari:
                    pozY--;
                    break;
                case YonEnum.Sol:
                    pozX--;
                    break;
                case YonEnum.Sag:
                default:
                    pozX++;
                    break;
            }

        }
        private void YonTespitEt(Keys Keydata)//Yılanın yönünü tespit eden metot.
        {
            if (Keydata== Keys.Down)//Kullanıcı aşağı yön tuşuna basarsa.
            {
                if (yilaninYonu != YonEnum.Yukari)
                {
                    yilaninYonu = YonEnum.Asagi;
                }
            }
            if (Keydata== Keys.Right)//Kullanıcı sağ yön tuşuna basarsa.
            {
                if (yilaninYonu != YonEnum.Sol)
                {
                    yilaninYonu = YonEnum.Sag;
                }
            }
            if (Keydata== Keys.Left)//Kullanıcı sol yön tuşuna basarsa.
            {
                if (yilaninYonu != YonEnum.Sag)
                {
                    yilaninYonu = YonEnum.Sol;
                }

            }
            if (Keydata== Keys.Up)//Kullanıcı yukarı yön tuşuna basarsa.
            {
                if (yilaninYonu != YonEnum.Asagi)
                {
                    yilaninYonu = YonEnum.Yukari;
                }
            }
        }
        private static void dosyayaYaz(string isim, string süre, float skor)//Not defterine skorları yazdıran metot.
        {
            isimler.Add(isim);
            süreler.Add(süre);
            skorlar.Add(skor);
            //İşlem yapacağımız dosyanın yolunu belirtiyoruz.
            if (Directory.Exists(@"C:\Skorlar") == false)//Belirtilen klasör yoksa klasörü oluşturuyor.
            {                                            //Bunu yapmamın sebebi oyunun oynandığı her bilgisayarda
                Directory.CreateDirectory(@"C:\Skorlar");//Skorları yazdırma işleminin yapılabilmesi.
            }
            using (FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write))
            {   // Dosya işlemleri için File tream nesnesi oluşturduk.
                StreamWriter sw = new StreamWriter(fs);//Seçilen dosyaya yazma işlemi için stream writer nesnesi oluşturduk.
                int i = 0;
                float skor2;//For döngüsü için kullanılacak değişkenler.
                string isim2, süre2;
                for (i = 0; i <= satirSayisi; i++)//Satır satır yazdırma işlemi için for döngüsünü kullandım.
                {
                    isim2 = isimler[i];
                    süre2 = süreler[i];
                    skor2 = skorlar[i];
                    sw.WriteLine(isim2 + " " + süre2 + " " + "skor:" + skor2);
                }
                sw.Close();//Yapacağımız işlemler bittiği için nesneleri iadae ettik.
                fs.Close();
            }
        }
        private void SüreTutma_Tick(object sender, EventArgs e)//Oyun oynanırken bize güncel süreyi gösterir.
        {                                                    //SüreTutma timerını saniyede 1 şeklinde ayarladım.
            //sürelabel ına değerleri yazdırabilmemk için her koşulu değerlendirdim.
            if (dakika < 10)//Dakika 10 dan küçükse
            {
                if (saniye < 10)//Saniye 10 dan küçükse
                {
                    süreLabel.Text = "Geçen Süre:0" + dakika.ToString() + ":0" + saniye.ToString();
                    süreNot = süreLabel.Text;
                    saniye++;
                }
                else if (saniye >= 10 && saniye < 60)//Saniye 10 dan büyük eşitse ve saniye 60 dan küçükse
                {                                    //Ekranda 60 gözükmesin direkt dakikaya geçilsin diye.
                    süreLabel.Text = "Geçen Süre:0" + dakika.ToString() + ":" + saniye.ToString();
                    süreNot = süreLabel.Text;
                    saniye++;
                }
                else//(saniye==60)
                {
                    dakika++;
                    saniye = 0;
                }
            }

            if (dakika >= 10)//Yukarıdaki işlemleri dakikanın 10 dan büyük eşit olduğu durum içinde yaptık.
            {
                if (saniye < 10)
                {
                    süreLabel.Text = "Geçen Süre:" + dakika.ToString() + ":0" + saniye.ToString();
                    süreNot = süreLabel.Text;
                    saniye++;
                }
                else if (saniye >= 10 && saniye < 60)
                {
                    süreLabel.Text = "Geçen Süre:" + dakika.ToString() + ":" + saniye.ToString();
                    süreNot = süreLabel.Text;
                    saniye++;
                }
                else//(saniye==60)
                {
                    dakika++;
                    saniye = 0;
                }
            }
            yemYemeSuresi++;//yem yenildiğinde sıfırlanıp yeniden saymaya başlayacak.
        }
    }
    public enum YonEnum// Yön enumları
    {
        Tanımlanmadi, Yukari, Asagi, Sag, Sol
    }
}


