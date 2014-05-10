using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace ChaosTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller Controller
        {
            get { return DataContext as Controller; }
        }

        private int mColorData;

        public static Dispatcher CurrentDispatcher { get; set; }

        static public WriteableBitmap Bitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Controller();
            Controller.DrawPixel += DrawPixel;
            CurrentDispatcher = this.Dispatcher;
            // Compute the pixel's color.
            mColorData = 255 << 16; // R
            mColorData |= 255 << 8;   // G
            mColorData |= 255 << 0;   // B

        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (Controller.IsRunning)
            {
                btnReset_Click(null, e);
                Thread.Sleep(100);
            }
            Controller.Run((int)Bitmap.Width, (int)Bitmap.Height);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Controller.Reset();
            InitBitmap();
        }

        private void Border_Loaded_1(object sender, RoutedEventArgs e)
        {
            InitBitmap();
        }

        private void InitBitmap()
        {
            Bitmap = new WriteableBitmap(
                (int) imgContainer.ActualWidth,
                (int) imgContainer.ActualHeight,
                96,
                96,
                PixelFormats.Bgr32,
                null);
            img.Source = Bitmap;
        }

        // The DrawPixel method updates the WriteableBitmap by using
        // unsafe code to write a pixel into the back buffer.
        public void DrawPixel(int x, int y)
        {
            if (!Controller.IsRunning)
                return;
            int column = x;
            int row = y;

            // Reserve the back buffer for updates.
            Bitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer.
                int pBackBuffer = (int)Bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += row * Bitmap.BackBufferStride;
                pBackBuffer += column * 4;

                

                // Assign the color data to the pixel.
                *((int*)pBackBuffer) = mColorData;
            }

            // Specify the area of the bitmap that changed.
            Bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));

            // Release the back buffer and make it available for display.
            Bitmap.Unlock();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (Equals(sender, btnDenominatorUp))
                Controller.Denominator++;
            else if (Equals(sender, btnDenominatorDown))
                Controller.Denominator--;
            else if (Equals(sender, btnVerticesUp))
                Controller.NumberOfVertices++;
            else if (Equals(sender, btnVerticesDown))
                Controller.NumberOfVertices--;
            btnRun_Click(null,e);
        }

    }
}
