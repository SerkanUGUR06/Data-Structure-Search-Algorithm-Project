using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKSS_VeriYapilariProje
{
    static class yakineslesme
    {
        public static int FindLevenshteinDistance(this string Source, string Target)
        {
            int n = Source.Length;
            int m = Target.Length;
            // Hesaplama matrisi üretilir. 2 boyutlu matrisin boyut uzunlukları ise kaynak ve hedef metinlerin karakter uzunluklarına göre set edilir
            int[,] Matrix = new int[n + 1, m + 1];

            // Eğer kaynak metin yoksa zaten hedef metnin tüm harflerinin değişimi söz konusu olduğundan,
            //Hedef metnin uzunluğu kadar bir yakınlık değeri mümkün olabilir 
            if (n == 0) 
                return m;

            if (m == 0) // Yukarıdaki durum hedefin karakter içermemesi halinde de geçerlidir 
                return n;

            // Aşağıdaki iki döngü ile yatay ve düşey eksenlerdeki standart 0,1,2,3,4...n elemanları doldurulur 
            for (int i = 0; i <= n; i++)
                Matrix[i, 0] = i;

            for (int j = 0; j <= m; j++)
                Matrix[0, j] = j;

            // Kıyaslama ve derecelendirme operasyonu yapılır 
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= m; j++)
                {
                    int cost = (Target[j - 1] == Source[i - 1]) ? 0 : 1;
                    Matrix[i, j] = Math.Min(Math.Min(Matrix[i - 1, j] + 1, Matrix[i, j - 1] + 1), Matrix[i - 1, j - 1] + cost);
                }

            return Matrix[n, m]; // sağ alt taraftaki hücre değeri döndürülür  
        }
    }
}
