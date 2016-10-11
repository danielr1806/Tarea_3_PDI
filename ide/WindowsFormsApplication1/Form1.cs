using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        private string DireccionGuardado;
        private string DireccionAbrir;
        public Bitmap Image;
        public Bitmap Image2;
        public Bitmap Image3;
        public int Acum;
        bool Act;
        int Zoom, ZoomAux, Sum;
        int[] histogram_a;
        int[] histogram_r;
        int[] histogram_g;
        int[] histogram_b;
        float[] Fa, Fr, Fg, Fb;


        public Form1()
        {
            InitializeComponent();
            Acum = 0;
            DireccionGuardado = "";
            DireccionAbrir = "";
            Act = false;
            Zoom = 0;
            ZoomAux = 0;
            Sum = 0;
        }

        private void AplicarFiltro(int tam, double[,] array)
        {
            // Verifico si hay alguna imagen
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            int w=Image2.Width, h=Image2.Height;
            Bitmap ImagenAux = CopyBitmap(Image);
            Color Pixel;
            // Ciclo principal para el recorrido de toda la Imagen.
            for (int x=0; x<w; x++ )
            {
                for (int y=0; y<h; y++ )
                {
                    double red = 0.0, green = 0.0, blue = 0.0, alpha = 0.0;
                    // Segundo ciclo para aplicar el filtro
                    for (int ny = 0; ny < tam; ny++)
                    {
                        for (int nx = 0; nx < tam; nx++)
                        {
                            int ImagenX = (x - (tam/2) + nx + w) % w;
                            int ImagenY = (y - (tam/2) + ny + h) % h;
                            Pixel = Image2.GetPixel(ImagenX,ImagenY);
                            red += (Pixel.R * array[ny,nx]);
                            green += (Pixel.G * array[ny,nx]);
                            blue +=(Pixel.B * array[ny,nx]);
                            alpha += (Pixel.A * array[ny, nx]);
                        }
                    }
                    ImagenAux.SetPixel(x, y, Color.FromArgb(255, Clamp((int)red + Sum), Clamp((int)green + Sum), Clamp((int)blue + Sum)));
                }
            }
            Image2 = CopyBitmap(ImagenAux);
            pictureBox1.Image = Image2;
        }

        public void DibujarHistograma() {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            MessageBox.Show(" Espere unos segundos ... ");
            //chart1.Series.Clear();
            histogram_r = new int[256];
            histogram_g = new int[256];
            histogram_b = new int[256];
            histogram_a = new int[256];
            // Calcular la cantidad de colores

            for ( int i=0; i<Image.Width; i++ )
            {
                for ( int j=0; j<Image.Height; j++)
                {
                    Color Pixel = Image2.GetPixel(i,j);
                    histogram_b[Pixel.B]++;
                    histogram_r[Pixel.R]++;
                    histogram_g[Pixel.G]++;
                    histogram_a[Pixel.A]++;
                }
            }
            // Crea Histograma
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            for (int i=0; i < histogram_r.Length; i++)
            {
                chart1.Series["Red"].Points.AddXY(i, histogram_r[i]);
                chart1.Series["Blue"].Points.AddXY(i, histogram_b[i]);
                chart1.Series["Green"].Points.AddXY(i, histogram_g[i]);
            }
                
        }

        public string GetDireccionGuardar()
        {
            return DireccionGuardado;
        }

        public string GetDireccionAbrir()
        {
            return DireccionAbrir;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Transformar
        private void Transformar()
        {
            Image2 = new Bitmap(Image.Width, Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

            for ( int i=0; i<Image.Width; i++ )
            {
                for ( int j=0; j<Image.Height; j++ )
                {
                    Color Pixel = Image.GetPixel(i, j);
                    Image2.SetPixel(i,j,Pixel);
                }
            }

            Image = CopyBitmap(Image2);
        }
        // Size Image

        private int sizeImage()
        {
            return 0;
        }

        // Guardar
        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            try
            {
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.Filter = ".BMP|*.bmp";
                SFD.ShowDialog();
                DireccionGuardado = SFD.FileName;
                Image.Save(DireccionGuardado, System.Drawing.Imaging.ImageFormat.Bmp);

            }catch (System.ArgumentException) { return; } 
        }
        // Cerrar
        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);  
        }
        // Abrir
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        // BMP
        private void imagenBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog OFD = new OpenFileDialog();
                OFD.Filter = "BMP|*.bmp";
                OFD.ShowDialog();
                DireccionAbrir = OFD.FileName;
                Image = new Bitmap(DireccionAbrir);
                pictureBox1.Image = Image;
                Image2 = CopyBitmap(Image);
                label1.Text = "Ancho: " + (Image.Width).ToString();
                label2.Text = "Alto: " + (Image.Height).ToString();
                label3.Text = "Tamaño: " + (Image.Width * Image.Height).ToString();
                label4.Text = "Profundidad: " + (Image.PixelFormat).ToString();
                Act = true;
                DibujarHistograma();
                if ( Image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb ) {Transformar();}
            }
            catch (ArgumentException)
            {
                return;
            }

        }
        // Otros formatos
        private void imagenJPEGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { 
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "(JPEG, JPG, PNG)|*.jpeg;*.jpg;*.png";
            OFD.ShowDialog();
            DireccionAbrir = OFD.FileName;
            Image = new Bitmap(DireccionAbrir);

            pictureBox1.Image = Image;
            Image2 = CopyBitmap(Image);
            label1.Text = "Ancho: " + (Image.Width).ToString();
            label2.Text = "Alto: " + (Image.Height).ToString();
            label3.Text = "Cantidad de Pixeles: " + (Image.Width * Image.Height).ToString();
            label4.Text = "Profundidad: " + (Image.PixelFormat).ToString();
            textBox1.Text = "0";
            Act = true;
            if (Image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb) { Transformar(); }
            }
            catch (ArgumentException)
            {
                return;
            }
}

        // Ecualización del histograma
        private void button11_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }

            int C = Image2.Width * Image2.Height;
            try
            {
                // Alpha
                Fa = new float[256];
                Fa[0] = 0;
                float aAcum = histogram_a[0];
                for (int i = 0; i < 255; i++)
                {
                    Fa[i] = (aAcum * 255) / C;
                    aAcum = aAcum + histogram_a[i];
                }
                Fa[255] = 255;

                // Red
                Fr = new float[256];
                Fr[0] = 0;
                float rAcum = histogram_r[0];
                for (int i = 0; i < 255; i++)
                {
                    Fr[i] = (rAcum * 255) / C;
                    rAcum = rAcum + histogram_r[i];
                }
                Fr[255] = 255;

                // Green
                Fg = new float[256];
                Fg[0] = 0;
                float gAcum = histogram_g[0];
                for (int i = 0; i < 255; i++)
                {
                    Fg[i] = (gAcum * 255) / C;
                    gAcum = gAcum + histogram_g[i];
                }
                Fg[255] = 255;

                // Blue
                Fb = new float[256];
                Fb[0] = 0;
                float bAcum = histogram_b[0];
                for (int i = 0; i < 255; i++)
                {
                    Fb[i] = (bAcum * 255) / C;
                    bAcum = bAcum + histogram_b[i];
                }
                Fb[255] = 255;

                for ( int i=0; i<Image2.Width; i++ )
                {
                    for ( int j=0; j<Image2.Height; j++ )
                    {
                        Color Pixel = Image2.GetPixel(i, j);
                        Image2.SetPixel(i, j, Color.FromArgb(Pixel.A,Clamp((int)Fr[Pixel.R]),Clamp((int)Fg[Pixel.G]),Clamp((int)Fb[Pixel.B])));
                    }
                }
                pictureBox1.Image = Image2;

            }
            catch (NullReferenceException)
            {
                MessageBox.Show(" No hay un histograma");
            }

        }


        // Deshacer
        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            // Restablecer los valores de la interfaz y la acumulada.
            Image2 = CopyBitmap(Image);
            trackBar1.Value = 0;
            label6.Text = "V a l o r : 0 ";
            Acum = 0;
            trackBar2.Value = 1;
            label8.Text = "V a l o r : 0 ";
            pictureBox1.Image = Image;
            ZoomAux = Zoom;
            textBox2.Text = "";
            textBox3.Text = "";
        }
        // Rehacer
        private void rehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            // Aplicar cambios a la Imagen
            Image = CopyBitmap(Image2);
            // Restablecer los valores de la interfaz y la acumulada.
            trackBar1.Value = 0;
            label6.Text = "V a l o r : 0 ";
            Acum = 0;
            trackBar2.Value = 1;
            label8.Text = "V a l o r : 0 ";
            pictureBox1.Image = Image;
            Image2 = CopyBitmap(Image);
            label1.Text = "Ancho: " + (Image.Width).ToString();
            label2.Text = "Alto: " + (Image.Height).ToString();
            label3.Text = "Tamaño: " + (Image.Width * Image.Height).ToString();
            label4.Text = "Profundidad: " + (Image.PixelFormat).ToString();
            textBox1.Text = "0";
            Zoom = ZoomAux;
            textBox2.Text = "";
            textBox3.Text = "";
        }
        // Salir
        private void button3_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        // Negativo
        private void button2_Click(object sender, EventArgs e)
        {
            if ( Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            for ( int i=0; i < Image2.Width; i++)
            {
                for ( int j=0; j<Image2.Height; j++)
                {
                    Color Pixel;
                    Pixel = Image2.GetPixel(i, j);
                    Image2.SetPixel(i, j, Color.FromArgb(Pixel.A,255 - Pixel.R, 255 - Pixel.G, 255 - Pixel.B));
                }
            }
            pictureBox1.Image = Image2;
        }
        // Funcion  Comparar
        private int Comparar ( int C, int U)
        {
            if ( C <= U)
            {
                return 0;
            }
            else
            {
                return 255;
            }
        }
        // Compiar bitmap
        protected Bitmap CopyBitmap(Bitmap source)
        {
            try
            {
                return new Bitmap(source);
            }catch(OutOfMemoryException)
            {
                MessageBox.Show("  Out of memory  ");
                Environment.Exit(0);
                return new Bitmap(source);
            }
        }

        // Clamp
        protected int Clamp ( int  P )
        {
            if (P > 255)
            {
                return 255;
            }
            if ( P < 0)
            {
                return 0;
            }
            return P;
        }
        // Umbralizar
        private void button1_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            try {
                int U = Int32.Parse(textBox1.Text);
                int A;
                for (int i = 0; i < Image2.Width; i++)
                {
                    for (int j = 0; j < Image2.Height; j++)
                    {
                        Color Pixel;
                        Pixel = Image2.GetPixel(i, j);
                        A = (Pixel.B + Pixel.G + Pixel.R) / 3;
                        A = Comparar(A, U);
                        Image2.SetPixel(i,j,Color.FromArgb(Pixel.A,A,A,A));
                    }
                }
                pictureBox1.Image = Image2;
            }
            catch (System.FormatException)
            {
                MessageBox.Show(" Error en los parametros ingresados ");
                return;
            }
}

        private void button4_Click(object sender, EventArgs e)
        {
            DibujarHistograma();
        }

        // Espejo Vertical
        private void button5_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            Color aux, color;
            int k;
            k = Image.Height - 1;

            for (int i = 0; i < Image2.Width; i++)
            {
                for (int j = 0; j <= (Image2.Height / 2); j++)
                {
                    aux = Image2.GetPixel(i, j);
                    color = Image2.GetPixel(i, k);
                    Image2.SetPixel(i, j, Color.FromArgb(color.A,color.R,color.G,color.B));
                    Image2.SetPixel(i, k, Color.FromArgb(aux.A,aux.R,aux.G,aux.B));
                    k = k - 1;
                }
                k = Image.Height - 1;
            }

            pictureBox1.Image = Image2;
        }

        // Espejo Horizontal
        private void button6_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            Color aux, color;
            int k;
            k = Image.Width - 1;
            for (int i = 0; i < Image2.Height; i++)
            {
                for (int j = 0; j <= (Image2.Width / 2); j++)
                {
                    aux = Image2.GetPixel(j, i);
                    color = Image2.GetPixel(k, i);
                    Image2.SetPixel(j, i,Color.FromArgb(color.A,color.R,color.G,color.B));
                    Image2.SetPixel(k, i,Color.FromArgb(aux.A,aux.R,aux.G,aux.B));
                    k = k - 1;
                }
                k = Image.Width - 1;
            }

            pictureBox1.Image = Image2;

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label6.Text = "V a l o r : " + trackBar1.Value.ToString();
        }

        // Aceptar Brillo
        private void button7_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            Acum = Acum + trackBar1.Value;  // Acumular los valores del Trackbar
            for (int i = 0; i < Image2.Width; i++)
            {
                for (int j = 0; j < Image2.Height; j++)
                {
                    Color Pixel = Image2.GetPixel(i,j);
                    Image2.SetPixel(i, j, Color.FromArgb(Pixel.A,Clamp(Pixel.R + Acum),Clamp(Pixel.G + Acum), Clamp(Pixel.B + Acum) ) );
                }
            }
            // Restablecer
            pictureBox1.Image = Image2;
            trackBar1.Value = 0;
            label6.Text = "V a l o r : 0 ";
        }

        // Aceptar Contraste
        private void button10_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            double Contraste;
            Contraste = trackBar2.Value;
            Contraste = (100 + Contraste) / 100;
            double A, R, G, B;
            for ( int i=0; i<Image2.Width;i++)
            {
                for ( int j=0; j<Image2.Height; j++)
                {
                    Color Pixel = Image2.GetPixel(i, j);
                    A = Math.Abs( ((((Pixel.A / 255.0) - 0.5) * Contraste) + 0.5) * 255.0 );
                    R = Math.Abs( ((((Pixel.R / 255.0) - 0.5) * Contraste) + 0.5) * 255.0 );
                    G = Math.Abs( ((((Pixel.G / 255.0) - 0.5) * Contraste) + 0.5) * 255.0 );
                    B = Math.Abs( ((((Pixel.B / 255.0) - 0.5) * Contraste) + 0.5) * 255.0 );
                    Image2.SetPixel(i, j, Color.FromArgb( Pixel.A, Clamp( (int)R ), Clamp( (int)G ), Clamp( (int)B ) ) );
                }
            }
            // Restablecer
            pictureBox1.Image = Image2;
            trackBar2.Value = 1;
            label8.Text = "V a l o r : 0 ";
        }

        // Zoom out
        private void button13_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            if (ZoomAux < -2)
            {
                MessageBox.Show(" Ha llegado a la cantidad maxima de zoom posible ");
                return;
            }
            Bitmap Image3 = new Bitmap(Image2.Width / 2, Image2.Height / 2, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Color Pixel1, Pixel2, Pixel3, Pixel4;
            int A, R, G, B;
            int Alto = Image2.Height, Ancho = Image2.Width;
            if (Image2.Width % 2 != 0) {
                Ancho = Ancho - 1;
            }
            if ( Image2.Height % 2 != 0)
            {
                Alto = Alto - 1;
            } 
            
                for (int i = 0; i < Ancho; i = i + 2)
                {
                    for (int j = 0; j < Alto; j = j + 2)
                    {
                        Pixel1 = Image2.GetPixel(i, j);
                        Pixel2 = Image2.GetPixel(i + 1, j);
                        Pixel3 = Image2.GetPixel(i, j + 1);
                        Pixel4 = Image2.GetPixel(i + 1, j + 1);
                        A = (Pixel1.A + Pixel2.A + Pixel3.A + Pixel4.A) / 4;
                        B = (Pixel4.B + Pixel3.B + Pixel2.B + Pixel1.B) / 4;
                        G = (Pixel1.G + Pixel2.G + Pixel3.G + Pixel4.G) / 4;
                        R = (Pixel1.R + Pixel2.R + Pixel3.R + Pixel4.R) / 4;
                        Image3.SetPixel(i / 2, j / 2, Color.FromArgb(A, R, G, B));
                    }
                }
                Image2 = CopyBitmap(Image3);
                pictureBox1.Image = Image2;
                ZoomAux--;
            
        }

        public Bitmap Escalar(int Ancho, int Alto)
        {
            int w2 = Ancho;
            int h2 = Alto;
            Bitmap Image3 = new Bitmap(w2, h2, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int x, y;
            float x_ratio = ((float)(Image2.Width - 1)) / w2;
            float y_ratio = ((float)(Image2.Height - 1)) / h2;
            float x_diff, y_diff;
            for (int i = 0; i < h2; i++)
            {
                for (int j = 0; j < w2; j++)
                {
                    x = (int)(x_ratio * j);
                    y = (int)(y_ratio * i);
                    x_diff = (x_ratio * j) - x;
                    y_diff = (y_ratio * i) - y;
                    Color Pixel1 = Image2.GetPixel(x, y);
                    Color Pixel2 = Image2.GetPixel(x + 1,y);
                    Color Pixel3 = Image2.GetPixel(x, y + 1);
                    Color Pixel4 = Image2.GetPixel(x + 1, y + 1);
                    float blue, red, green;

                    // blue element
                    // Yb = Ab(1-w)(1-h) + Bb(w)(1-h) + Cb(h)(1-w) + Db(wh)
                    blue = (Pixel1.B) * (1 - x_diff) * (1 - y_diff) + (Pixel2.B) * (x_diff) * (1 - y_diff) +
                           (Pixel3.B) * (y_diff) * (1 - x_diff) + (Pixel4.B) * (x_diff * y_diff);

                    // green element
                    // Yg = Ag(1-w)(1-h) + Bg(w)(1-h) + Cg(h)(1-w) + Dg(wh)
                    green = Pixel1.G * (1 - x_diff) * (1 - y_diff) + (Pixel2.G) * (x_diff) * (1 - y_diff) +
                            (Pixel3.G) * (y_diff) * (1 - x_diff) + (Pixel4.G) * (x_diff * y_diff);

                    // red element
                    // Yr = Ar(1-w)(1-h) + Br(w)(1-h) + Cr(h)(1-w) + Dr(wh)
                    red = (Pixel1.R) * (1 - x_diff) * (1 - y_diff) + (Pixel2.R) * (x_diff) * (1 - y_diff) +
                          (Pixel3.R) * (y_diff) * (1 - x_diff) + (Pixel4.R) * (x_diff * y_diff);

                    Image3.SetPixel(j, i, Color.FromArgb(Pixel1.A, Clamp((int)red), Clamp((int)green), Clamp((int)blue)));

                }
            }
            return Image3;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            try
            {
                int Alto, Ancho;
                Alto = Int32.Parse(textBox2.Text);
                Ancho = Int32.Parse(textBox3.Text);
                Image2 = Escalar(Ancho, Alto);
                pictureBox1.Image = Image2;
                textBox2.Text = "";
                textBox3.Text = "";
            }
            catch (System.FormatException)
            {
                MessageBox.Show(" Error en los parametros ingresados ");
                return;
            }

        }

        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Rotar
        private void button15_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            Color color;

            double t = Double.Parse(textBox4.Text);

            int x, y, x1, y1;
            int nr, nc;  //numero de renglones, numero de columnas
            nr = Image2.Height;
            nc = Image2.Width;
            t = t * Math.PI / 180;
            int min_x = (int)Math.Round(Math.Min(0.0, Math.Min(-Math.Sin(t) * (Image2.Width - 1), Math.Min(Math.Cos(t) * (Image2.Height - 1), Math.Cos(t) * (Image2.Height - 1) - Math.Sin(t) * (Image2.Width - 1)))));
            int max_x = (int)Math.Round(Math.Max(0.0, Math.Max(-Math.Sin(t) * (Image2.Width - 1), Math.Max(Math.Cos(t) * (Image2.Height - 1), Math.Cos(t) * (Image2.Height - 1) - Math.Sin(t) * (Image2.Width - 1)))));
            int min_y = (int)Math.Round(Math.Min(0.0, Math.Min(Math.Cos(t) * (Image2.Width - 1), Math.Min(Math.Sin(t) * (Image2.Height - 1), Math.Sin(t) * (Image2.Height - 1) + Math.Cos(t) * (Image2.Width - 1)))));
            int max_y = (int)Math.Round(Math.Max(0.0, Math.Max(Math.Cos(t) * (Image2.Width - 1), Math.Max(Math.Sin(t) * (Image2.Height - 1), Math.Sin(t) * (Image2.Height - 1) + Math.Cos(t) * (Image2.Width - 1)))));

            Image3 = new Bitmap(max_y - min_y + 1, max_x - min_x + 1);

            for (x1 = 0; x1 < max_x - min_x + 1; x1++)
            {
                for (y1 = 0; y1 < max_y - min_y + 1; y1++)
                {
                    x = (int)Math.Round(Math.Cos(t) * x1 - Math.Sin(t) * y1 - min_x);
                    y = (int)Math.Round(Math.Sin(t) * x1 + Math.Cos(t) * y1 - min_y);

                    if ((x >= 0) && (x < nr) && (y >= 0) && (y < nc))
                    {
                        color = Image2.GetPixel(y, x);
                        Image3.SetPixel(y1, x1, color);

                    }
                    /*else
                    {
                        color = Color.Black; //aqui le pones un valor que quieras
                    }*/

                }
            }

            Image2 = Image3;
            pictureBox1.Image = Image2;
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] array = new double[3,3] { { 0.0625, 0.125, 0.0625 }, { 0.125, 0.25, 0.125 }, { 0.0625, 0.125, 0.0625 } };
            AplicarFiltro(3, array);
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] array = new double[5, 5] { { 0.0039, 0.0155, 0.0235, 0.0155, 0.0039 }, { 0.0155, 0.062, 0.093, 0.062, 0.0155 }, { 0.0235, 0.093, 0.14, 0.093, 0.0235 }, { 0.0155, 0.062, 0.093, 0.062, 0.0155 }, { 0.0039, 0.0155, 0.0235, 0.0155, 0.0039 } };
            AplicarFiltro(5, array);
        }

        private void x7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] array = new double[7, 7] { { 0.00025, 0.00146, 0.00366, 0.00488, 0.00366, 0.00146, 0.00025 }, { 0.00146, 0.00878, 0.02197, 0.29296, 0.02197, 0.00878, 0.00146 }, { 0.00366, 0.02197, 0.055, 0.07324, 0.055, 0.02197, 0.00366 }, { 0.00488, 0.29296, 0.07324, 0.09765, 0.07324, 0.29296, 0.00488 }, { 0.00366, 0.02197, 0.055, 0.07324, 0.055, 0.02197, 0.00366 }, { 0.00146, 0.00878, 0.02197, 0.29296, 0.02197, 0.00878, 0.00146 }, { 0.00025, 0.00146, 0.00366, 0.00488, 0.00366, 0.00146, 0.00025 } };
            AplicarFiltro(7, array);
        }

        private void x9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] array = new double[9, 9] { { 0.0000, 0.0001, 0.0004, 0.0009, 0.0011, 0.0009, 0.0004, 0.0001, 0.0000 }, { 0.0001, 0.0010, 0.0034, 0.0068, 0.0085, 0.0068, 0.0034, 0.0010, 0.0001 }, { 0.0004, 0.0034, 0.0120, 0.0239, 0.0299, 0.0239, 0.0120, 0.0034, 0.0004 }, { 0.0009, 0.0068, 0.0239, 0.0479, 0.0598, 0.0479, 0.0239, 0.0068, 0.0009 }, { 0.0011, 0.0085, 0.0299, 0.0598, 0.0748, 0.0598, 0.0299, 0.0085, 0.0011 }, { 0.0009, 0.0068, 0.0239, 0.0479, 0.0598, 0.0479, 0.0239, 0.0068, 0.0009 }, { 0.0004, 0.0034, 0.0120, 0.0239, 0.0299, 0.0239, 0.0120, 0.0034, 0.0004 }, { 0.0001, 0.0010, 0.0034, 0.0068, 0.0085, 0.0068, 0.0034, 0.0010, 0.0001 }, { 0.0000, 0.0001, 0.0004, 0.0009, 0.0011, 0.0009, 0.0004, 0.0001, 0.0000 } };
            AplicarFiltro(9, array);
        }

        private void x11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] array = new double[11, 11] { { 0.0000, 0.0000, 0.0000, 0.0001, 0.0002, 0.0002, 0.0002, 0.0001, 0.0000, 0.0000, 0.0000 }, { 0.0000, 0.0001, 0.0004, 0.0011, 0.0020, 0.0024, 0.0020, 0.0011, 0.0004, 0.0001, 0.0000 }, { 0.0000, 0.0004, 0.0019, 0.0051, 0.0090, 0.0108, 0.0090, 0.0051, 0.0019, 0.0004, 0.0000 }, { 0.0001, 0.0011, 0.0051, 0.0137, 0.0240, 0.0288, 0.0240, 0.0137, 0.0051, 0.0011, 0.0001 }, { 0.0002, 0.0020, 0.0090, 0.0240, 0.0421, 0.0505, 0.0421, 0.0240, 0.0090, 0.0020, 0.0002 }, { 0.0002, 0.0024, 0.0108, 0.0288, 0.0505, 0.0606, 0.0505, 0.0288, 0.0108, 0.0024, 0.0002 }, { 0.0002, 0.0020, 0.0090, 0.0240, 0.0421, 0.0505, 0.0421, 0.0240, 0.0090, 0.0020, 0.0002 }, { 0.0001, 0.0011, 0.0051, 0.0137, 0.0240, 0.0288, 0.0240, 0.0137, 0.0051, 0.0011, 0.0001 }, { 0.0000, 0.0004, 0.0019, 0.0051, 0.0090, 0.0108, 0.0090, 0.0051, 0.0019, 0.0004, 0.0000 }, { 0.0000, 0.0001, 0.0004, 0.0011, 0.0020, 0.0024, 0.0020, 0.0011, 0.0004, 0.0001, 0.0000 }, { 0.0000, 0.0000, 0.0000, 0.0001, 0.0002, 0.0002, 0.0002, 0.0001, 0.0000, 0.0000, 0.0000 } };
            AplicarFiltro(11, array);

        }

        private void x15ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] array = new double[15, 15] { { 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0000, 0.0000, 0.0001, 0.0001, 0.0002, 0.0002, 0.0002, 0.0001, 0.0001, 0.0000, 0.0000, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0000, 0.0001, 0.0003, 0.0007, 0.0010, 0.0012, 0.0010, 0.0007, 0.0003, 0.0001, 0.0000, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0001, 0.0005, 0.0014, 0.0027, 0.0041, 0.0047, 0.0041, 0.0027, 0.0014, 0.0005, 0.0001, 0.0000, 0.0000 }, { 0.0000, 0.0001, 0.0003, 0.0014, 0.0037, 0.0075, 0.0112, 0.0128, 0.0112, 0.0075, 0.0037, 0.0014, 0.0003, 0.0000, 0.0000 }, { 0.0000, 0.0001, 0.0007, 0.0027, 0.0075, 0.0149, 0.0224, 0.0256, 0.0224, 0.0149, 0.0075, 0.0027, 0.0007, 0.0001, 0.0000 }, { 0.0000, 0.0002, 0.0010, 0.0041, 0.0112, 0.0224, 0.0336, 0.0384, 0.0336, 0.0224, 0.0112, 0.0041, 0.0010, 0.0001, 0.0000 }, { 0.0000, 0.0002, 0.0012, 0.0047, 0.0128, 0.0256, 0.0384, 0.0439, 0.0384, 0.0256, 0.0128, 0.0047, 0.0012, 0.0002, 0.0000 }, { 0.0000, 0.0002, 0.0010, 0.0041, 0.0112, 0.0224, 0.0336, 0.0384, 0.0336, 0.0224, 0.0112, 0.0041, 0.0010, 0.0001, 0.0000 }, { 0.0000, 0.0001, 0.0007, 0.0027, 0.0075, 0.0149, 0.0224, 0.0256, 0.0224, 0.0149, 0.0075, 0.0027, 0.0007, 0.0001, 0.0000 }, { 0.0000, 0.0001, 0.0003, 0.0014, 0.0037, 0.0075, 0.0112, 0.0128, 0.0112, 0.0075, 0.0037, 0.0014, 0.0003, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0001, 0.0005, 0.0014, 0.0027, 0.0041, 0.0047, 0.0041, 0.0027, 0.0014, 0.0005, 0.0001, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0000, 0.0001, 0.0003, 0.0007, 0.0010, 0.0012, 0.0010, 0.0007, 0.0003, 0.0001, 0.0000, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0001, 0.0001, 0.0002, 0.0001, 0.0001, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000 }, { 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000, 0.0000 } };
            AplicarFiltro(15, array);
        }

        private void suavizadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Nada
        }

        private void x3ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] array = new double[3, 3] { {0.11111,0.11111,0.11111}, {0.11111,0.11111,0.11111 }, {0.11111,0.11111,0.11111} };
            AplicarFiltro(3, array);
        }

        private void x5ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] array = new double[5, 5] { { 0.04, 0.04, 0.04, 0.04, 0.04 }, { 0.04, 0.04, 0.04, 0.04, 0.04 }, { 0.04, 0.04, 0.04, 0.04, 0.04 }, { 0.04, 0.04, 0.04, 0.04, 0.04 }, { 0.04, 0.04, 0.04, 0.04, 0.04 } };
            AplicarFiltro(5, array);
        }

        private void x7ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] array = new double[7, 7] { {0.02,0.02,0.02,0.02,0.02,0.02,0.02 }, { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02 }, { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02 }, { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02 }, { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02 }, { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02 }, { 0.02, 0.02, 0.02, 0.02, 0.02, 0.02, 0.02 } };
            AplicarFiltro(7, array);
        }

        private void x9ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] array = new double[9, 9] { {0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 }, { 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456, 0.0123456 } };
            AplicarFiltro(9, array);
        }

        private void x11ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] array = new double[11, 11] { {0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083}, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 }, { 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083, 0.0083 } };
            AplicarFiltro(11, array);
        }

        private void x15ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] array = new double[15, 15] { {0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 }, { 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045, 0.0045 } };
            AplicarFiltro(15, array);
        }

        private void x3ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[,] array = new double[3, 3] { {-1,0,1 }, { -1, 0, 1 }, { -1, 0, 1 } };
            AplicarFiltro(3,array);
        }

        private void x5ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[,] array = new double[5, 5] { { -1, -1, 0, 1, 1 }, { -1, -1, 0, 1, 1 }, { -1, -1, 0, 1, 1 }, { -1, -1, 0, 1, 1 }, { -1, -1, 0, 1, 1 } };
            AplicarFiltro(5, array);
        }

        private void x7ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[,] array = new double[7, 7] { {-1,-1,-1,0,1,1,1}, { -1, -1, -1, 0, 1, 1, 1 }, { -1, -1, -1, 0, 1, 1, 1 }, { -1, -1, -1, 0, 1, 1, 1 }, { -1, -1, -1, 0, 1, 1, 1 }, { -1, -1, -1, 0, 1, 1, 1 }, { -1, -1, -1, 0, 1, 1, 1 } };
            AplicarFiltro(7,array);
        }

        private void x9ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[,] array = new double[9, 9] { {-1,-1,-1,-1,0,1,1,1,1}, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 } };
            AplicarFiltro(9, array);
        }

        private void x11ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[,] array = new double[11, 11] { {-1,-1,-1,-1,-1,0,1,1,1,1,1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 } };
            AplicarFiltro(11, array);
        }

        private void x15ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[,] array = new double[15, 15] { {-1,-1,-1,-1,-1,-1,-1,0,1,1,1,1,1,1,1}, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 }, { -1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1 } };
            AplicarFiltro(15, array);
        }

        private void x5ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double[,] array = new double[5, 5]  { {-1,-1,0,1,1 }, {-4,-4,0,4,4 }, {-6,-6,0,6,6 }, {-4,-4,0,4,4 }, {-1,-1,0,1,1 } };
            AplicarFiltro(5,array);
        }

        private void x7ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double[,] array = new double[7, 7] { {-1,-1,-1,0,1,1,1 }, {-6,-6,-6,0,6,6,6}, {-15,-15,-15,0,15,15,15}, {-20,-20,-20,0,20,20,20 }, { -15, -15, -15, 0, 15, 15, 15 }, { -6, -6, -6, 0, 6, 6, 6 }, { -1, -1, -1, 0, 1, 1, 1 } };
            AplicarFiltro(7,array);
        }

        private void x9ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double[,] array = new double[9, 9] { { -1, -1, -1, -1, 0, 1, 1, 1, 1 }, { -8, -8, -8, -8, 0, 8, 8, 8, 8 }, { -28, -28, -28, -28, 0, 28, 28, 28, 28 }, { -56, -56, -56, -56, 0, 56, 56, 56, 56 }, { -70, -70, -70, -70, 0, 70, 70, 70, 70 }, { -56, -56, -56, -56, 0, 56, 56, 56, 56 }, { -28, -28, -28, -28, 0, 28, 28, 28, 28 }, { -8, -8, -8, -8, 0, 8, 8, 8, 8 }, { -1, -1, -1, -1, 0, 1, 1, 1, 1 } };
            AplicarFiltro(9, array);
        }

        private void x11ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double[,] array = new double[11, 11] { { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 }, { -10, -10, -10, -10, -10, 0, 10, 10, 10, 10, 10 }, { -45, -45, -45, -45, -45, 0, 45, 45, 45, 45, 45 }, { -120, -120, -120, -120, -120, 0, 120, 120, 120, 120, 120 }, { -210, -210, -210, -210, -210, 0, 210, 210, 210, 210, 210 }, { -252, -252, -252, -252, -252, 0, 252, 252, 252, 252, 252 }, { -210, -210, -210, -210, -210, 0, 210, 210, 210, 210, 210 }, { -120, -120, -120, -120, -120, 0, 120, 120, 120, 120, 120 }, { -45, -45, -45, -45, -45, 0, 45, 45, 45, 45, 45 }, { -10, -10, -10, -10, -10, 0, 10, 10, 10, 10, 10 }, { -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1 } };
            AplicarFiltro(11,array);
        }

        private void x15ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double[,] array = new double[15, 15]    {{-1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1}, {-14, -14, -14, -14, -14, -14, -14, 0, 14, 14, 14, 14, 14, 14, 14}, {-94, -94, -94, -94, -94, -94, -94, 0, 94, 94, 94, 94, 94, 94,94}, {-364, -364, -364, -364, -364, -364, -364, 0, 364, 364, 364, 364, 364, 364, 364}, {-1001, -1001, -1001, -1001, -1001, -1001, -1001, 0, 1001, 1001, 1001, 1001, 1001, 1001, 1001}, {-2002, -2002, -2002, -2002, -2002, -2002, -2002, 0, 2002, 2002, 2002, 2002, 2002, 2002, 2002},
    {-3003, -3003, -3003, -3003, -3003, -3003, -3003, 0, 3003, 3003, 3003, 3003, 3003, 3003, 3003}, {-3432, -3432, -3432, -3432, -3432, -3432, -3432, 0, 3432, 3432, 3432, 3432, 3432, 3432, 3432}, {-3003, -3003, -3003, -3003, -3003, -3003, -3003, 0, 3003, 3003, 3003, 3003, 3003, 3003, 3003},
    {-2002, -2002, -2002, -2002, -2002, -2002, -2002, 0, 2002, 2002, 2002, 2002, 2002, 2002, 2002}, {-1001, -1001, -1001, -1001, -1001, -1001, -1001, 0, 1001, 1001, 1001, 1001, 1001, 1001, 1001}, {-364, -364, -364, -364, -364, -364, -364, 0, 364, 364, 364, 364, 364, 364, 364}, {-94, -94, -94, -94, -94, -94, -94, 0, 94, 94, 94, 94, 94, 94,94}, {-14, -14, -14, -14, -14, -14, -14, 0, 14, 14, 14, 14, 14, 14, 14}, {-1, -1, -1, -1, -1, -1, -1, 0, 1, 1, 1, 1, 1, 1, 1}};
            AplicarFiltro(15, array);
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            Sum = 128;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Sum = 0;
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void x3ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            double[,] array = new double[3, 3] { {+1,0,0}, {0,0,0}, {0,0,-1 } };
            AplicarFiltro(3, array);

        }

        private void x5ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            double[,] array = new double[5, 5] { {+1,0,0,0,0 }, {0,+1,0,0,0 }, {0,0,0,0,0}, {0,0,0,-1,0 }, {0,0,0,0,-1 } };
            AplicarFiltro(5, array);
        }

        private void x7ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            double[,] array = new double[7, 7] { {1,0,0,0,0,0,0 }, {0,1,0,0,0,0,0 }, {0,0,1,0,0,0,0 }, {0,0,0,0,0,0,0 }, {0,0,0,0,-1,0,0}, {0,0,0,0,0,-1,0 }, {0,0,0,0,0,0,-1 } };
            AplicarFiltro(7, array);
        }

        private void x9ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            double[,] array = new double[9, 9] { {1,0,0,0,0,0,0,0,0 }, {0,1,0,0,0,0,0,0,0 }, {0,0,1,0,0,0,0,0,0 }, {0,0,0,1,0,0,0,0,0 }, {0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,-1,0,0,0}, {0,0,0,0,0,0,-1,0,0}, {0,0,0,0,0,0,0,-1,0}, {0,0,0,0,0,0,0,0,-1} };
            AplicarFiltro(9, array);
        }

        private void x11ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            double[,] array = new double[11, 11] { {1,0,0,0,0,0,0,0,0,0,0 }, {0,1,0,0,0,0,0,0,0,0,0 }, {0,0,1,0,0,0,0,0,0,0,0 }, {0,0,0,1,0,0,0,0,0,0,0 }, {0,0,0,0,1,0,0,0,0,0,0 }, {0,0,0,0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,-1,0,0,0,0 }, {0,0,0,0,0,0,0,-1,0,0,0 }, {0,0,0,0,0,0,0,0,-1,0,0 }, {0,0,0,0,0,0,0,0,0,-1,0 }, {0,0,0,0,0,0,0,0,0,0,-1} };
            AplicarFiltro(11,array);
        }

        private void x15ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            double[,] array = new double[15, 15] { {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0 }, {0,0,1,0,0,0,0,0,0,0,0,0,0,0,0 }, {0,0,0,1,0,0,0,0,0,0,0,0,0,0,0 }, {0,0,0,0,1,0,0,0,0,0,0,0,0,0,0 }, {0,0,0,0,0,1,0,0,0,0,0,0,0,0,0 }, {0,0,0,0,0,0,1,0,0,0,0,0,0,0,0 }, {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, {0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0 }, {0,0,0,0,0,0,0,0,0,-1,0,0,0,0,0 }, {0,0,0,0,0,0,0,0,0,0,-1,0,0,0,0 }, {0,0,0,0,0,0,0,0,0,0,0,-1,0,0,0 }, {0,0,0,0,0,0,0,0,0,0,0,0,-1,0,0 }, {0,0,0,0,0,0,0,0,0,0,0,0,0,-1,0 }, {0,0,0,0,0,0,0,0,0,0,0,0,0,0,-1 } };
            AplicarFiltro(15,array);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Bitmap ImageAux = CopyBitmap(Image2);
            double[,] array = new double[3, 3] { { -1, -1, -1 }, {-1,8,-1 }, {-1,-1,-1} };
            AplicarFiltro(3, array);
        }

        private void x3ToolStripMenuItem5_Click(object sender, EventArgs e)
        {

        }

        private void x3ToolStripMenuItem5_Click_1(object sender, EventArgs e)
        {
            double[,] array = new double[3, 3] { { -3, 0, 3 }, { -10, 0, 10 }, { -3, 0, 3 } };
            AplicarFiltro(3, array);
        }

        private void x5ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            double[,] array = new double[5, 5] { {-1,-1,0,1,1}, {-2,-2,0,2,2}, {-3,-6,0,6,3}, {-2,-2,0,2,2}, {-1,-1,0,1,1} };
            AplicarFiltro(5, array);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }

            int Gris = 0; ;
            Color Pixel;
            for ( int i=0; i < Image2.Width; i++)
            {
                for(int j = 0; j < Image2.Height; j++)
                {
                    Pixel = Image2.GetPixel(i, j);
                    Gris = (Pixel.R + Pixel.G + Pixel.B) / 3;
                    Image2.SetPixel(i, j, Color.FromArgb(255, Gris, Gris, Gris));
                }
            }
            pictureBox1.Image = Image2;
        }

        private void buscarArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    OpenFileDialog OFD = new OpenFileDialog();
                    OFD.Filter = "TXT|*.txt";
                    OFD.ShowDialog();
                    DireccionAbrir = OFD.FileName;
                    System.IO.StreamReader file = new System.IO.StreamReader(DireccionAbrir);
                    string Texto;
                    Texto = file.ReadToEnd();
                    string[] integerStrings = Texto.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    double[] integers = new double[integerStrings.Length];
                    for (int n = 0; n < integerStrings.Length; n++)
                    {
                        integers[n] = Convert.ToDouble(integerStrings[n]);
                    }
                    double[,] array = new double[(int)integers[0], (int)integers[0]];
                    int K = 1;
                    for (int i = 0; i < integers[0]; i++)
                    {
                        for (int j = 0; j < integers[0]; j++)
                        {
                            array[i, j] = integers[K];
                            K++;
                        }
                    }
                    label1.Text = integers[1].ToString();
                    AplicarFiltro((int)integers[0], array);
                    file.Close();
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
            catch(System.FormatException)
            {
                MessageBox.Show("Error en el formato de la matriz");
                return;
            }
        }

        private void x3ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            double[,] array = new double[3, 3] { {-1,0,1 }, {-2,0,2 }, {-1,0,1 } };
            AplicarFiltro(3, array);
        }




        // Zoom in
        private void button12_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            if ( ZoomAux > 2 )
            {
                MessageBox.Show(" Ha llegado a la cantidad maxima de zoom posible ");
                return;
            }
            Bitmap Image3 = new Bitmap(Image2.Width*2, Image2.Height*2, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for ( int i=0; i<Image2.Width; i++)
            {
                for ( int j=0; j<Image2.Height; j++)
                {
                    Color Pixel = Image2.GetPixel(i, j);
                    Image3.SetPixel(i*2,j*2,Pixel);
                    Image3.SetPixel((i*2)+1,j*2,Pixel);
                    Image3.SetPixel(i*2,(j*2)+1,Pixel);
                    Image3.SetPixel((i*2)+1,(j*2)+1,Pixel);
                }
            }
            Image2 = CopyBitmap(Image3);
            pictureBox1.Image = Image2;
            ZoomAux++;

        }

        // Aplicar
        private void button8_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            // Aplicar cambios a la Imagen
            Image =  CopyBitmap(Image2);
            // Restablecer los valores de la interfaz y la acumulada.
            trackBar1.Value = 0;
            label6.Text = "V a l o r : 0 ";
            Acum = 0;
            trackBar2.Value = 1;
            label8.Text = "V a l o r : 0 ";
            pictureBox1.Image = Image;
            Image2 = CopyBitmap(Image);
            label1.Text = "Ancho: " + (Image.Width).ToString();
            label2.Text = "Alto: " + (Image.Height).ToString();
            label3.Text = "Tamaño: " + (Image.Width * Image.Height).ToString();
            label4.Text = "Profundidad: " + (Image.PixelFormat).ToString();
            textBox1.Text = "0";
            Zoom = ZoomAux;
            textBox2.Text = "";
            textBox3.Text = "";
        }

        // Cancelar
        private void button9_Click(object sender, EventArgs e)
        {
            if (Act == false)
            {
                MessageBox.Show(" No hay imagen ");
                return;
            }
            // Restablecer los valores de la interfaz y la acumulada.
            Image2 = CopyBitmap(Image);
            trackBar1.Value = 0;
            label6.Text = "V a l o r : 0 ";
            Acum = 0;
            trackBar2.Value = 1;
            label8.Text = "V a l o r : 0 ";
            pictureBox1.Image = Image;
            ZoomAux = Zoom;
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label8.Text = "V a l o r : " + trackBar2.Value.ToString();
        }

    }

}
