using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;

namespace MiniTrading
{
    public partial class Form1 : Form
    {
        public RedisHelper m_redisConn = new RedisHelper();

        public Form1()
        {
            InitializeComponent();

            this.chart_MarketData.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;//仅不显示x轴方向的网格线
            this.chart_MarketData.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;//仅不显示y轴方向的网格线

            this.chart_KD.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;//仅不显示x轴方向的网格线
            this.chart_KD.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;//仅不显示y轴方向的网格线

            this.TestMarketData();

            this.Sub();
        }

        /// <summary>
        /// 测试画图数据
        /// </summary>
        private void TestMarketData()
        {
            //表1
            double[] y1 = { 12200, 12225, 14000, 13000 };

            this.chart_MarketData.Series["Series_K"].Points.Add(y1);
            this.chart_MarketData.Series["Series_MA"].Points.Add(13000);

            //表2
            this.chart_KD.Series["Series_80"].Points.Add(80);
            this.chart_KD.Series["Series_80"].Points.Add(80);
            this.chart_KD.Series["Series_80"].Points.Add(80);
            this.chart_KD.Series["Series_20"].Points.Add(20);
            this.chart_KD.Series["Series_20"].Points.Add(20);
            this.chart_KD.Series["Series_20"].Points.Add(20);

            this.chart_KD.Series["Series_K"].Points.Add(80);
            this.chart_KD.Series["Series_D"].Points.Add(76);
        }

        /// <summary>
        /// 测试发布数据
        /// </summary>
        private void TestPublishData()
        {
            Random random1 = new Random();
            int randomData = random1.Next(0, 1001);

            //for (int i = 0; i < 10; i++)
            {
                RealMarketMCData data = new RealMarketMCData();
                data.NowBarInfo = new double[] { randomData, randomData, randomData, randomData, randomData, 30, 20 };

                Tester.m_redisConn.Publish<RealMarketMCData>(Entity.CHANNEL, data);
                //System.Threading.Thread.Sleep(1000);
            }

        }
        public void Sub()
        {
            m_redisConn.Subscribe(Entity.CHANNEL, (channel, message) =>
            {
                RealMarketMCData data = RedisHelper.Deserialize<RealMarketMCData>(message);

                if (data == null) return;

                RefreashChartData(data);
            });

            m_redisConn.Subscribe(Entity.CHANNELTRADE, (channel, message) =>
            {
                string tradeInfo = RedisHelper.Deserialize<string>(message);
                Debug.WriteLine(tradeInfo);
            });
        }

        private void RefreashChartData(RealMarketMCData data)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<RealMarketMCData>(RefreashChartData),data);
                return;
            }

            if (data == null) return;
            if (data.ThreeBarAgoInfo == null) return;
            if (data.TwoBarAgoInfo == null) return;
            if (data.NowBarInfo == null) return;

            //清空
            this.chart_MarketData.Series["Series_K"].Points.Clear();
            this.chart_MarketData.Series["Series_MA"].Points.Clear();
            this.chart_KD.Series["Series_80"].Points.Clear();
            this.chart_KD.Series["Series_20"].Points.Clear();
            this.chart_KD.Series["Series_K"].Points.Clear();
            this.chart_KD.Series["Series_D"].Points.Clear();

            double[] bar1Info = data.NowBarInfo;

            double[] bar1AgoOHLC = new double[] { bar1Info[0], bar1Info[1], bar1Info[2], bar1Info[3] };
            double bar1Avg = bar1Info[4];
            double bar1K = bar1Info[5];
            double bar1D = bar1Info[6];

            this.chart_MarketData.Series["Series_K"].Points.Add(bar1AgoOHLC);

            this.chart_MarketData.Series["Series_MA"].Points.Add(bar1Avg);

            //表2
            this.chart_KD.Series["Series_80"].Points.Add(80);
            this.chart_KD.Series["Series_80"].Points.Add(80);
            this.chart_KD.Series["Series_80"].Points.Add(80);
            this.chart_KD.Series["Series_20"].Points.Add(20);
            this.chart_KD.Series["Series_20"].Points.Add(20);
            this.chart_KD.Series["Series_20"].Points.Add(20);

            this.chart_KD.Series["Series_K"].Points.Add(bar1K);
            this.chart_KD.Series["Series_D"].Points.Add(bar1D);

        }

        /// <summary>
        /// Buy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Tester.m_redisConn.Publish<string>(Entity.CHANNELTRADE, "B");
        }

        /// <summary>
        /// Sell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Tester.m_redisConn.Publish<string>(Entity.CHANNELTRADE, "S");
        }

        /// <summary>
        /// Cover
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Tester.m_redisConn.Publish<string>(Entity.CHANNELTRADE, "C");
        }

        /// <summary>
        /// Put Order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool m_isPutOrderNow = false;

        private void toolStripButton4_ClickAsync(object sender, EventArgs e)
        {
            m_isPutOrderNow = true;
            TestPublishData();
        }

        private void ChartK_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Cross;
            int _currentPointX = e.X;
            int _currentPointY = e.Y;

            //MsChart.Refresh();没啥效果

            this.chart_MarketData.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(_currentPointX, _currentPointY), true);
            this.chart_MarketData.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(_currentPointX, _currentPointY), true);
            //Application.DoEvents(); 使用此方法当有线程操作时会引发异常
        }

        private void ChartK_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void ChartK_MouseClick(object sender, MouseEventArgs e)
        {
            //在点击了P的前提下，开始划线挂单
            if (m_isPutOrderNow)
            {
                int i = 0;

                m_isPutOrderNow = false;
            }
            else
            {
                //Do null
            }
        }
    }

}
