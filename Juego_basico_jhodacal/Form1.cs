using System.Drawing.Drawing2D;

namespace Juego_basico_jhodacal
{
    public partial class Form1 : Form
    {

        //*********** VARIABLES GLOBALES *************//
        PictureBox navex = new PictureBox();
        PictureBox naveRival = new PictureBox();
        PictureBox contiene = new PictureBox();
        StatusStrip statusStrip1 = new StatusStrip();
        ToolStripStatusLabel toolStripStatusLabel1 = new ToolStripStatusLabel();
        ToolStripStatusLabel toolStripStatusLabel2 = new ToolStripStatusLabel();
        System.Windows.Forms.Timer tiempo;
        int Dispara = 0;
        bool flag = false;
        float angulo = 0;
        //*********** DIAGRAMAR DEL MISIL ***********//
        public void CrearMisil(int AngRotar, Color pintar, string nombre, int x, int y)
        {
            dynamic Balas = new PictureBox();
            int PosX = 1;
            int PosY = 1;
            int largoM = 11;
            int anchoM = 8;
            //declarar los array de puntos.
            Point[] myMisil1 = { new Point(4 * PosX, 0 * PosY), new Point(5 * PosX, 1 * PosY), new Point(6 * PosX, 2 * PosY), new Point(6 * PosX, 7 * PosY), new Point(7 * PosX, 8 * PosY), new Point(8 * PosX, 9 * PosY), new Point(7 * PosX, 9 * PosY), new Point(6 * PosX, 10 * PosY), new Point(2 * PosX, 10 * PosY), new Point(1 * PosX, 9 * PosY), new Point(0 * PosX, 9 * PosY), new Point(1 * PosX, 8 * PosY), new Point(2 * PosX, 7 * PosY), new Point(2 * PosX, 2 * PosY), new Point(3 * PosX, 1 * PosY), new Point(4 * PosX, 0 * PosY) };
            Point[] myMisil = new Point[myMisil1.Count()];
            for (int i = 0; i < myMisil1.Count(); i++)
            {
                myMisil[i].X = myMisil1[i].X;
                if (AngRotar == 180)
                    myMisil[i].Y = largoM - myMisil1[i].Y;
                else
                    myMisil[i].Y = myMisil1[i].Y;
            }
            GraphicsPath ObjGrafico = new GraphicsPath();
            ObjGrafico.AddPolygon(myMisil);
            Balas.Location = new Point(x, y);
            Balas.BackColor = pintar;
            Balas.Size = new Size(anchoM * PosX, largoM * PosY);
            Balas.Region = new Region(ObjGrafico);
            contiene.Controls.Add(Balas);
            Balas.Visible = true;
            Balas.Tag = nombre;
            //************** DIBUJAR COLORES *******************//
            Bitmap flag = new Bitmap(anchoM, largoM);
            Graphics flagImagen = Graphics.FromImage(flag);
            flagImagen.FillRectangle(Brushes.Orange, 2, 8, 5, 1);
            flagImagen.FillRectangle(Brushes.Yellow, 3, 10, 3, 1);
            Balas.Image = flag;
        }

        //****************DESTRUCTOR DEL MISIL***********//
        private void ImpactarTick(object sender, EventArgs e)
        {
            // VARIABLES LOCALES
            int X = naveRival.Location.X;
            int Y = naveRival.Location.Y;
            int W = naveRival.Width;
            int H = naveRival.Height;
            int PH = 6;
            int X2 = navex.Location.X;
            int Y2 = navex.Location.Y;
            int W2 = navex.Width;
            int H2 = navex.Height;
            int x = naveRival.Location.X;
            int y = naveRival.Location.Y;

            Dispara++;
            // ACCION DE DISPARAR DEL RIVAL
            if (Dispara == 100 && naveRival.Visible == true)
            {
                int xRival = naveRival.Location.X + (naveRival.Width / 2);
                int yRival = naveRival.Location.Y + (naveRival.Height / 2);
                CrearMisil(180, Color.DarkRed, "Rival", xRival, yRival);
                Dispara = 0;
            }

            // MOVIMIENTO DE LA NAVE A DESTRUIR
            if (flag == false)
            {
                if (contiene.Width == x + naveRival.Width)
                    flag = true;
                else
                    x++;
            }
            else
            {
                if (contiene.Location.X == x)
                    flag = false;
                else
                    x--;
            }

            naveRival.Location = new Point(x, y);
            naveRival.BorderStyle = BorderStyle.FixedSingle;
            // ELIMINACION DEL MISIL Y DESCONTAR PUNTOS DE IMPACTO DE LA NAVE RIVAL
            foreach (Control c in contiene.Controls)
            {
                if (c is PictureBox)
                {
                    int X1 = ((PictureBox)c).Location.X;
                    int Y1 = ((PictureBox)c).Location.Y;
                    int W1 = ((PictureBox)c).Width;
                    int H1 = ((PictureBox)c).Height;
                    string nombre = ((PictureBox)c).Tag.ToString();
                    // ACTIVIDAD DE IMPACTO CON LA NAVE RIVAL
                    if (X < X1 && X1 + W1 < X + W && Y < Y1 && Y1 + H1 < Y + H && nombre == "Misil")
                    {
                        // RETO 1: Cálculo de zonas de impacto geométrico relativas
                        // Se divide el ancho W de la nave rival en 3 segmentos iguales
                        int segmento = W / 3;
                        int cabinaInicio = X + segmento;
                        int cabinaFin = X + (2 * segmento);
                        int misilCentroX = X1 + (W1 / 2);

                        // Validación del punto de impacto:
                        // Si el centro del misil cae en la cabina (segmento central), quita 10 puntos de vida.
                        // Si impacta en las alas (ala izquierda o derecha), quita 1 punto de vida.
                        if (misilCentroX >= cabinaInicio && misilCentroX <= cabinaFin)
                        {
                            // Impacto crítico en la Cabina (Centro)
                            ((PictureBox)c).Dispose();
                            naveRival.Tag = int.Parse(naveRival.Tag.ToString()) - 10;
                        }
                        else
                        {
                            // Impacto en las Alas (Ala Izquierda o Ala Derecha)
                            ((PictureBox)c).Dispose();
                            naveRival.Tag = int.Parse(naveRival.Tag.ToString()) - 1;
                        }
                        toolStripStatusLabel1.Text = "Vida del Rival: " + naveRival.Tag.ToString();
                        //tiempo.Stop();
                    }

                    else if (int.Parse(naveRival.Tag.ToString()) <= 0)
                    {
                        naveRival.Dispose();
                        Bitmap NuevoImg = new Bitmap(contiene.Width, contiene.Height);
                        Graphics flagImagen = Graphics.FromImage(NuevoImg);
                        // Crear cadena para dibujar.
                        String drawString = "Felicitaciones Ganaste !";
                        // Crear la fuente y el pincel.
                        Font drawFont = new Font("Arial", 16);
                        SolidBrush drawBrush = new SolidBrush(Color.Blue);
                        Point drawPoint = new Point(40, 150);
                        // Dibujar cadena en pantalla.
                        flagImagen.DrawString(drawString, drawFont, drawBrush, drawPoint);
                        contiene.Image = NuevoImg;
                        tiempo.Stop();
                        // Escribir resultado en archivo para pruebas automatizadas
                        System.IO.File.WriteAllText("resultado.txt", "Ganaste");
                    }

                    // ACTIVIDAD DE IMPACTO CON MI NAVE
                    if (X2 < X1 && X1 + W1 < X2 + W2 && Y2 < Y1 && Y1 + H1 < Y2 + H2 && nombre == "Rival")
                    {
                        if (X2 + PH < X1 && X1 + W1 < X2 + W2 - PH)
                        {
                            ((PictureBox)c).Dispose();
                            navex.Tag = int.Parse(navex.Tag.ToString()) - 10;
                        }
                        else
                        {
                            ((PictureBox)c).Dispose();
                            navex.Tag = int.Parse(navex.Tag.ToString()) - 1;
                        }
                        toolStripStatusLabel2.Text = "Mi Nave: " + navex.Tag.ToString();
                    }
                    else if (int.Parse(navex.Tag.ToString()) <= 0)
                    {
                        navex.Dispose();
                        Bitmap NuevoImg = new Bitmap(contiene.Width, contiene.Height);
                        Graphics flagImagen = Graphics.FromImage(NuevoImg);
                        // Crear cadena para dibujar.
                        String drawString = "Perdiste el Juego";
                        // Crear la fuente y el pincel.
                        Font drawFont = new Font("Arial", 16);
                        SolidBrush drawBrush = new SolidBrush(Color.Red);
                        Point drawPoint = new Point(70, 150);
                        // Dibujar cadena en pantalla.
                        flagImagen.DrawString(drawString, drawFont, drawBrush, drawPoint);
                        contiene.Image = NuevoImg;
                        tiempo.Stop();
                        // Escribir resultado en archivo para pruebas automatizadas
                        System.IO.File.WriteAllText("resultado.txt", "Perdiste");
                    }
                    if (((PictureBox)c).Location.Y <= 0 && nombre == "Misil")
                    {
                        ((PictureBox)c).Dispose();
                    }
                    if (((PictureBox)c).Location.Y >= contiene.Height && nombre == "Rival")
                    {
                        ((PictureBox)c).Dispose();
                    }
                    if (nombre == "Misil")
                    {
                        ((PictureBox)c).Top -= 10;
                    }
                    if (nombre == "Rival")
                    {
                        ((PictureBox)c).Top += 10;
                    }
                    if (X + W >= X2 && Y + H >= Y2 && X2 + W2 >= X && Y2 + H2 >= Y)
                    {
                        naveRival.Dispose();
                        navex.Dispose();
                    }
                }
                else
                    tiempo.Stop();
            }

        }

        //*********** DIAGRAMAR NAVE Y TEXTURAS ***********//
        // Array de capas de colores y puntos extraídos de avion.png
        private static readonly (Color color, Point[] puntos)[] capasColores = new (Color, Point[])[]
        {
            (Color.FromArgb(96, 96, 96), new Point[] { new Point(28, 0), new Point(29, 0), new Point(25, 8), new Point(32, 8), new Point(27, 9), new Point(30, 9), new Point(24, 10), new Point(33, 10), new Point(26, 12), new Point(31, 12), new Point(27, 13), new Point(30, 13), new Point(25, 14), new Point(32, 14), new Point(22, 15), new Point(24, 15), new Point(28, 15), new Point(29, 15), new Point(33, 15), new Point(35, 15), new Point(24, 16), new Point(28, 16), new Point(29, 16), new Point(33, 16), new Point(24, 17), new Point(28, 17), new Point(29, 17), new Point(33, 17), new Point(23, 20), new Point(27, 20), new Point(30, 20), new Point(34, 20), new Point(25, 21), new Point(27, 21), new Point(30, 21), new Point(32, 21), new Point(23, 22), new Point(25, 22), new Point(32, 22), new Point(34, 22), new Point(28, 24), new Point(29, 24), new Point(23, 25), new Point(24, 25), new Point(28, 25), new Point(29, 25), new Point(33, 25), new Point(34, 25), new Point(24, 26), new Point(28, 26), new Point(29, 26), new Point(33, 26), new Point(22, 27), new Point(35, 27), new Point(24, 28), new Point(25, 28), new Point(32, 28), new Point(33, 28), new Point(35, 28), new Point(25, 29), new Point(26, 29), new Point(31, 29), new Point(32, 29), new Point(7, 30), new Point(26, 30), new Point(31, 30), new Point(50, 30), new Point(52, 30), new Point(7, 31), new Point(18, 31), new Point(26, 31), new Point(27, 31), new Point(30, 31), new Point(31, 31), new Point(50, 31), new Point(21, 32), new Point(25, 32), new Point(27, 32), new Point(30, 32), new Point(32, 32), new Point(36, 32), new Point(6, 33), new Point(21, 33), new Point(25, 33), new Point(32, 33), new Point(36, 33), new Point(38, 33), new Point(51, 33), new Point(21, 34), new Point(22, 34), new Point(27, 34), new Point(30, 34), new Point(35, 34), new Point(36, 34), new Point(8, 35), new Point(17, 35), new Point(21, 35), new Point(24, 35), new Point(33, 35), new Point(36, 35), new Point(40, 35), new Point(7, 36), new Point(20, 36), new Point(21, 36), new Point(27, 36), new Point(30, 36), new Point(36, 36), new Point(37, 36), new Point(50, 36), new Point(8, 37), new Point(17, 37), new Point(27, 37), new Point(30, 37), new Point(40, 37), new Point(5, 38), new Point(16, 38), new Point(23, 38), new Point(27, 38), new Point(30, 38), new Point(34, 38), new Point(41, 38), new Point(52, 38), new Point(5, 39), new Point(7, 39), new Point(16, 39), new Point(41, 39), new Point(50, 39), new Point(52, 39), new Point(5, 40), new Point(17, 40), new Point(40, 40), new Point(52, 40), new Point(8, 41), new Point(15, 41), new Point(20, 41), new Point(37, 41), new Point(42, 41), new Point(49, 41), new Point(13, 42), new Point(15, 42), new Point(19, 42), new Point(38, 42), new Point(42, 42), new Point(44, 42), new Point(8, 43), new Point(12, 43), new Point(17, 43), new Point(18, 43), new Point(25, 43), new Point(32, 43), new Point(39, 43), new Point(40, 43), new Point(45, 43), new Point(8, 44), new Point(49, 44), new Point(26, 45), new Point(50, 45), new Point(7, 46), new Point(10, 46), new Point(13, 46), new Point(44, 46), new Point(47, 46), new Point(50, 46), new Point(3, 47), new Point(7, 47), new Point(19, 47), new Point(38, 47), new Point(50, 47), new Point(54, 47), new Point(4, 48), new Point(7, 48), new Point(10, 48), new Point(20, 48), new Point(21, 48), new Point(36, 48), new Point(37, 48), new Point(47, 48), new Point(50, 48), new Point(53, 48), new Point(3, 49), new Point(7, 49), new Point(16, 49), new Point(20, 49), new Point(23, 49), new Point(24, 49), new Point(26, 49), new Point(33, 49), new Point(34, 49), new Point(37, 49), new Point(41, 49), new Point(50, 49), new Point(54, 49), new Point(22, 50), new Point(35, 50), new Point(0, 51), new Point(2, 51), new Point(10, 51), new Point(25, 51), new Point(26, 51), new Point(32, 51), new Point(47, 51), new Point(55, 51), new Point(57, 51), new Point(2, 52), new Point(21, 52), new Point(25, 52), new Point(32, 52), new Point(36, 52), new Point(55, 52), new Point(2, 53), new Point(6, 53), new Point(9, 53), new Point(12, 53), new Point(48, 53), new Point(51, 53), new Point(55, 53), new Point(2, 54), new Point(13, 54), new Point(16, 54), new Point(17, 54), new Point(23, 54), new Point(34, 54), new Point(40, 54), new Point(41, 54), new Point(44, 54), new Point(55, 54), new Point(1, 55), new Point(11, 55), new Point(17, 55), new Point(40, 55), new Point(46, 55), new Point(56, 55), new Point(9, 56), new Point(18, 56), new Point(39, 56), new Point(48, 56), new Point(49, 56), new Point(18, 57), new Point(39, 57), new Point(51, 57), new Point(6, 58), new Point(23, 58), new Point(24, 58), new Point(33, 58), new Point(34, 58), new Point(51, 58), new Point(6, 59), new Point(33, 59), new Point(51, 59), new Point(55, 59), new Point(5, 60), new Point(6, 60), new Point(22, 60), new Point(35, 60), new Point(52, 60), new Point(5, 61), new Point(52, 61), new Point(4, 63), new Point(53, 63), new Point(23, 65), new Point(34, 65), new Point(22, 66), new Point(35, 66), new Point(3, 67), new Point(21, 67), new Point(54, 67), new Point(24, 69), new Point(33, 69), new Point(22, 70), new Point(35, 70), new Point(24, 71), new Point(33, 71), new Point(5, 72), new Point(23, 72), new Point(34, 72), new Point(52, 72), new Point(34, 73) }),
            (Color.FromArgb(160, 160, 160), new Point[] { new Point(27, 1), new Point(30, 1), new Point(31, 3), new Point(33, 9), new Point(34, 11), new Point(26, 13), new Point(31, 13), new Point(22, 14), new Point(26, 14), new Point(31, 14), new Point(35, 14), new Point(23, 17), new Point(34, 17), new Point(23, 18), new Point(34, 18), new Point(28, 19), new Point(29, 19), new Point(24, 20), new Point(33, 20), new Point(28, 21), new Point(29, 21), new Point(24, 22), new Point(28, 22), new Point(29, 22), new Point(33, 22), new Point(6, 24), new Point(36, 27), new Point(5, 28), new Point(21, 28), new Point(36, 28), new Point(18, 29), new Point(39, 29), new Point(22, 30), new Point(35, 30), new Point(5, 31), new Point(22, 31), new Point(35, 31), new Point(5, 32), new Point(22, 32), new Point(35, 32), new Point(52, 32), new Point(26, 33), new Point(31, 33), new Point(8, 34), new Point(20, 34), new Point(28, 34), new Point(29, 34), new Point(37, 34), new Point(49, 34), new Point(52, 34), new Point(6, 35), new Point(19, 35), new Point(25, 35), new Point(32, 35), new Point(38, 35), new Point(51, 35), new Point(19, 36), new Point(38, 36), new Point(24, 37), new Point(33, 37), new Point(6, 38), new Point(26, 38), new Point(31, 38), new Point(51, 38), new Point(15, 39), new Point(23, 39), new Point(34, 39), new Point(22, 40), new Point(35, 40), new Point(3, 41), new Point(25, 41), new Point(32, 41), new Point(54, 41), new Point(24, 42), new Point(33, 42), new Point(11, 43), new Point(18, 44), new Point(19, 44), new Point(38, 44), new Point(39, 44), new Point(9, 45), new Point(21, 45), new Point(36, 45), new Point(48, 45), new Point(12, 46), new Point(45, 46), new Point(6, 47), new Point(11, 47), new Point(12, 47), new Point(16, 47), new Point(26, 47), new Point(31, 47), new Point(41, 47), new Point(45, 47), new Point(46, 47), new Point(51, 47), new Point(6, 48), new Point(51, 48), new Point(4, 49), new Point(6, 49), new Point(51, 49), new Point(53, 49), new Point(6, 50), new Point(24, 50), new Point(33, 50), new Point(51, 50), new Point(3, 51), new Point(54, 51), new Point(10, 52), new Point(22, 52), new Point(31, 52), new Point(35, 52), new Point(46, 52), new Point(47, 52), new Point(10, 53), new Point(11, 53), new Point(22, 53), new Point(35, 53), new Point(46, 53), new Point(47, 53), new Point(57, 53), new Point(5, 54), new Point(7, 54), new Point(10, 54), new Point(21, 54), new Point(22, 54), new Point(35, 54), new Point(36, 54), new Point(47, 54), new Point(50, 54), new Point(52, 54), new Point(5, 55), new Point(52, 55), new Point(12, 56), new Point(16, 56), new Point(41, 56), new Point(45, 56), new Point(17, 58), new Point(39, 58), new Point(40, 58), new Point(20, 59), new Point(22, 59), new Point(35, 59), new Point(37, 59), new Point(4, 60), new Point(24, 60), new Point(33, 60), new Point(53, 60), new Point(2, 61), new Point(4, 61), new Point(24, 61), new Point(53, 61), new Point(55, 61), new Point(2, 62), new Point(51, 62), new Point(55, 62), new Point(6, 63), new Point(51, 63), new Point(51, 64), new Point(54, 68), new Point(3, 69), new Point(54, 69), new Point(36, 70), new Point(54, 70), new Point(5, 74), new Point(23, 75) }),
            (Color.FromArgb(64, 64, 64), new Point[] { new Point(28, 1), new Point(29, 1), new Point(27, 2), new Point(28, 2), new Point(29, 2), new Point(30, 2), new Point(27, 3), new Point(30, 3), new Point(27, 5), new Point(28, 5), new Point(29, 5), new Point(30, 5), new Point(31, 5), new Point(26, 6), new Point(27, 6), new Point(28, 6), new Point(29, 6), new Point(30, 6), new Point(31, 6), new Point(26, 7), new Point(27, 7), new Point(28, 7), new Point(29, 7), new Point(30, 7), new Point(31, 7), new Point(26, 8), new Point(27, 8), new Point(28, 8), new Point(29, 8), new Point(30, 8), new Point(31, 8), new Point(25, 9), new Point(28, 9), new Point(29, 9), new Point(32, 9), new Point(25, 10), new Point(32, 10), new Point(24, 11), new Point(33, 11), new Point(23, 13), new Point(24, 13), new Point(33, 13), new Point(34, 13), new Point(23, 14), new Point(24, 14), new Point(33, 14), new Point(34, 14), new Point(23, 15), new Point(34, 15), new Point(22, 16), new Point(26, 16), new Point(31, 16), new Point(35, 16), new Point(22, 17), new Point(35, 17), new Point(22, 18), new Point(24, 18), new Point(25, 18), new Point(27, 18), new Point(30, 18), new Point(32, 18), new Point(33, 18), new Point(35, 18), new Point(22, 19), new Point(24, 19), new Point(33, 19), new Point(35, 19), new Point(27, 22), new Point(30, 22), new Point(22, 23), new Point(24, 23), new Point(25, 23), new Point(27, 23), new Point(30, 23), new Point(32, 23), new Point(33, 23), new Point(35, 23), new Point(23, 24), new Point(34, 24), new Point(26, 25), new Point(31, 25), new Point(22, 26), new Point(26, 26), new Point(31, 26), new Point(35, 26), new Point(25, 27), new Point(26, 27), new Point(28, 27), new Point(29, 27), new Point(31, 27), new Point(32, 27), new Point(26, 28), new Point(31, 28), new Point(24, 29), new Point(33, 29), new Point(25, 30), new Point(32, 30), new Point(28, 31), new Point(29, 31), new Point(6, 32), new Point(7, 32), new Point(18, 32), new Point(23, 32), new Point(34, 32), new Point(39, 32), new Point(50, 32), new Point(51, 32), new Point(7, 33), new Point(19, 33), new Point(50, 33), new Point(7, 34), new Point(24, 34), new Point(33, 34), new Point(50, 34), new Point(7, 35), new Point(22, 35), new Point(27, 35), new Point(30, 35), new Point(35, 35), new Point(50, 35), new Point(8, 36), new Point(17, 36), new Point(18, 36), new Point(39, 36), new Point(40, 36), new Point(49, 36), new Point(7, 37), new Point(18, 37), new Point(20, 37), new Point(23, 37), new Point(34, 37), new Point(37, 37), new Point(39, 37), new Point(50, 37), new Point(7, 38), new Point(18, 38), new Point(39, 38), new Point(50, 38), new Point(18, 39), new Point(22, 39), new Point(27, 39), new Point(30, 39), new Point(35, 39), new Point(39, 39), new Point(4, 40), new Point(6, 40), new Point(7, 40), new Point(15, 40), new Point(21, 40), new Point(27, 40), new Point(30, 40), new Point(36, 40), new Point(42, 40), new Point(50, 40), new Point(51, 40), new Point(53, 40), new Point(5, 41), new Point(6, 41), new Point(14, 41), new Point(26, 41), new Point(27, 41), new Point(30, 41), new Point(31, 41), new Point(43, 41), new Point(51, 41), new Point(52, 41), new Point(3, 42), new Point(4, 42), new Point(5, 42), new Point(14, 42), new Point(18, 42), new Point(26, 42), new Point(31, 42), new Point(39, 42), new Point(52, 42), new Point(54, 42), new Point(4, 43), new Point(16, 43), new Point(41, 43), new Point(53, 43), new Point(7, 44), new Point(10, 44), new Point(11, 44), new Point(12, 44), new Point(13, 44), new Point(16, 44), new Point(24, 44), new Point(26, 44), new Point(31, 44), new Point(33, 44), new Point(41, 44), new Point(44, 44), new Point(45, 44), new Point(46, 44), new Point(47, 44), new Point(50, 44), new Point(2, 45), new Point(6, 45), new Point(10, 45), new Point(13, 45), new Point(16, 45), new Point(22, 45), new Point(24, 45), new Point(33, 45), new Point(35, 45), new Point(47, 45), new Point(51, 45), new Point(55, 45), new Point(3, 46), new Point(8, 46), new Point(9, 46), new Point(24, 46), new Point(33, 46), new Point(48, 46), new Point(49, 46), new Point(54, 46), new Point(13, 47), new Point(15, 47), new Point(21, 47), new Point(36, 47), new Point(42, 47), new Point(44, 47), new Point(9, 48), new Point(13, 48), new Point(15, 48), new Point(23, 48), new Point(24, 48), new Point(33, 48), new Point(34, 48), new Point(42, 48), new Point(44, 48), new Point(48, 48), new Point(2, 49), new Point(15, 49), new Point(17, 49), new Point(19, 49), new Point(21, 49), new Point(22, 49), new Point(25, 49), new Point(31, 49), new Point(32, 49), new Point(35, 49), new Point(36, 49), new Point(38, 49), new Point(40, 49), new Point(42, 49), new Point(55, 49), new Point(1, 50), new Point(2, 50), new Point(7, 50), new Point(10, 50), new Point(11, 50), new Point(12, 50), new Point(15, 50), new Point(20, 50), new Point(25, 50), new Point(32, 50), new Point(37, 50), new Point(42, 50), new Point(45, 50), new Point(46, 50), new Point(47, 50), new Point(50, 50), new Point(55, 50), new Point(56, 50), new Point(1, 51), new Point(7, 51), new Point(21, 51), new Point(36, 51), new Point(50, 51), new Point(56, 51), new Point(0, 52), new Point(1, 52), new Point(9, 52), new Point(48, 52), new Point(56, 52), new Point(57, 52), new Point(13, 53), new Point(15, 53), new Point(17, 53), new Point(18, 53), new Point(20, 53), new Point(37, 53), new Point(39, 53), new Point(40, 53), new Point(42, 53), new Point(44, 53), new Point(6, 54), new Point(8, 54), new Point(12, 54), new Point(18, 54), new Point(39, 54), new Point(45, 54), new Point(49, 54), new Point(51, 54), new Point(2, 55), new Point(6, 55), new Point(8, 55), new Point(9, 55), new Point(13, 55), new Point(18, 55), new Point(20, 55), new Point(21, 55), new Point(36, 55), new Point(37, 55), new Point(39, 55), new Point(44, 55), new Point(48, 55), new Point(49, 55), new Point(51, 55), new Point(55, 55), new Point(6, 56), new Point(17, 56), new Point(19, 56), new Point(38, 56), new Point(40, 56), new Point(51, 56), new Point(6, 57), new Point(17, 57), new Point(20, 57), new Point(23, 57), new Point(24, 57), new Point(33, 57), new Point(34, 57), new Point(37, 57), new Point(40, 57), new Point(2, 58), new Point(22, 58), new Point(35, 58), new Point(55, 58), new Point(23, 59), new Point(34, 59), new Point(3, 60), new Point(21, 60), new Point(36, 60), new Point(54, 60), new Point(36, 61), new Point(5, 62), new Point(21, 62), new Point(36, 62), new Point(52, 62), new Point(5, 63), new Point(52, 63), new Point(3, 64), new Point(4, 64), new Point(23, 64), new Point(34, 64), new Point(53, 64), new Point(3, 65), new Point(22, 65), new Point(35, 65), new Point(54, 65), new Point(3, 66), new Point(54, 66), new Point(23, 67), new Point(34, 67), new Point(5, 68), new Point(52, 68), new Point(5, 69), new Point(52, 69), new Point(5, 70), new Point(52, 70), new Point(5, 71), new Point(23, 71), new Point(34, 71), new Point(52, 71), new Point(24, 72), new Point(33, 72), new Point(33, 73), new Point(24, 74), new Point(4, 75), new Point(53, 75), new Point(24, 76), new Point(33, 76) }),
            (Color.FromArgb(192, 192, 192), new Point[] { new Point(26, 2), new Point(25, 6), new Point(32, 6), new Point(24, 9), new Point(23, 11), new Point(21, 17), new Point(21, 18), new Point(36, 18), new Point(21, 19), new Point(23, 19), new Point(34, 19), new Point(36, 19), new Point(21, 20), new Point(36, 21), new Point(21, 22), new Point(21, 23), new Point(36, 23), new Point(36, 24), new Point(51, 24), new Point(52, 24), new Point(5, 25), new Point(21, 25), new Point(52, 25), new Point(21, 26), new Point(36, 26), new Point(7, 27), new Point(21, 27), new Point(23, 27), new Point(34, 27), new Point(50, 27), new Point(52, 27), new Point(23, 28), new Point(34, 28), new Point(52, 28), new Point(23, 29), new Point(34, 29), new Point(37, 29), new Point(20, 30), new Point(34, 30), new Point(37, 30), new Point(20, 31), new Point(37, 31), new Point(52, 31), new Point(20, 32), new Point(37, 32), new Point(17, 33), new Point(20, 33), new Point(37, 33), new Point(40, 33), new Point(49, 33), new Point(26, 34), new Point(31, 34), new Point(26, 35), new Point(31, 35), new Point(16, 36), new Point(25, 36), new Point(26, 36), new Point(29, 36), new Point(31, 36), new Point(32, 36), new Point(41, 36), new Point(51, 36), new Point(26, 37), new Point(28, 37), new Point(29, 37), new Point(31, 37), new Point(4, 38), new Point(8, 38), new Point(24, 38), new Point(28, 38), new Point(29, 38), new Point(33, 38), new Point(49, 38), new Point(53, 38), new Point(25, 39), new Point(29, 39), new Point(32, 39), new Point(8, 40), new Point(14, 40), new Point(49, 40), new Point(22, 41), new Point(24, 41), new Point(33, 41), new Point(35, 41), new Point(2, 43), new Point(20, 43), new Point(21, 43), new Point(22, 43), new Point(23, 43), new Point(34, 43), new Point(35, 43), new Point(36, 43), new Point(37, 43), new Point(46, 43), new Point(55, 43), new Point(9, 44), new Point(48, 44), new Point(18, 45), new Point(19, 45), new Point(20, 45), new Point(37, 45), new Point(38, 45), new Point(39, 45), new Point(1, 46), new Point(11, 46), new Point(19, 46), new Point(38, 46), new Point(46, 46), new Point(5, 49), new Point(52, 49), new Point(4, 50), new Point(5, 50), new Point(52, 50), new Point(53, 50), new Point(5, 51), new Point(23, 51), new Point(24, 51), new Point(33, 51), new Point(34, 51), new Point(52, 51), new Point(3, 52), new Point(5, 52), new Point(23, 52), new Point(24, 52), new Point(26, 52), new Point(33, 52), new Point(34, 52), new Point(52, 52), new Point(54, 52), new Point(3, 53), new Point(23, 53), new Point(34, 53), new Point(54, 53), new Point(3, 54), new Point(4, 54), new Point(53, 54), new Point(54, 54), new Point(3, 55), new Point(4, 55), new Point(7, 55), new Point(53, 55), new Point(54, 55), new Point(3, 56), new Point(4, 56), new Point(25, 56), new Point(32, 56), new Point(53, 56), new Point(54, 56), new Point(1, 57), new Point(4, 57), new Point(53, 57), new Point(56, 57), new Point(4, 58), new Point(53, 58), new Point(4, 59), new Point(53, 59), new Point(20, 60), new Point(37, 60), new Point(33, 61), new Point(37, 61), new Point(33, 62), new Point(2, 63), new Point(33, 63), new Point(55, 63), new Point(6, 64), new Point(21, 64), new Point(24, 64), new Point(33, 64), new Point(36, 64), new Point(6, 65), new Point(21, 65), new Point(33, 65), new Point(36, 65), new Point(51, 67), new Point(3, 70), new Point(25, 70), new Point(32, 70), new Point(3, 71), new Point(3, 72), new Point(54, 72), new Point(54, 73), new Point(3, 74), new Point(5, 75), new Point(52, 75), new Point(23, 76), new Point(34, 76) }),
            (Color.FromArgb(160, 160, 192), new Point[] { new Point(31, 2), new Point(28, 20), new Point(29, 20), new Point(24, 21), new Point(33, 21), new Point(6, 37), new Point(51, 37), new Point(21, 42), new Point(36, 42), new Point(22, 44), new Point(35, 44), new Point(18, 46), new Point(39, 46), new Point(17, 47), new Point(40, 47), new Point(5, 48), new Point(52, 48), new Point(11, 52), new Point(5, 53), new Point(52, 53), new Point(3, 57), new Point(54, 57) }),
            (Color.FromArgb(128, 128, 160), new Point[] { new Point(26, 3), new Point(23, 26), new Point(34, 26), new Point(26, 39), new Point(31, 39), new Point(24, 43), new Point(33, 43), new Point(17, 46), new Point(40, 46), new Point(6, 51), new Point(51, 51), new Point(52, 56), new Point(3, 58), new Point(54, 58) }),
            (Color.FromArgb(32, 32, 32), new Point[] { new Point(28, 3), new Point(29, 3), new Point(27, 4), new Point(28, 4), new Point(29, 4), new Point(30, 4), new Point(26, 9), new Point(31, 9), new Point(26, 10), new Point(28, 10), new Point(29, 10), new Point(31, 10), new Point(25, 11), new Point(28, 11), new Point(29, 11), new Point(32, 11), new Point(24, 12), new Point(28, 12), new Point(29, 12), new Point(33, 12), new Point(25, 13), new Point(32, 13), new Point(27, 14), new Point(28, 14), new Point(29, 14), new Point(30, 14), new Point(27, 16), new Point(30, 16), new Point(26, 17), new Point(27, 17), new Point(30, 17), new Point(31, 17), new Point(25, 19), new Point(26, 19), new Point(31, 19), new Point(32, 19), new Point(22, 20), new Point(35, 20), new Point(22, 21), new Point(35, 21), new Point(22, 22), new Point(26, 22), new Point(31, 22), new Point(35, 22), new Point(26, 23), new Point(31, 23), new Point(22, 24), new Point(24, 24), new Point(25, 24), new Point(26, 24), new Point(27, 24), new Point(30, 24), new Point(31, 24), new Point(32, 24), new Point(33, 24), new Point(35, 24), new Point(6, 25), new Point(22, 25), new Point(25, 25), new Point(27, 25), new Point(30, 25), new Point(32, 25), new Point(35, 25), new Point(51, 25), new Point(6, 26), new Point(25, 26), new Point(32, 26), new Point(51, 26), new Point(51, 27), new Point(27, 28), new Point(28, 28), new Point(29, 28), new Point(30, 28), new Point(27, 29), new Point(28, 29), new Point(29, 29), new Point(30, 29), new Point(24, 30), new Point(27, 30), new Point(28, 30), new Point(29, 30), new Point(30, 30), new Point(33, 30), new Point(6, 31), new Point(19, 31), new Point(24, 31), new Point(33, 31), new Point(38, 31), new Point(51, 31), new Point(19, 32), new Point(24, 32), new Point(33, 32), new Point(38, 32), new Point(18, 33), new Point(23, 33), new Point(24, 33), new Point(33, 33), new Point(34, 33), new Point(39, 33), new Point(18, 34), new Point(39, 34), new Point(18, 35), new Point(23, 35), new Point(34, 35), new Point(39, 35), new Point(22, 36), new Point(23, 36), new Point(34, 36), new Point(35, 36), new Point(21, 37), new Point(36, 37), new Point(20, 38), new Point(21, 38), new Point(22, 38), new Point(35, 38), new Point(36, 38), new Point(37, 38), new Point(19, 39), new Point(20, 39), new Point(37, 39), new Point(38, 39), new Point(18, 40), new Point(20, 40), new Point(37, 40), new Point(39, 40), new Point(4, 41), new Point(7, 41), new Point(17, 41), new Point(18, 41), new Point(19, 41), new Point(38, 41), new Point(39, 41), new Point(40, 41), new Point(50, 41), new Point(53, 41), new Point(6, 42), new Point(7, 42), new Point(16, 42), new Point(17, 42), new Point(27, 42), new Point(30, 42), new Point(40, 42), new Point(41, 42), new Point(50, 42), new Point(51, 42), new Point(53, 42), new Point(3, 43), new Point(5, 43), new Point(6, 43), new Point(7, 43), new Point(13, 43), new Point(14, 43), new Point(26, 43), new Point(31, 43), new Point(43, 43), new Point(44, 43), new Point(50, 43), new Point(51, 43), new Point(52, 43), new Point(3, 44), new Point(4, 44), new Point(5, 44), new Point(6, 44), new Point(14, 44), new Point(15, 44), new Point(25, 44), new Point(32, 44), new Point(43, 44), new Point(51, 44), new Point(52, 44), new Point(53, 44), new Point(54, 44), new Point(4, 45), new Point(5, 45), new Point(14, 45), new Point(15, 45), new Point(23, 45), new Point(25, 45), new Point(32, 45), new Point(42, 45), new Point(43, 45), new Point(52, 45), new Point(53, 45), new Point(2, 46), new Point(4, 46), new Point(5, 46), new Point(14, 46), new Point(15, 46), new Point(21, 46), new Point(23, 46), new Point(25, 46), new Point(32, 46), new Point(34, 46), new Point(36, 46), new Point(42, 46), new Point(43, 46), new Point(52, 46), new Point(53, 46), new Point(55, 46), new Point(8, 47), new Point(9, 47), new Point(14, 47), new Point(20, 47), new Point(22, 47), new Point(23, 47), new Point(25, 47), new Point(32, 47), new Point(34, 47), new Point(35, 47), new Point(37, 47), new Point(43, 47), new Point(48, 47), new Point(49, 47), new Point(3, 48), new Point(8, 48), new Point(14, 48), new Point(18, 48), new Point(22, 48), new Point(25, 48), new Point(26, 48), new Point(31, 48), new Point(32, 48), new Point(35, 48), new Point(39, 48), new Point(43, 48), new Point(49, 48), new Point(54, 48), new Point(8, 49), new Point(9, 49), new Point(10, 49), new Point(11, 49), new Point(12, 49), new Point(13, 49), new Point(14, 49), new Point(43, 49), new Point(44, 49), new Point(45, 49), new Point(46, 49), new Point(47, 49), new Point(48, 49), new Point(49, 49), new Point(8, 50), new Point(9, 50), new Point(13, 50), new Point(14, 50), new Point(16, 50), new Point(17, 50), new Point(18, 50), new Point(19, 50), new Point(21, 50), new Point(36, 50), new Point(38, 50), new Point(39, 50), new Point(40, 50), new Point(41, 50), new Point(43, 50), new Point(44, 50), new Point(48, 50), new Point(49, 50), new Point(8, 51), new Point(9, 51), new Point(13, 51), new Point(14, 51), new Point(15, 51), new Point(17, 51), new Point(18, 51), new Point(19, 51), new Point(20, 51), new Point(38, 51), new Point(39, 51), new Point(40, 51), new Point(42, 51), new Point(43, 51), new Point(44, 51), new Point(48, 51), new Point(49, 51), new Point(7, 52), new Point(8, 52), new Point(13, 52), new Point(17, 52), new Point(18, 52), new Point(19, 52), new Point(20, 52), new Point(37, 52), new Point(38, 52), new Point(39, 52), new Point(40, 52), new Point(44, 52), new Point(49, 52), new Point(50, 52), new Point(1, 53), new Point(7, 53), new Point(8, 53), new Point(14, 53), new Point(16, 53), new Point(19, 53), new Point(25, 53), new Point(32, 53), new Point(38, 53), new Point(41, 53), new Point(43, 53), new Point(49, 53), new Point(50, 53), new Point(56, 53), new Point(1, 54), new Point(19, 54), new Point(24, 54), new Point(38, 54), new Point(56, 54), new Point(12, 55), new Point(19, 55), new Point(22, 55), new Point(23, 55), new Point(24, 55), new Point(33, 55), new Point(34, 55), new Point(35, 55), new Point(38, 55), new Point(45, 55), new Point(2, 56), new Point(20, 56), new Point(21, 56), new Point(22, 56), new Point(23, 56), new Point(24, 56), new Point(33, 56), new Point(34, 56), new Point(35, 56), new Point(36, 56), new Point(37, 56), new Point(55, 56), new Point(2, 57), new Point(21, 57), new Point(22, 57), new Point(35, 57), new Point(36, 57), new Point(55, 57), new Point(21, 59), new Point(36, 59), new Point(3, 61), new Point(21, 61), new Point(22, 61), new Point(23, 61), new Point(34, 61), new Point(35, 61), new Point(54, 61), new Point(22, 62), new Point(23, 62), new Point(34, 62), new Point(35, 62), new Point(23, 63), new Point(34, 63), new Point(5, 64), new Point(22, 64), new Point(35, 64), new Point(52, 64), new Point(54, 64), new Point(4, 65), new Point(5, 65), new Point(52, 65), new Point(53, 65), new Point(4, 66), new Point(5, 66), new Point(52, 66), new Point(53, 66), new Point(4, 67), new Point(5, 67), new Point(52, 67), new Point(53, 67), new Point(23, 68), new Point(34, 68), new Point(23, 70), new Point(24, 70), new Point(33, 70), new Point(34, 70), new Point(24, 73), new Point(33, 74), new Point(24, 75), new Point(33, 75) }),
            (Color.FromArgb(128, 128, 128), new Point[] { new Point(26, 4), new Point(31, 4), new Point(25, 7), new Point(32, 7), new Point(27, 10), new Point(30, 10), new Point(27, 11), new Point(30, 11), new Point(27, 12), new Point(30, 12), new Point(34, 12), new Point(25, 15), new Point(26, 15), new Point(31, 15), new Point(32, 15), new Point(23, 16), new Point(25, 16), new Point(32, 16), new Point(34, 16), new Point(25, 17), new Point(32, 17), new Point(28, 18), new Point(29, 18), new Point(25, 20), new Point(32, 20), new Point(23, 21), new Point(34, 21), new Point(23, 23), new Point(28, 23), new Point(29, 23), new Point(34, 23), new Point(7, 28), new Point(50, 28), new Point(5, 29), new Point(7, 29), new Point(21, 29), new Point(22, 29), new Point(35, 29), new Point(36, 29), new Point(50, 29), new Point(52, 29), new Point(5, 30), new Point(18, 30), new Point(21, 30), new Point(36, 30), new Point(39, 30), new Point(21, 31), new Point(36, 31), new Point(39, 31), new Point(26, 32), new Point(28, 32), new Point(29, 32), new Point(31, 32), new Point(5, 33), new Point(22, 33), new Point(27, 33), new Point(28, 33), new Point(29, 33), new Point(30, 33), new Point(35, 33), new Point(52, 33), new Point(5, 34), new Point(6, 34), new Point(17, 34), new Point(19, 34), new Point(25, 34), new Point(32, 34), new Point(38, 34), new Point(40, 34), new Point(51, 34), new Point(5, 35), new Point(20, 35), new Point(37, 35), new Point(49, 35), new Point(52, 35), new Point(5, 36), new Point(24, 36), new Point(33, 36), new Point(52, 36), new Point(16, 37), new Point(19, 37), new Point(38, 37), new Point(41, 37), new Point(49, 37), new Point(52, 37), new Point(17, 38), new Point(19, 38), new Point(38, 38), new Point(40, 38), new Point(4, 39), new Point(6, 39), new Point(42, 39), new Point(51, 39), new Point(53, 39), new Point(16, 40), new Point(41, 40), new Point(16, 41), new Point(21, 41), new Point(36, 41), new Point(41, 41), new Point(8, 42), new Point(20, 42), new Point(25, 42), new Point(32, 42), new Point(37, 42), new Point(49, 42), new Point(19, 43), new Point(27, 43), new Point(30, 43), new Point(38, 43), new Point(49, 43), new Point(2, 44), new Point(17, 44), new Point(23, 44), new Point(34, 44), new Point(55, 44), new Point(8, 45), new Point(11, 45), new Point(12, 45), new Point(17, 45), new Point(31, 45), new Point(40, 45), new Point(45, 45), new Point(46, 45), new Point(49, 45), new Point(6, 46), new Point(16, 46), new Point(20, 46), new Point(37, 46), new Point(41, 46), new Point(51, 46), new Point(5, 47), new Point(10, 47), new Point(18, 47), new Point(39, 47), new Point(47, 47), new Point(52, 47), new Point(11, 48), new Point(12, 48), new Point(16, 48), new Point(17, 48), new Point(40, 48), new Point(41, 48), new Point(45, 48), new Point(46, 48), new Point(3, 50), new Point(23, 50), new Point(26, 50), new Point(31, 50), new Point(34, 50), new Point(54, 50), new Point(11, 51), new Point(22, 51), new Point(31, 51), new Point(35, 51), new Point(46, 51), new Point(6, 52), new Point(12, 52), new Point(45, 52), new Point(51, 52), new Point(0, 53), new Point(21, 53), new Point(24, 53), new Point(33, 53), new Point(36, 53), new Point(11, 54), new Point(14, 54), new Point(25, 54), new Point(32, 54), new Point(43, 54), new Point(46, 54), new Point(10, 55), new Point(16, 55), new Point(41, 55), new Point(47, 55), new Point(1, 56), new Point(8, 56), new Point(10, 56), new Point(11, 56), new Point(46, 56), new Point(47, 56), new Point(56, 56), new Point(5, 57), new Point(25, 57), new Point(32, 57), new Point(52, 57), new Point(5, 58), new Point(18, 58), new Point(20, 58), new Point(37, 58), new Point(52, 58), new Point(2, 59), new Point(3, 59), new Point(5, 59), new Point(24, 59), new Point(52, 59), new Point(54, 59), new Point(2, 60), new Point(51, 60), new Point(55, 60), new Point(6, 61), new Point(51, 61), new Point(4, 62), new Point(6, 62), new Point(53, 62), new Point(21, 63), new Point(36, 63), new Point(21, 66), new Point(23, 66), new Point(34, 66), new Point(36, 66), new Point(22, 67), new Point(35, 67), new Point(3, 68), new Point(21, 68), new Point(22, 68), new Point(35, 68), new Point(36, 68), new Point(21, 70), new Point(5, 73), new Point(23, 73), new Point(52, 73), new Point(23, 74), new Point(34, 74), new Point(52, 74), new Point(34, 75) }),
            (Color.FromArgb(64, 96, 96), new Point[] { new Point(26, 5) }),
            (Color.FromArgb(32, 32, 64), new Point[] { new Point(26, 11), new Point(43, 42), new Point(22, 69), new Point(35, 69) }),
            (Color.FromArgb(32, 64, 64), new Point[] { new Point(31, 11) }),
            (Color.FromArgb(96, 96, 128), new Point[] { new Point(23, 12), new Point(27, 19), new Point(30, 19), new Point(22, 28), new Point(5, 37), new Point(17, 39), new Point(40, 39), new Point(26, 40), new Point(31, 40), new Point(12, 51), new Point(45, 51), new Point(45, 53), new Point(20, 54), new Point(37, 54), new Point(36, 67), new Point(21, 69), new Point(36, 69) }),
            (Color.FromArgb(0, 0, 0), new Point[] { new Point(25, 12), new Point(32, 12), new Point(28, 13), new Point(29, 13), new Point(27, 15), new Point(30, 15), new Point(26, 18), new Point(31, 18), new Point(26, 20), new Point(31, 20), new Point(26, 21), new Point(31, 21), new Point(27, 26), new Point(30, 26), new Point(6, 27), new Point(27, 27), new Point(30, 27), new Point(6, 28), new Point(51, 28), new Point(6, 29), new Point(19, 29), new Point(38, 29), new Point(51, 29), new Point(6, 30), new Point(19, 30), new Point(38, 30), new Point(51, 30), new Point(23, 34), new Point(34, 34), new Point(22, 37), new Point(35, 37), new Point(21, 39), new Point(36, 39), new Point(19, 40), new Point(38, 40), new Point(15, 43), new Point(42, 43), new Point(54, 43), new Point(42, 44), new Point(3, 45), new Point(34, 45), new Point(54, 45), new Point(22, 46), new Point(35, 46), new Point(4, 47), new Point(24, 47), new Point(33, 47), new Point(53, 47), new Point(19, 48), new Point(38, 48), new Point(18, 49), new Point(39, 49), new Point(16, 51), new Point(41, 51), new Point(14, 52), new Point(15, 52), new Point(16, 52), new Point(41, 52), new Point(42, 52), new Point(43, 52), new Point(21, 58), new Point(36, 58), new Point(23, 60), new Point(34, 60), new Point(3, 62), new Point(54, 62), new Point(3, 63), new Point(22, 63), new Point(35, 63), new Point(54, 63), new Point(4, 68), new Point(53, 68), new Point(4, 69), new Point(23, 69), new Point(34, 69), new Point(53, 69), new Point(4, 70), new Point(53, 70), new Point(4, 71), new Point(53, 71), new Point(4, 72), new Point(53, 72), new Point(4, 73), new Point(53, 73), new Point(4, 74), new Point(53, 74) }),
            (Color.FromArgb(192, 192, 224), new Point[] { new Point(36, 22), new Point(25, 37), new Point(32, 37), new Point(25, 38), new Point(32, 38), new Point(23, 40), new Point(34, 40), new Point(23, 42), new Point(34, 42) }),
            (Color.FromArgb(224, 224, 224), new Point[] { new Point(36, 25), new Point(28, 39), new Point(4, 51), new Point(53, 51), new Point(50, 55), new Point(32, 71), new Point(54, 71) }),
            (Color.FromArgb(96, 128, 128), new Point[] { new Point(24, 27), new Point(33, 27), new Point(23, 31), new Point(34, 31), new Point(40, 44) }),
            (Color.FromArgb(192, 224, 224), new Point[] { new Point(38, 28) }),
            (Color.FromArgb(160, 192, 192), new Point[] { new Point(23, 30), new Point(6, 36), new Point(25, 40), new Point(32, 40) }),
            (Color.FromArgb(64, 64, 96), new Point[] { new Point(25, 31), new Point(32, 31), new Point(7, 45), new Point(41, 45), new Point(44, 45), new Point(9, 54), new Point(48, 54) }),
            (Color.FromArgb(32, 32, 0), new Point[] { new Point(37, 51) }),
            (Color.FromArgb(0, 0, 32), new Point[] { new Point(33, 54) }),
            (Color.FromArgb(128, 160, 160), new Point[] { new Point(5, 56) }),
        };

        // Método para pintar la textura del avión con soporte para rotación
        public void PintarTexturaAvion(Graphics g, int AngRotar, int largoN)
        {
            foreach (var capa in capasColores)
            {
                using (SolidBrush brush = new SolidBrush(capa.color))
                {
                    foreach (var p in capa.puntos)
                    {
                        int posY = (AngRotar == 180) ? (largoN - 1 - p.Y) : p.Y;
                        g.FillRectangle(brush, p.X, posY, 1, 1);
                    }
                }
            }
        }

        //*********** DIAGRAMAR NAVE***********//
        public void CrearNave(PictureBox Avion, int AngRotar, int Tipox, Color Pintar, int Vida)
        {
            int largoN = 0;
            int anchoN = 0;
            Point[] myNave1 = { new Point(29, 0), new Point(30, 1), new Point(30, 6), new Point(31, 6), new Point(31, 11), new Point(32, 11), new Point(32, 17), new Point(35, 17), new Point(35, 16), new Point(37, 16), new Point(37, 17), new Point(38, 18), new Point(38, 28), new Point(39, 28), new Point(42, 39), new Point(45, 45), new Point(50, 51), new Point(51, 51), new Point(51, 52), new Point(58, 59), new Point(58, 66), new Point(39, 66), new Point(39, 71), new Point(35, 71), new Point(35, 74), new Point(32, 74), new Point(32, 77), new Point(26, 77), new Point(26, 74), new Point(23, 74), new Point(23, 71), new Point(19, 71), new Point(19, 66), new Point(0, 66), new Point(0, 59), new Point(7, 52), new Point(7, 51), new Point(8, 51), new Point(13, 45), new Point(16, 39), new Point(19, 28), new Point(20, 28), new Point(20, 18), new Point(21, 17), new Point(21, 16), new Point(23, 16), new Point(23, 17), new Point(26, 17), new Point(26, 11), new Point(27, 11), new Point(27, 6), new Point(28, 6), new Point(28, 1), new Point(29, 0) };
            Point[] myNave2 = { new Point(24, 0), new Point(29, 5), new Point(29, 18), new Point(32, 21), new Point(34, 21), new Point(38, 17), new Point(41, 20), new Point(41, 30), new Point(47, 36), new Point(47, 41), new Point(41, 41), new Point(38, 44), new Point(36, 44), new Point(33, 41), new Point(30, 41), new Point(25, 46), new Point(22, 46), new Point(17, 41), new Point(14, 41), new Point(11, 44), new Point(9, 44), new Point(6, 41), new Point(0, 41), new Point(0, 36), new Point(6, 30), new Point(6, 20), new Point(9, 17), new Point(13, 21), new Point(15, 21), new Point(18, 18), new Point(18, 5), new Point(23, 0) };
            Point[] myNave3 = { new Point(25, 54), new Point(26, 54), new Point(26, 50), new Point(27, 50), new Point(28, 50), new Point(29, 50), new Point(30, 51), new Point(31, 51), new Point(32, 52), new Point(32, 49), new Point(31, 48), new Point(30, 47), new Point(29, 46), new Point(28, 45), new Point(27, 44), new Point(27, 36), new Point(28, 35), new Point(28, 25), new Point(29, 25), new Point(30, 25), new Point(31, 25), new Point(32, 26), new Point(33, 26), new Point(34, 27), new Point(35, 28), new Point(36, 28), new Point(37, 29), new Point(38, 30), new Point(39, 30), new Point(40, 31), new Point(41, 32), new Point(42, 32), new Point(43, 33), new Point(44, 34), new Point(45, 35), new Point(46, 36), new Point(47, 36), new Point(48, 36), new Point(49, 37), new Point(50, 37), new Point(51, 38), new Point(51, 37), new Point(51, 36), new Point(51, 35), new Point(50, 35), new Point(37, 22), new Point(37, 15), new Point(36, 14), new Point(35, 14), new Point(34, 15), new Point(34, 21), new Point(28, 15), new Point(28, 7), new Point(27, 6), new Point(26, 5), new Point(25, 5), new Point(24, 6), new Point(23, 7), new Point(23, 15), new Point(17, 21), new Point(17, 15), new Point(16, 14), new Point(15, 14), new Point(14, 15), new Point(14, 22), new Point(1, 35), new Point(0, 35), new Point(0, 36), new Point(0, 37), new Point(0, 38), new Point(1, 37), new Point(2, 37), new Point(3, 36), new Point(4, 36), new Point(5, 36), new Point(6, 35), new Point(7, 34), new Point(8, 33), new Point(9, 32), new Point(10, 32), new Point(11, 31), new Point(12, 30), new Point(13, 30), new Point(14, 29), new Point(15, 28), new Point(16, 28), new Point(17, 27), new Point(18, 26), new Point(19, 26), new Point(20, 25), new Point(21, 25), new Point(22, 25), new Point(23, 25), new Point(24, 25), new Point(24, 35), new Point(24, 36), new Point(23, 44), new Point(22, 45), new Point(22, 46), new Point(21, 47), new Point(20, 48), new Point(19, 49), new Point(19, 52), new Point(20, 51), new Point(21, 51), new Point(22, 50), new Point(23, 50), new Point(24, 50), new Point(25, 50), new Point(25, 54) };
            // Puntos extraídos de avion.png
            Point[] myNave4 = { new Point(28, 0), new Point(31, 2), new Point(35, 14), new Point(36, 28), new Point(39, 29), new Point(42, 40), new Point(47, 44), new Point(49, 43), new Point(49, 33), new Point(52, 24), new Point(52, 37), new Point(55, 43), new Point(54, 48), new Point(57, 51), new Point(53, 75), new Point(51, 56), new Point(45, 56), new Point(42, 53), new Point(40, 58), new Point(38, 56), new Point(37, 57), new Point(36, 70), new Point(34, 71), new Point(33, 76), new Point(33, 55), new Point(31, 52), new Point(32, 46), new Point(29, 39), new Point(27, 40), new Point(25, 46), new Point(26, 52), new Point(23, 62), new Point(23, 68), new Point(25, 70), new Point(24, 76), new Point(23, 71), new Point(21, 70), new Point(20, 57), new Point(19, 56), new Point(17, 58), new Point(15, 53), new Point(12, 56), new Point(6, 56), new Point(5, 75), new Point(3, 74), new Point(3, 64), new Point(0, 51), new Point(3, 48), new Point(1, 46), new Point(5, 37), new Point(5, 25), new Point(6, 24), new Point(7, 27), new Point(8, 43), new Point(10, 44), new Point(16, 38), new Point(18, 29), new Point(21, 29), new Point(21, 17), new Point(28, 0) };
            Point[] myNave;

            //***********INSERTAR EL OBJETO************//
            GraphicsPath ObjGrafico = new GraphicsPath();

            if (Tipox == 1)
            {
                largoN = 77;
                anchoN = 58;
                // rotar
                myNave = new Point[myNave1.Count()];
                for (int i = 0; i < myNave1.Count(); i++)
                {
                    myNave[i].X = myNave1[i].X;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave1[i].Y;
                    else
                        myNave[i].Y = myNave1[i].Y;
                }
                ObjGrafico.AddPolygon(myNave);
            }
            else if (Tipox == 2)
            {
                largoN = 46 * 1;
                anchoN = 47 * 1;
                // rotar
                myNave = new Point[myNave2.Count()];
                for (int i = 0; i < myNave2.Count(); i++)
                {
                    myNave[i].X = myNave2[i].X * 1;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave2[i].Y * 1;
                    else
                        myNave[i].Y = myNave2[i].Y * 1;
                }
                //ObjGrafico.AddPolygon(myNave);
                ObjGrafico.AddLines(myNave);
            }
            else if (Tipox == 3)
            {
                largoN = 54;
                anchoN = 51; // rotar
                myNave = new Point[myNave3.Count()];
                for (int i = 0; i < myNave3.Count(); i++)
                {
                    myNave[i].X = myNave3[i].X;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave3[i].Y;
                    else
                        myNave[i].Y = myNave3[i].Y;
                }
                ObjGrafico.AddPolygon(myNave);
            }
            else if (Tipox == 4)
            {
                largoN = 77;
                anchoN = 58;
                // rotar
                myNave = new Point[myNave4.Count()];
                for (int i = 0; i < myNave4.Count(); i++)
                {
                    myNave[i].X = myNave4[i].X;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNave4[i].Y;
                    else
                        myNave[i].Y = myNave4[i].Y;
                }
                ObjGrafico.AddPolygon(myNave);
            }

            Avion.BackColor = Pintar;
            Avion.Size = new Size(anchoN, largoN);
            Avion.Region = new Region(ObjGrafico);
            Avion.Location = new Point(0, 0);
            //*********INSERTAR LA NAVE AL CONTENDOR***********//
            contiene.Controls.Add(Avion);

            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            
            if (Tipox == 4)
            {
                PintarTexturaAvion(PintaImg, AngRotar, largoN);
            }
            else
            {
                PintaImg.DrawPolygon(Pens.Black, ObjGrafico.PathData.Points);
            }

            Avion.Image = Imagen;
            //NaveCorre(Avion, Tipox, AngRotar);
            Avion.Tag = Vida;
            Avion.Visible = true;
        }

        //*********** EFECTOS DE LA NAVE PRINCIPAL ***********//
        public void NaveCorre(PictureBox Avion, int AngRotar, int velox)
        {
            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            Point[] puntoDer = { new Point(35, 28), new Point(35, 30), new Point(36, 30), new Point(37, 31), new Point(37, 37), new Point(38, 38), new Point(38, 40), new Point(39, 41), new Point(39, 44), new Point(40, 45), new Point(40, 46), new Point(42, 48), new Point(43, 48), new Point(44, 49), new Point(44, 64), new Point(43, 65), new Point(42, 65), new Point(41, 66), new Point(40, 66), new Point(38, 68), new Point(36, 69), new Point(36, 68), new Point(36, 63), new Point(35, 62), new Point(35, 30) };
            Point[] puntoIzq = { new Point(23, 28), new Point(23, 30), new Point(22, 30), new Point(21, 31), new Point(21, 37), new Point(20, 38), new Point(20, 40), new Point(19, 41), new Point(19, 44), new Point(18, 45), new Point(18, 46), new Point(16, 48), new Point(15, 48), new Point(14, 49), new Point(14, 64), new Point(15, 65), new Point(16, 65), new Point(17, 66), new Point(18, 66), new Point(20, 68), new Point(22, 69), new Point(22, 68), new Point(22, 63), new Point(23, 62), new Point(23, 28) };
            Point[] puntoAtr = { new Point(31, 19), new Point(29, 21), new Point(33, 25), new Point(33, 20), new Point(32, 19), new Point(34, 26), new Point(32, 63), new Point(32, 65), new Point(34, 68), new Point(33, 69), new Point(33, 74), new Point(32, 73), new Point(31, 73), new Point(29, 71), new Point(27, 73), new Point(26, 73), new Point(25, 74), new Point(25, 69), new Point(24, 68), new Point(26, 65), new Point(26, 63), new Point(24, 26), new Point(25, 25), new Point(29, 21), new Point(27, 20), new Point(26, 19), new Point(25, 19) };

            PintaImg.FillPolygon(Brushes.DarkGreen, puntoDer);
            PintaImg.FillPolygon(Brushes.DarkGreen, puntoIzq);
            PintaImg.FillPolygon(Brushes.DarkGreen, puntoAtr);
            PintaImg.FillRectangle(Brushes.Silver, 35, 1, 25, 15);
            PintaImg.FillRectangle(Brushes.Silver, 35, 32, 1, 15);
            PintaImg.FillRectangle(Brushes.Silver, 29, 1, 58, 13);

            if (velox == 1)
            {
                PintaImg.FillRectangle(Brushes.DarkOrange, 35, 68, 6, 1);
                PintaImg.FillRectangle(Brushes.Orange, 36, 69, 4, 1);
                PintaImg.FillRectangle(Brushes.Yellow, 37, 70, 2, 1);
                PintaImg.FillRectangle(Brushes.DarkOrange, 17, 68, 6, 1);
                PintaImg.FillRectangle(Brushes.Orange, 18, 69, 4, 1);
                PintaImg.FillRectangle(Brushes.Yellow, 19, 70, 2, 1);
                RotateImage(navex.Image, angulo);
            }
            else if (velox == 2)
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 15, 30, 1, 8);
                PintaImg.FillRectangle(Brushes.DarkRed, 25, 28, 1, 16);
                PintaImg.FillRectangle(Brushes.DarkRed, 35, 30, 1, 8);
            }
            else if (velox == 3)
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 15, 30, 1, 8);
                PintaImg.FillRectangle(Brushes.DarkRed, 25, 28, 1, 16);
                PintaImg.FillRectangle(Brushes.DarkRed, 35, 30, 1, 8);
            }
            Avion.Image = RotateImage(Imagen, AngRotar);
        }

        //**********CREAR ANGULO DE ROTACION******************//
        public static Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            return bmp;
        }

        //*********** MOVIMIENTO DEL TECLADO DEL USUARIO ***********//
        public void ActividadTecla(object sender, KeyEventArgs e)
        {
            //INSTRUCCIONES DE LOS BOTONES
            switch (e.KeyValue)
            {
                case 37: // flecha hacia la izquierda
                    {
                        if (contiene.Left < navex.Left) // PARAMETROS DE LIMITE
                        {
                            navex.Left -= 10;
                            angulo = -15;
                            NaveCorre(navex, 1, 0);
                        }
                        break;
                    }
                case 38: // flecha hacia arriba
                    {
                        if (contiene.Top < navex.Top)
                        {
                            navex.Top -= 10;
                            NaveCorre(navex, 0, 1);
                        }
                        break;
                    }
                case 39: // flecha hacia la derecha
                    {
                        if (contiene.Right > navex.Right)
                        {
                            navex.Left += 10;
                            angulo = +15;
                            NaveCorre(navex, 1, 0);
                        }
                        break;
                    }
                case 40: // flecha hacia abajo
                    {
                        if (contiene.Bottom > navex.Bottom)
                        {
                            navex.Top += 10;
                            NaveCorre(navex, 0, 1);
                        }
                        break;
                    }
                case 13:
                    {
                        tiempo.Start();
                        int x = navex.Location.X + (navex.Width / 2);
                        int y = navex.Location.Y + (navex.Height / 2);
                        CrearMisil(0, Color.DarkMagenta, "Misil", x, y);
                        break;
                    }
            }
        }

        //*************** ACTIVAR ACCIONES DE INICIALIZACION **********************//
        public void Iniciar()
        {
            //***************************************//
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.Width = 400;
            this.Height = 600;
            this.Text = "JUEGO DE AVIONES BASICO";
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2 });
            toolStripStatusLabel1.Text = "Mi Rival: 50 | ";
            toolStripStatusLabel2.Text = "Mi Avion: 20";
            Controls.Add(statusStrip1);
            this.KeyDown += new KeyEventHandler(ActividadTecla);
            //***************************************//
            contiene.Location = new Point(0, 0);
            contiene.BackColor = Color.AliceBlue;
            contiene.Size = new Size(400, 600);
            contiene.Dock = DockStyle.Fill;
            Controls.Add(contiene);
            contiene.Visible = true;

            // contenido del formulario.
            Random r = new Random();
            int aleaty = r.Next(250, 330);
            int aleatx = r.Next(50, 250);
            CrearNave(navex, 0, 4, Color.SeaGreen, 20);
            //ELEGIR NAVE DE SALIDA RIVAL
            Random sal = new Random();
            int sale = sal.Next(1, 5);
            CrearNave(naveRival, 180, sale, Color.DarkBlue, 50);
            //Modulo.Escenario(contiene, sale);
            navex.Location = new Point(aleatx, aleaty);

            tiempo = new System.Windows.Forms.Timer();
            tiempo.Interval = 5;
            tiempo.Enabled = true;
            tiempo.Tick += new EventHandler(ImpactarTick);

            this.KeyPreview = true;
        }

        public Form1()
        {
            InitializeComponent();
            Iniciar();
        }
    }
}