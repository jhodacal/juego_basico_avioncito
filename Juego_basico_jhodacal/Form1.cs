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

        //*********** DIAGRAMAR NAVE***********//
        public void CrearNave(PictureBox Avion, int AngRotar, int Tipox, Color Pintar, int Vida)
        {
            int largoN = 0;
            int anchoN = 0;
            Point[] myNave1 = { new Point(29, 0), new Point(30, 1), new Point(30, 6), new Point(31, 6), new Point(31, 11), new Point(32, 11), new Point(32, 17), new Point(35, 17), new Point(35, 16), new Point(37, 16), new Point(37, 17), new Point(38, 18), new Point(38, 28), new Point(39, 28), new Point(42, 39), new Point(45, 45), new Point(50, 51), new Point(51, 51), new Point(51, 52), new Point(58, 59), new Point(58, 66), new Point(39, 66), new Point(39, 71), new Point(35, 71), new Point(35, 74), new Point(32, 74), new Point(32, 77), new Point(26, 77), new Point(26, 74), new Point(23, 74), new Point(23, 71), new Point(19, 71), new Point(19, 66), new Point(0, 66), new Point(0, 59), new Point(7, 52), new Point(7, 51), new Point(8, 51), new Point(13, 45), new Point(16, 39), new Point(19, 28), new Point(20, 28), new Point(20, 18), new Point(21, 17), new Point(21, 16), new Point(23, 16), new Point(23, 17), new Point(26, 17), new Point(26, 11), new Point(27, 11), new Point(27, 6), new Point(28, 6), new Point(28, 1), new Point(29, 0) };
            Point[] myNave2 = { new Point(24, 0), new Point(29, 5), new Point(29, 18), new Point(32, 21), new Point(34, 21), new Point(38, 17), new Point(41, 20), new Point(41, 30), new Point(47, 36), new Point(47, 41), new Point(41, 41), new Point(38, 44), new Point(36, 44), new Point(33, 41), new Point(30, 41), new Point(25, 46), new Point(22, 46), new Point(17, 41), new Point(14, 41), new Point(11, 44), new Point(9, 44), new Point(6, 41), new Point(0, 41), new Point(0, 36), new Point(6, 30), new Point(6, 20), new Point(9, 17), new Point(13, 21), new Point(15, 21), new Point(18, 18), new Point(18, 5), new Point(23, 0) };
            Point[] myNave3 = { new Point(25, 54), new Point(26, 54), new Point(26, 50), new Point(27, 50), new Point(28, 50), new Point(29, 50), new Point(30, 51), new Point(31, 51), new Point(32, 52), new Point(32, 49), new Point(31, 48), new Point(30, 47), new Point(29, 46), new Point(28, 45), new Point(27, 44), new Point(27, 36), new Point(28, 35), new Point(28, 25), new Point(29, 25), new Point(30, 25), new Point(31, 25), new Point(32, 26), new Point(33, 26), new Point(34, 27), new Point(35, 28), new Point(36, 28), new Point(37, 29), new Point(38, 30), new Point(39, 30), new Point(40, 31), new Point(41, 32), new Point(42, 32), new Point(43, 33), new Point(44, 34), new Point(45, 35), new Point(46, 36), new Point(47, 36), new Point(48, 36), new Point(49, 37), new Point(50, 37), new Point(51, 38), new Point(51, 37), new Point(51, 36), new Point(51, 35), new Point(50, 35), new Point(37, 22), new Point(37, 15), new Point(36, 14), new Point(35, 14), new Point(34, 15), new Point(34, 21), new Point(28, 15), new Point(28, 7), new Point(27, 6), new Point(26, 5), new Point(25, 5), new Point(24, 6), new Point(23, 7), new Point(23, 15), new Point(17, 21), new Point(17, 15), new Point(16, 14), new Point(15, 14), new Point(14, 15), new Point(14, 22), new Point(1, 35), new Point(0, 35), new Point(0, 36), new Point(0, 37), new Point(0, 38), new Point(1, 37), new Point(2, 37), new Point(3, 36), new Point(4, 36), new Point(5, 36), new Point(6, 35), new Point(7, 34), new Point(8, 33), new Point(9, 32), new Point(10, 32), new Point(11, 31), new Point(12, 30), new Point(13, 30), new Point(14, 29), new Point(15, 28), new Point(16, 28), new Point(17, 27), new Point(18, 26), new Point(19, 26), new Point(20, 25), new Point(21, 25), new Point(22, 25), new Point(23, 25), new Point(24, 25), new Point(24, 35), new Point(24, 36), new Point(23, 44), new Point(22, 45), new Point(22, 46), new Point(21, 47), new Point(20, 48), new Point(19, 49), new Point(19, 52), new Point(20, 51), new Point(21, 51), new Point(22, 50), new Point(23, 50), new Point(24, 50), new Point(25, 50), new Point(25, 54) };
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

            Avion.BackColor = Pintar;
            Avion.Size = new Size(anchoN, largoN);
            Avion.Region = new Region(ObjGrafico);
            Avion.Location = new Point(0, 0);
            //*********INSERTAR LA NAVE AL CONTENDOR***********//
            contiene.Controls.Add(Avion);

            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            Point[] Colorea = { new Point(24, 2), new Point(27, 5), new Point(27, 18), new Point(31, 22), new Point(34, 22), new Point(37, 19), new Point(38, 19), new Point(39, 20), new Point(39, 30), new Point(45, 36), new Point(45, 39), new Point(41, 39), new Point(38, 42), new Point(35, 42), new Point(32, 39), new Point(30, 39), new Point(25, 44), new Point(21, 44), new Point(16, 39), new Point(14, 39), new Point(11, 42), new Point(8, 42), new Point(5, 39), new Point(1, 39), new Point(1, 36), new Point(7, 30), new Point(7, 20), new Point(8, 19), new Point(9, 19), new Point(12, 22), new Point(15, 22), new Point(19, 18), new Point(19, 5), new Point(22, 2) };

            //PintaImg.FillPolygon(Brushes.DarkGreen, Colorea);
            Point[] poix = new Point[myNave2.Count()];
            for (int i = 0; i < myNave2.Count(); i++)
            {
                poix[i].X = myNave2[i].X;
                poix[i].Y = myNave2[i].Y;
            }
            PintaImg.DrawPolygon(Pens.Black, ObjGrafico.PathData.Points);
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
            CrearNave(navex, 0, 1, Color.SeaGreen, 20);
            //ELEGIR NAVE DE SALIDA RIVAL
            Random sal = new Random();
            int sale = sal.Next(1, 3);
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