using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
//VERİ YAPILARI PROJESİ
//172119001 Abdulkadir DEMİREL
//172119005 Emrullah AŞĞAROĞLU
//172119013 Selim BOZKURT
//172119015 Serkan UĞUR

namespace EKSS_VeriYapilariProje
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        Node yrd;
        Node anaveri;//Butun cekilen verileri tutuyor.root gorevı goruyor.
        string veritipi = "";//Dosyalarin uzantasini tutuyor.
        string DosyaYolu;//Dosyanin bilgisayardaki adresini tutuyor.
        OpenFileDialog file_dialog = new OpenFileDialog();
        class Node //Çift Bağlı liste tanımlanması
        {
            public Node next { get; set; }//nexti tanimladik
            public Node prev { get; set; }//previ tanimladik
            public string data; // node'un verisi
        }

        //Çekilen bütün veriyi node'a ekliyor.(Txt,doc,pdf,html)
        //Tam eşleşmede cümle olarak noda ekler.
        //Yakın eşleşmede kelime kelime noda ekler.
        //Kısacası ne gönderirsen o şekilde node oluşturur...
        Node ekle(Node gelennode, string data)
        {
            if (gelennode == null)
            {
                gelennode = new Node();
                gelennode.data = data;
                gelennode.next = gelennode;
                gelennode.prev = gelennode;
            }
            else
            {
                yrd = gelennode.prev;
                yrd.next = new Node();
                yrd.next.data = data;
                yrd.next.next = gelennode;
                yrd.next.prev = yrd;
                gelennode.prev = yrd.next;
                gelennode = yrd.next.next;
                yrd = null;
            }
            return gelennode;
        }

        Node ekle(Node anaroot, Node KelimeAyirNode)
        {
            if (anaroot != null)
            {
                if (KelimeAyirNode != null)
                {
                    yrd = KelimeAyirNode.prev;
                    anaroot.prev.next = KelimeAyirNode;
                    KelimeAyirNode.prev = anaroot.prev;
                    anaroot.prev = yrd;
                    yrd.next = anaroot;
                    yrd = null;
                }
            }
            else
            {
                anaroot = KelimeAyirNode;//Şayet anaroot boşsa Kelime_Ayir'daki parçalanmış cümlenin gelen node'unu ana root yapar.
            }
            return anaroot;
        }
        //Gelen cümleyi kelime kelime ayirarak yeni bir node oluşturur.
        //Yakın eşleşmede kelime kelime ayirmak için kullanılır.
        Node Kelime_Ayir(string data)
        {
            Node gecicinode = null;
            string ayrilmiskelime = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != ' ')
                    ayrilmiskelime += data[i];//Bosluk gorene kadar karekter ekliyor 
                else
                {
                    gecicinode = ekle(gecicinode, ayrilmiskelime);
                    ayrilmiskelime = "";//Ayrilmis kelime temizlendi
                }
            }
            if (ayrilmiskelime != "")//Cumle sonunda kelime kalip kalmadigini kontrol ediyor.
                gecicinode = ekle(gecicinode, ayrilmiskelime);
            return gecicinode;//Cümleyi parçalanmış node olarak gönderiyor.
        }

        Node Txt_Veri_Al(string adres, bool kelimeayir)
        {
            var butunmetin = File.ReadLines(adres);//Txt'deki verileri satir satir diziye aktariyor.
            Node NodeTxt = null;
            if (!kelimeayir)//Tam eşleşmenin olacağı satiri aliyor node ekliyor.
            {
                foreach (string satir in butunmetin)//Dizideki verileri satir satir node'a ekliyor ve türkçe karakter sorununu çözüyor aşağıdaki kod.
                {
                    NodeTxt = ekle(NodeTxt, satir.Replace("&#252;", "ü").Replace("&#246;", "ö").Replace("&#231;", "ç").Replace("&#220;", "Ü").Replace("&#199;", "Ç").Replace("&#214;", "Ö").Replace("&#351;", "ş").Replace("&#350;", "Ş").Replace("&#304;", "İ").Replace("&#305;", "i").Replace("&#287;", "ğ").Replace("&#286;", "Ğ").Replace("&amp;", "&"));
                }
            }
            else
            {
                foreach (string satir in butunmetin)//Yakın eşleşmede kelime kelime ayirip node ekliyor.
                {
                    NodeTxt = ekle(NodeTxt, Kelime_Ayir(satir.Replace("&#252;", "ü").Replace("&#246;", "ö").Replace("&#231;", "ç").Replace("&#220;", "Ü").Replace("&#199;", "Ç").Replace("&#214;", "Ö").Replace("&#351;", "ş").Replace("&#350;", "Ş").Replace("&#304;", "İ").Replace("&#305;", "i").Replace("&#287;", "ğ").Replace("&#286;", "Ğ").Replace("&amp;", "&")));
                }
            }
            return NodeTxt;
        }

        Node Word_Veri_Al(object adres, bool kelimeayir)
        {
            Node NodeWord = null;
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref adres, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            if (kelimeayir)
                for (int i = 0; i < docs.Paragraphs.Count; i++)//Yakin eşleşmede kelime kelime NodeWord'e ekliyor.
                {
                    NodeWord = ekle(NodeWord, Kelime_Ayir(docs.Paragraphs[i + 1].Range.Text.ToString()));
                }
            else
                for (int i = 0; i < docs.Paragraphs.Count; i++)//Tam eşleşmede satir satir NodeWord'e ekliyor.
                {
                    NodeWord = ekle(NodeWord, docs.Paragraphs[i + 1].Range.Text.ToString());
                }
            docs.Close();//Sayfayi kapatiyor.
            word.Quit();//Programi komple kapatiyor.
            return NodeWord;
        }

        Node Pdf_Veri_Al(string adres, bool kelimeayir)
        {
            Node pdf = null;
            //Pdf'in adresindeki verileri oku nesnesinde tutuyor.
            iTextSharp.text.pdf.PdfReader oku = new iTextSharp.text.pdf.PdfReader(adres);
            if (kelimeayir)
                for (int i = 1; i <= oku.NumberOfPages; i++)//Yakin eşleşmede kelime kelime pdf'e ekliyor.
                {
                    pdf = ekle(pdf, Kelime_Ayir(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(oku, i).Replace("&#252;", "ü").Replace("&#246;", "ö").Replace("&#231;", "ç").Replace("&#220;", "Ü").Replace("&#199;", "Ç").Replace("&#214;", "Ö").Replace("&#351;", "ş").Replace("&#350;", "Ş").Replace("&#304;", "İ").Replace("&#305;", "i").Replace("&#287;", "ğ").Replace("&#286;", "Ğ").Replace("&amp;", "&")));
                }
            else
                for (int i = 1; i <= oku.NumberOfPages; i++)//Tam  eşleşmede satir satir pdf'e ekliyor.
                {
                    pdf = ekle(pdf, iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(oku, i).Replace("&#252;", "ü").Replace("&#246;", "ö").Replace("&#231;", "ç").Replace("&#220;", "Ü").Replace("&#199;", "Ç").Replace("&#214;", "Ö").Replace("&#351;", "ş").Replace("&#350;", "Ş").Replace("&#304;", "İ").Replace("&#305;", "i").Replace("&#287;", "ğ").Replace("&#286;", "Ğ").Replace("&amp;", "&"));
                }
            return pdf;
        }

        Node Html_Veri_Al(string adres, bool kelimeayir)
        {
            bool deger = false;
            Node NodeHtml = null;
            var butunmetin = File.ReadLines(adres);
            string satir, yazi = "";
            foreach (string satirr in butunmetin)
            {
                satir = satirr.Replace("&#252;", "ü").Replace("&#246;", "ö").Replace("&#231;", "ç").Replace("&#220;", "Ü").Replace("&#199;", "Ç").Replace("&#214;", "Ö").Replace("&#351;", "ş").Replace("&#350;", "Ş").Replace("&#304;", "İ").Replace("&#305;", "i").Replace("&#287;", "ğ").Replace("&#286;", "Ğ").Replace("&amp;", "&");
                for (int i = 0; i < satir.Length; i++)
                {
                    if (satir[i] == '>')//Buradaki amac  buyuktur ile kucuktur arasindaki verileri noda aktarmak.
                        deger = true;
                    else if (satir[i] == '<')
                        deger = false;
                    else
                    {
                        if (deger)
                            yazi += satir[i];
                    }
                }
                if (kelimeayir)
                    NodeHtml = ekle(NodeHtml, Kelime_Ayir(yazi));//Kelime kelime ayirarak NodeHtml node'una ekler.
                else
                    NodeHtml = ekle(NodeHtml, yazi);//Tam eşleşme satir satir ekleyerek NodeHtml node'una ekler.
                yazi = "";
            }
            return NodeHtml;
        }
        //Aradığımız kelimenin bulunduğu cümledeki ilk harfi aynı olanı buluyor.
        //Sonra diğer harflere göre karşılaştırmasını yapıyor 
        //Haflari ve boyutları eşit olduğu an sayacı arttırıyor.
        int BruteForceMatching(string pattern, string text)//Arama Algoritmasi
        {
            int sayac = 0, i, j, m = pattern.Length, n = text.Length;
            string x = pattern, y = text;
            for (j = 0; j <= n - m; ++j)
            {
                for (i = 0; i < m && x[i] == y[i + j]; ++i) ;
                if (i >= m)
                {
                    sayac++;
                }
            }
            return sayac;
        }
        private void Btn_txt_Click(object sender, RoutedEventArgs e)
        {
            file_dialog.DefaultExt = ".txt";
            file_dialog.Filter = "Text Files (.txt)|*.txt";//txt uzantili dosyaları filtreler
            file_dialog.ShowDialog();//Dosya secme ekrani
            DosyaYolu = file_dialog.FileName;//dosya yolunun adresini atiyor
            veritipi = "txt";//Dosyanin tipini kontrol amacli tuttugumuz string
        }

        private void Btn_Doc_Click(object sender, RoutedEventArgs e)
        {
            file_dialog.DefaultExt = ".docx";
            file_dialog.Filter = "Word Files (.docx)|*.docx";//.docx uzantili dosyalari filtreler
            file_dialog.ShowDialog();//Dosya secme ekrani
            DosyaYolu = file_dialog.FileName;//Dosya yolunun adresini tutuyor
            veritipi = "docx";//Dosyanin tipinikontrol amacli tuttugumuz string
        }

        private void Btn_Pdf_Click(object sender, RoutedEventArgs e)
        {
            file_dialog.DefaultExt = ".pdf";
            file_dialog.Filter = "PDF Files (.pdf)|*.pdf";//.pdf uzantili dosyalari filtreliyor
            file_dialog.ShowDialog();//Dosya secme ekrani
            DosyaYolu = file_dialog.FileName;//Dosya yolunun adresini tutup stringe gonderiyor
            veritipi = "pdf";//Dosyanin tipini kontrol amacli tutttugumuz string
        }

        private void Btn_Html_Click(object sender, RoutedEventArgs e)
        {
            file_dialog.DefaultExt = ".html";
            file_dialog.Filter = "HTML Files (.html)|*.html";//.html uzantili dosyalari filtreliyor
            file_dialog.ShowDialog();//Dosya secme ekrani
            DosyaYolu = file_dialog.FileName;//Dosya yolunun adresini tutup stringe gonderiyor
            veritipi = "html";//Dosyanin tipini kontrol amacli tuttugumuz string
        }

        private void Btn_Ara_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult message = MessageBoxResult.No;
            sp_AramaSonrasi.Visibility = Visibility.Visible;//Tasarim ekraninda label kismini görünür yapar.

            string veritabani = "C:\\Users/serkan/Desktop/proje/EKSS_VeriYapilariProje51/EKSS_VeriYapilariProje/deneme.txt";//Çekilen verilerin tutuldugu yer
            string enyakineslesme = "";//mesafe algoritması en yakin esleşmeyi tutuyor
            File.Delete(@veritabani);//Veritabinini (txt)'yi siliyor.
            FileStream fs = new FileStream(veritabani, FileMode.Append, FileAccess.Write, FileShare.Write);//Txt oluşturur
            StreamWriter sw = new StreamWriter(fs);
            DateTime baslangic = DateTime.Now, bitis;//Program çalişma anini  başlangıç olarak aktarıyor
            int sayac = 0;//Aranan verinin sayisini tutuyor
            Node son;
            int indexx = 0;
            switch (veritipi)//Butun verileri satir satir ana veriye aktariyor tam eşleşme
            {
                case "txt":
                    anaveri = Txt_Veri_Al(DosyaYolu, false);
                    break;
                case "docx":
                    anaveri = Word_Veri_Al(DosyaYolu, false);
                    break;
                case "pdf":
                    anaveri = Pdf_Veri_Al(DosyaYolu, false);
                    break;
                case "html":
                    anaveri = Html_Veri_Al(DosyaYolu, false);
                    break;
                default:
                    break;
            }
            son = anaveri;//anaveri başlangıicimz yani rootumuz
            lst_Search.Items.Clear();//list boxu aramaya başlamadan önce temizleniyor.

            do
            {
                //Aranan kelime ile o satiri kucultup bulunanlari indexx'e aktariyoruz
                indexx = BruteForceMatching(txt_Arama.Text.ToLower(), anaveri.data.ToLower());
                if (indexx > 0)
                {
                    lst_Search.Items.Add(anaveri.data);//Listbox'a yazdiriyoruz
                    sw.WriteLine("Bu satirda " + indexx + " kadar " + txt_Arama.Text + " bulunmuştur");
                    sw.WriteLine(anaveri.data);//Arka plandaki yaptıklarimizi txt'ye yazdiriyor
                    sw.WriteLine();//Boşluk koyma
                    //indexlerdekini sayaca atiyoruz cunku index satir satir çalıstıgı için sıfırlanıyor buldugumuz eslesmeleri kaybetmemek icin
                    sayac += indexx;
                }
                anaveri = anaveri.next;//Diğer satira geçmek için kulaniyoruz
            } while (anaveri != son);//root ilk baslagic yerine gelene kadar calisicak

            bitis = DateTime.Now;//aramanin bittigi andaki zaman

            sw.WriteLine(DosyaYolu + " Tam Eşleşmeden " + sayac.ToString() + " " + txt_Arama.Text + " tane bulundu " + (bitis - baslangic).Minutes + ":" + (bitis - baslangic).Seconds + ":" + (bitis - baslangic).Milliseconds + " süre aldı");

            lbl_ArananKelime.Content = "Aranan Kelime : " + txt_Arama.Text;
            lbl_AramaSuresi.Content = (bitis - baslangic).Minutes + ":" + (bitis - baslangic).Seconds + ":" + (bitis - baslangic).Milliseconds + " süre aldı";
            lbl_AramadaBulunanKelimeSayisi.Content = sayac.ToString() + " Tane bulundu";

            if (sayac == 0)//Hic tam eşleşme yapamadıysak
            {
                switch (veritipi)
                {
                    case "txt":
                        anaveri = Txt_Veri_Al(DosyaYolu, true);
                        break;
                    case "docx":
                        anaveri = Word_Veri_Al(DosyaYolu, true);
                        break;
                    case "pdf":
                        anaveri = Pdf_Veri_Al(DosyaYolu, true);
                        break;
                    case "html":
                        anaveri = Html_Veri_Al(DosyaYolu, true);
                        break;
                    default:
                        break;
                }

                son = anaveri;
                enyakineslesme = anaveri.data;//Başlangicda ilk elimizde kontrol verisi

                //Aradığımız kelimede bozuk karakter sayisini atiyor en az bozuk karakterli olani en yakin sonuc olarak veriyor.
                int enyakinsonuc;//Kaç karakterin bozuk oldugunu gosteren degisken
                enyakinsonuc = yakineslesme.FindLevenshteinDistance(anaveri.data, txt_Arama.Text);
                anaveri = anaveri.next;//Bir sonraki satira geçiyor

                while (anaveri != son)//Gelecegimiz yere başlangıca eşit mi diye bakiyor
                {
                    //Bozuk karakter sayisi buyuk olan kucukden buyukse  anaveriyi en yakın eslesme yapıyor.
                    //Ilk enyakın eslesme iptal oluyor anaveriyi en yakin sonuc yapiyoruz.
                    if (enyakinsonuc > yakineslesme.FindLevenshteinDistance(anaveri.data, txt_Arama.Text))
                    {
                        enyakineslesme = anaveri.data;
                        enyakinsonuc = yakineslesme.FindLevenshteinDistance(anaveri.data, txt_Arama.Text);
                    }
                    anaveri = anaveri.next;//rootu ilerletiyoruz

                }

                bitis = DateTime.Now;//Tam eşleşme başladı bulamayıp yakın eşleşmeye gelip buldugu an aradaki süre.
                message = MessageBox.Show(txt_Arama.Text + " den yakın eşleşme olarak " + enyakineslesme + " bulundu " + (bitis - baslangic).Minutes + ":" + (bitis - baslangic).Seconds + ":" + (bitis - baslangic).Milliseconds + " süre aldı", "EKSS", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                sw.WriteLine(DosyaYolu + " " + txt_Arama.Text + " den Yakın Eşleşme olarak " + enyakineslesme + " bulundu " + (bitis - baslangic).Minutes + ":" + (bitis - baslangic).Seconds + ":" + (bitis - baslangic).Milliseconds + " süre aldı");
            }
            sw.Close();
            if (message == MessageBoxResult.Yes)
            {
                txt_Arama.Text = enyakineslesme;//Arama motorunda en yakın eşleşme ile yer değiştiriyoruz.(orn:kadir iken kadır olması)
                Btn_Ara_Click(sender, e);
            }
            else
                System.Diagnostics.Process.Start(@veritabani);//En son arka planda kaydettiğimiz txt'yi açıyor ne yaptıysak.
        }
    }
}