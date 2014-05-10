using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ChaosTest
{
    public class Controller : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event raised to notify that a property has changed
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Properties

        private bool mIsRunning;

        public bool IsRunning
        {
            get { return mIsRunning; }
            set
            {
                if (value == mIsRunning)
                    return;
                mIsRunning = value;

                OnPropertyChanged("IsRunning");
                OnPropertyChanged("IsReady");

            }
        }

        public bool IsReady
        {
            get { return !mIsRunning; }
        }

        private int mInterval = 1;

        public int Interval
        {
            get { return mInterval; }
            set
            {
                if (value == mInterval)
                    return;
                mInterval = value;

                OnPropertyChanged("Interval");
            }
        }

        private double mDenominator = 2;

        public double Denominator
        {
            get { return mDenominator; }
            set
            {
                if (value == mDenominator)
                    return;
                mDenominator = value;

                OnPropertyChanged("Denominator");
            }
        }

        private int mNumberOfVertices = 3;

        public int NumberOfVertices
        {
            get { return mNumberOfVertices; }
            set
            {
                if (value == mNumberOfVertices)
                    return;
                mNumberOfVertices = value;

                OnPropertyChanged("NumberOfVertices");
            }
        }

        List<Point> mVertices = new List<Point>();
        public Point TowardsVertix { get; set; }
        public Point LastPoint { get; set; }

        private int mWidth;
        private int mHeight;
        Random mRandom = new Random();

        #endregion

        public static event Action<int, int> DrawPixel;

        public void Run(int height, int width)
        {
            IsRunning = true;
            mHeight = height;
            mWidth = width;
            CalcPolygon();
                //new List<Point>()
            //                {
            //                    new Point(0,height),
            //                    new Point((float)width/2,0),
            //                    new Point(width,height)
            //                    //new Point(0,0),
            //                    //new Point(width,height),
            //                    //new Point(0,height),
            //                    //new Point(width,0)


            //                };
            LastPoint = new Point((float)width / 2, (float)height / 2);
            new Thread(DoWork) {IsBackground = true, Name = "WorkerThread"}.Start();
        }

        private void CalcPolygon()
        {
            mVertices = new List<Point>();
            var angle = 2*Math.PI/NumberOfVertices;
            double mid = ((double) mHeight/2);
            for (int i = 0; i < NumberOfVertices; i++)
            {
                var x = mid + mid * Math.Sin(i * angle);
                var y = mid + mid * Math.Cos(i * angle);
                mVertices.Add(new Point(x,y));
            }
        }

        public void Reset()
        {
            IsRunning = false;
        }

        private void DoWork()
        {
            while (mIsRunning)
            {
                Point nextPoint = CalcNextPoint();
                int x = (int) nextPoint.X;
                int y = (int) nextPoint.Y;
                if (x > mWidth || y > mHeight)
                    Debugger.Break();
                if (DrawPixel != null)
                    MainWindow.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => DrawPixel(x, y)));
                LastPoint = nextPoint;

                Thread.Sleep(mInterval);
            }
        }

        private Point CalcNextPoint()
        {
            TowardsVertix = mVertices[mRandom.Next(mVertices.Count)];
            Point distance = new Point((Math.Max(TowardsVertix.X,LastPoint.X) - Math.Min(TowardsVertix.X,LastPoint.X)) * (1 - 1 / Denominator),
                                       (Math.Max(TowardsVertix.Y,LastPoint.Y) - Math.Min(TowardsVertix.Y,LastPoint.Y)) * (1 - 1 / Denominator));
            Point nextPoint = new Point(LastPoint.X + distance.X * Math.Sign(TowardsVertix.X - LastPoint.X),
                                        LastPoint.Y + distance.Y * Math.Sign(TowardsVertix.Y - LastPoint.Y));
            return nextPoint;
        }

    }
}
