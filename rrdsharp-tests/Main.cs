using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using RrdSharp.Core;
using RrdSharp.Graph;

namespace RrdSharpTests
{
	class RrdSharpTest
	{
		static void Main(string[] args)
		{
        
			RrdSharpTest rstest = new RrdSharpTest();
            //rstest.Test1();
            //rstest.Test2();
            //rstest.Test3();
			//rstest.Test4();
            //rstest.JonasTest();
            rstest.JonasTest2();

			// Uncomment to run the stress-test.  Requires stress-test.txt sample data
			// from the RrdSharp downloads page
			//rstest.StressTest();
			
	
		}

        public void JonasTest()
        {
            UInt32 start_time = 1370000000; // 31th may 2013, 11:33:20
            UInt32 end_time = start_time + 300 * 300;
            RrdDef rrdDef = new RrdDef("jonas.rrd");
            rrdDef.StartTime = start_time;
            rrdDef.AddDatasource("battery", "GAUGE", 600, 1.0, 5.0); // Heartbeat with in 10 minutes
            rrdDef.AddArchive("LAST", 0, 1, 576);    // stepsize 300
            rrdDef.AddArchive("AVERAGE", 0, 1, 576); 
            rrdDef.AddArchive("AVERAGE", 0, 7, 576); 
            rrdDef.AddArchive("MIN", 0, 7, 576);     
            rrdDef.AddArchive("MAX", 0, 7, 576);     
            rrdDef.AddArchive("AVERAGE", 0, 30, 576);
            rrdDef.AddArchive("MIN", 0, 30, 576);
            rrdDef.AddArchive("MAX", 0, 30, 576);
            rrdDef.AddArchive("AVERAGE", 0, 360, 576);
            rrdDef.AddArchive("MIN", 0, 360, 576);
            rrdDef.AddArchive("MAX", 0, 360, 576);
            RrdDb db = new RrdDb(rrdDef);
            db.Close();

            RrdDb rrd = new RrdDb("jonas.rrd");
            Random rand = new Random();
            //Sample s = rrd.CreateSample();
            // update
            for (long t = start_time + 300; t <= end_time; t += 304)
            {
                double r = (double)rand.Next(1, 6);
                //s.SetAndUpdate(String.Format("{0}:{1}", t, r));
                Sample sample = rrd.CreateSample(t);
                sample.SetValue("battery", (double)r);
                sample.Update();
                Console.WriteLine(r);
            }

            FetchRequest fr = rrd.CreateFetchRequest("LAST", start_time, end_time - 300);
            FetchData fd = fr.FetchData();
            for (int i = 0; i < fd.RowCount; i++)
            {
                FetchPoint fp = fd.GetRow(i);
                Console.WriteLine("{0}: {1}", fp.Time, fp.Values[0]);
            }

            rrd.Close();

            // Create new graph for the last 12 hours
            RrdGraphDef gdef = new RrdGraphDef(new DateTime(2013, 05, 31, 12, 00, 00), new DateTime(2013, 05, 31, 23, 59, 59));
            gdef.Title = "Battery voltage";
            gdef.VerticalLabel = "U (V)";
            gdef.SetValueAxis(0.2, 0.2);
            gdef.SetGridRange(1.000, 5.000, false);
            gdef.Datasource("vbat", "jonas.rrd", "battery", "LAST");
            gdef.Datasource("vbat_avg", "jonas.rrd", "battery", "AVERAGE");
            //gdef.Datasource("vbat_trend", "vbat,900,TREND");
            gdef.Line("vbat", System.Drawing.Color.Green, "Battery voltage", 1);
            gdef.Line("vbat_avg", System.Drawing.Color.Red, "Average over 15 min", 1);
            //gdef.Line("vbat_trend", System.Drawing.Color.Orange, "Trend over 15 min", 1);
            gdef.Gprint("vbat", "LAST", "Current: @2V@r");
            gdef.Gprint("vbat", "AVERAGE", "Average: @2V@r");
            gdef.Gprint("vbat", "MAX", "Max: @2V@r");
            gdef.Gprint("vbat", "MIN", "Min: @2V@r");
            RrdGraph graph = new RrdGraph(gdef);
            graph.SaveAsPNG("battery_2h.png", 576, 250);
        }

        public void JonasTest2()
        {
            UInt32 start_time = 1370000000; // 31th may 2013, 11:33:20
            UInt32 end_time = start_time + 300 * 300;
            RrdDef rrdDef = new RrdDef("jonas.rrd");
            rrdDef.StartTime = start_time;
            rrdDef.AddDatasource("battery", "GAUGE", 300, 1.0, 5.0); // Five minutes
            rrdDef.AddArchive("LAST", 0.5, 1, 576);         // 
            rrdDef.AddArchive("MIN", 0.5, 1, 576);          // 24 hours average, 2.5 minutes resolution
            rrdDef.AddArchive("MAX", 0.5, 1, 576);          // 24 hours average, 2.5 minutes resolution
            rrdDef.AddArchive("AVERAGE", 0.5, 6, 672);     // 7 days average, 15 min resolution
            rrdDef.AddArchive("MIN", 0.5, 6, 672);         // 7 days average, 15 min resolution
            rrdDef.AddArchive("MAX", 0.5, 6, 672);         // 7 days average, 15 min resolution
            rrdDef.AddArchive("AVERAGE", 0.5, 24, 87600);   // 10 year average, 1 hour resolution
            rrdDef.AddArchive("MIN", 0.5, 24, 87600);       // 10 year average, 1 hour resolution
            rrdDef.AddArchive("MAX", 0.5, 24, 87600);       // 10 year average, 1 hour resolution
            RrdDb db = new RrdDb(rrdDef);
            db.Close();

            RrdDb rrd = new RrdDb("jonas.rrd");
            Random rand = new Random();
            //Sample s = rrd.CreateSample();
			// update
			for(long t = start_time + 300; t <= end_time; t += 300) 
			{
                double r = (double)rand.Next(1, 6);
                //s.SetAndUpdate(String.Format("{0}:{1}", t, r));
                Sample sample = rrd.CreateSample(t);
                sample.SetValue("battery", (double)r);
                sample.Update();
                Console.WriteLine(r);
			}

            FetchRequest fr = rrd.CreateFetchRequest("LAST", start_time, end_time - 300);
            FetchData fd = fr.FetchData();
            for (int i = 0; i < fd.RowCount; i++)
            {
                FetchPoint fp = fd.GetRow(i);
                Console.WriteLine("{0}: {1}", fp.Time, fp.Values[0]);
            }

            rrd.Close();

            // Create new graph for the last 12 hours
            RrdGraphDef gdef = new RrdGraphDef(new DateTime(2013, 05, 31, 12, 00, 00), new DateTime(2013, 05, 31, 18, 59, 59));
            gdef.Title = "Battery voltage";
            gdef.VerticalLabel = "U (V)";
            gdef.SetValueAxis(1.0, 1.0);
            gdef.SetGridRange(1.000, 5.000, true);
            gdef.Datasource("vbat", "jonas.rrd", "battery", "LAST");
            //gdef.Datasource("vbat_avg", "jonas.rrd", "battery", "AVERAGE");
            gdef.Datasource("vbat_trend", "vbat,900,TREND");
            gdef.Line("vbat", System.Drawing.Color.Green, "Battery voltage", 1);
            //gdef.Line("vbat_avg", System.Drawing.Color.Red, "Average over 15 min", 1);
            gdef.Line("vbat_trend", System.Drawing.Color.Red, "Trend over 15 min", 1);
            gdef.Gprint("vbat", "LAST", "Current: @2V@r");
            gdef.Gprint("vbat", "AVERAGE", "Average: @2V@r");
            gdef.Gprint("vbat", "MAX", "Max: @2V@r");
            gdef.Gprint("vbat", "MIN", "Min: @2V@r");
            RrdGraph graph = new RrdGraph(gdef);
            graph.SaveAsPNG("battery2_2h.png", 700, 300);
        }

		public void Test1()
		{

			
			Console.WriteLine("Beginning Test1...");
			RrdDef rrdDef = new RrdDef("test1.rrd");
			rrdDef.StartTime = 978300900L;
			rrdDef.AddDatasource("a", "COUNTER", 600, Double.NaN, Double.NaN);
			rrdDef.AddDatasource("b", "GAUGE", 600, Double.NaN, Double.NaN);
			rrdDef.AddDatasource("c", "DERIVE", 600, Double.NaN, Double.NaN);
			rrdDef.AddDatasource("d", "ABSOLUTE", 600, Double.NaN, Double.NaN);
			rrdDef.AddArchive("AVERAGE", 0.5, 1, 10);		
			RrdDb rrdDb = new RrdDb(rrdDef);
			Sample sample = rrdDb.CreateSample();
			sample.SetAndUpdate("978301200:300:1:600:300");
			sample.SetAndUpdate("978301500:600:3:1200:600");
			sample.SetAndUpdate("978301800:900:5:1800:900");
			sample.SetAndUpdate("978302100:1200:3:2400:1200");
			sample.SetAndUpdate("978302400:1500:1:2400:1500");
			sample.SetAndUpdate("978302700:1800:2:1800:1800");
			sample.SetAndUpdate("978303000:2100:4:0:2100");
			sample.SetAndUpdate("978303300:2400:6:600:2400");
			sample.SetAndUpdate("978303600:2700:4:600:2700");
			sample.SetAndUpdate("978303900:3000:2:1200:3000");
			rrdDb.Close();

			RrdGraphDef graphDef = new RrdGraphDef();
			graphDef.SetTimePeriod(978300600L, 978304200L);
			graphDef.Title = "This is a cool title";
			graphDef.VerticalLabel = "Vertical Label";
			graphDef.Datasource("linea", "test1.rrd", "a", "AVERAGE");
			graphDef.Datasource("lineb", "test1.rrd", "b", "AVERAGE");
			graphDef.Datasource("linec", "test1.rrd", "c", "AVERAGE");
			graphDef.Datasource("lined", "test1.rrd", "d", "AVERAGE");
			graphDef.Line("linea", Color.Red,  "Line A", 3);
			graphDef.Line("lineb", Color.Lime,  "Line B", 3);
			graphDef.Line("linec", Color.Blue,  "Line C", 3);
			graphDef.Line("lined", Color.Cyan,  "Line D", 3);
			RrdGraph graph = new RrdGraph(graphDef);
			graph.SaveAsPNG("test1.png", 400, 400);
			Console.WriteLine("Test1 Complete.");

		}


		public void Test2()
		{

			
			Console.WriteLine("Beginning Test2...");
			RrdDef rrdDef = new RrdDef("test2.rrd");
			rrdDef.StartTime = 920804400L;
			rrdDef.AddDatasource("speed", "COUNTER", 600, Double.NaN, Double.NaN);
			rrdDef.AddArchive("AVERAGE", 0.5, 1, 24);
			rrdDef.AddArchive("AVERAGE", 0.5, 6, 10);
			RrdDb rrdDb = new RrdDb(rrdDef);
			rrdDb.Close();
		
			rrdDb = new RrdDb("test2.rrd");
			Sample sample = rrdDb.CreateSample();
			sample.SetAndUpdate("920804700:12345");
			sample.SetAndUpdate("920805000:12357");
			sample.SetAndUpdate("920805300:12363");
			sample.SetAndUpdate("920805600:12363");
			sample.SetAndUpdate("920805900:12363");
			sample.SetAndUpdate("920806200:12373");
			sample.SetAndUpdate("920806500:12383");
			sample.SetAndUpdate("920806800:12393");
			sample.SetAndUpdate("920807100:12399");
			sample.SetAndUpdate("920807400:12405");
			sample.SetAndUpdate("920807700:12411");
			sample.SetAndUpdate("920808000:12415");
			sample.SetAndUpdate("920808300:12420");
			sample.SetAndUpdate("920808600:12422");
			sample.SetAndUpdate("920808900:12423");
			rrdDb.Close();

			RrdGraphDef graphDef = new RrdGraphDef();
			graphDef.SetTimePeriod(920804400L, 920808000L);
			graphDef.Datasource("myspeed", "test2.rrd", "speed", "AVERAGE");
			graphDef.Datasource("realspeed", "myspeed,1000,*");
			graphDef.Line("realspeed", Color.Red, "speed", 2);
			RrdGraph graph = new RrdGraph(graphDef);
			graph.SaveAsPNG("test2a.png", 400, 100);
			
			graphDef = new RrdGraphDef();
			graphDef.SetTimePeriod(920804400L, 920808000L);
			graphDef.VerticalLabel = "km/h";
			graphDef.Overlay = "Sunset.jpg";
			graphDef.Datasource("myspeed", "test2.rrd", "speed", "AVERAGE");
			graphDef.Datasource("kmh", "myspeed,3600,*");
			graphDef.Datasource("fast", "kmh,100,GT,kmh,0,IF");
			graphDef.Datasource("good", "kmh,100,GT,0,kmh,IF");
			graphDef.Area("good", Color.Lime, "Good speed");
			graphDef.Area("fast", Color.Red, "Too fast");
			graphDef.Hrule(100, Color.Blue, "Maximum allowed");
			graph = new RrdGraph(graphDef);
			graph.SaveAsPNG("test2b.png", 400, 100);
			Console.WriteLine("Test2 Complete.");

		}


		public void Test3()
		{

			long start = Util.Time, end = start + 300 * 300;
			string rrdFile = "test3.rrd";
			string pngFile = "test3.png";

			Console.WriteLine("Beginning Test3...");
			RrdDef rrdDef = new RrdDef(rrdFile, start - 1, 300);
			rrdDef.AddDatasource("a", "GAUGE", 600, Double.NaN, Double.NaN);
			rrdDef.AddArchive("AVERAGE", 0.5, 1, 300);
			rrdDef.AddArchive("MIN", 0.5, 12, 300);
			rrdDef.AddArchive("MAX", 0.5, 12, 300);
			RrdDb rrdDb = new RrdDb(rrdDef);
			// update
			for(long t = start; t <  end; t += 300) 
			{
				Sample sample = rrdDb.CreateSample(t);
				sample.SetValue("a", Math.Sin(t / 3000.0) * 50 + 50);
				sample.Update();
			}
			rrdDb.Close();
			// graph
			RrdGraphDef gDef = new RrdGraphDef();
			gDef.SetTimePeriod(start, start + 86400);
			gDef.Title = "RRDTool's MINMAX.pl demo";
			gDef.TimeAxisLabel = "time";
			gDef.Datasource("a", rrdFile, "a", "AVERAGE");
			gDef.Datasource("b", rrdFile, "a", "MIN");
			gDef.Datasource("c", rrdFile, "a", "MAX");
			gDef.Area("a", Color.LightBlue, "real");
			gDef.Line("b", Color.Blue, "min");
			gDef.Line("c", Color.Lime, "max");
			RrdGraph graph = new RrdGraph(gDef);
			graph.SaveAsPNG(pngFile, 450, 0);
			Console.WriteLine("Test3 Complete.");

		}

		public void Test4()
		{

			
			Console.WriteLine("Beginning Test4...");
			// Time Values
			DateTime[] timestamps = {
										new DateTime(2004, 2, 1, 0, 0, 0),
										new DateTime(2004, 2, 1, 2, 0, 0),
										new DateTime(2004, 2, 1, 7, 0, 0),
										new DateTime(2004, 2, 1, 14, 0, 0),
										new DateTime(2004, 2, 1, 17, 0, 0),
										new DateTime(2004, 2, 1, 19, 0, 0),
										new DateTime(2004, 2, 1, 23, 0, 0),
										new DateTime(2004, 2, 2, 0, 0, 0)
									};

			// Corresponding Data Values
			double[] values = { 100, 250, 230, 370, 350, 300, 340, 350 };

			LinearInterpolator linear = new LinearInterpolator(timestamps, values); 
			CubicSplineInterpolator spline = new CubicSplineInterpolator(timestamps, values);

			// graph range
			RrdGraphDef gDef = new RrdGraphDef(timestamps[0], timestamps[timestamps.Length - 1]);

			// graph title, time and value axis labels
			gDef.Title = "Plottable demonstration";
			gDef.TimeAxisLabel = "time";
			gDef.VerticalLabel = "water level [inches]";

			// interpolated datasources
			gDef.Datasource("linear", linear);
			gDef.Datasource("spline", spline);

			// splined plot will be an orange filled area
			gDef.Area("spline", Color.Orange, "Spline interpolation");

			// simply interpolated plot will be a red line
			gDef.Line("linear", Color.Red, "Linear inteprolation@r");

			// print average values for both interpolation methods
			gDef.Gprint("spline", "AVERAGE", "Average spline value: @2 inches@r");
			gDef.Gprint("linear", "AVERAGE", "Average linear value: @0 inches@r");

			// create the graph...
			RrdGraph graph = new RrdGraph(gDef);

			// ...and save it somewhere
			string filename = "test4.png";
			graph.SaveAsPNG(filename, 300, 100);
			Console.WriteLine("Test4 Complete.");

		}

		public void StressTest()
		{
			string RRD_PATH = "stress.rrd";
			long RRD_START = 946710000L;
			long RRD_STEP = 30;
			string RRD_DATASOURCE_NAME = "T";
			int RRD_DATASOURCE_COUNT = 6;
			long TIME_START = 1060142010L;
			long TIME_END = 1080013472L;
			string PNG_PATH = "stress.png";
			int PNG_WIDTH = 400;
			int PNG_HEIGHT = 250;
				
			//Stress test
			DateTime testBegin = DateTime.UtcNow;
			printLapTime("Beginning Stress Test at " + testBegin.ToString());
			// create RRD database
			printLapTime("Creating RRD definition");
			RrdDef def = new RrdDef(RRD_PATH);
			def.StartTime = RRD_START;
			def.Step = RRD_STEP;
			for(int i = 0; i < RRD_DATASOURCE_COUNT; i++) 
			{
				def.AddDatasource(RRD_DATASOURCE_NAME + i, "GAUGE", 90, -60, 85);
			}
			def.AddArchive("LAST", 0.5, 1, 5760);
			def.AddArchive("MIN", 0.5, 1, 5760);
			def.AddArchive("MAX", 0.5, 1, 5760);
			def.AddArchive("AVERAGE", 0.5, 5, 13824);
			def.AddArchive("MIN", 0.5, 5, 13824);
			def.AddArchive("MAX", 0.5, 5, 13824);
			def.AddArchive("AVERAGE", 0.5, 60, 16704);
			def.AddArchive("MIN", 0.5, 60, 16704);
			def.AddArchive("MAX", 0.5, 60, 16704);
			def.AddArchive("AVERAGE", 0.5, 1440, 50000);
			def.AddArchive("MIN", 0.5, 1440, 50000);
			def.AddArchive("MAX", 0.5, 1440, 50000);
			printLapTime("Definition created, creating RRD file");

			RrdDb rrd = new RrdDb(def);
			printLapTime("RRD file created: " + RRD_PATH);
			
			FileStream fs = File.OpenRead("stress-test.txt");
			byte[] memBuffer = new byte[fs.Length]; 
			fs.Read(memBuffer, 0, memBuffer.Length); 
			fs.Close(); 
			StreamReader memoryReader = new StreamReader(new MemoryStream(memBuffer)); 
			

			//StreamReader diskReader = File.OpenText("stress-test.txt");
			//string allLines = diskReader.ReadToEnd();
			//diskReader.Close();	
			//StringReader memoryReader = new StringReader(allLines);


			//StreamReader memoryReader = File.OpenText("stress-test.txt");
			
			
			printLapTime("Input data loaded into memory, processing data");
			
			int count = 0;
			DateTime updateStart = DateTime.UtcNow;
			string line;

			while ((line = memoryReader.ReadLine()) != null)
			{
				Sample sample = rrd.CreateSample();
				try 
				{
					sample.SetAndUpdate(line);
					if(++count % 1000 == 0) 
					{
						DateTime now = DateTime.UtcNow;
						long speed = (long)(count * 1000.0 / (Util.TicksToMillis(now.Ticks) - Util.TicksToMillis(updateStart.Ticks)));
						printLapTime(count + " samples stored, " + speed + " updates/sec");
					}
				}
				catch(RrdException) 
				{
					printLapTime("RRD ERROR: " + line);
				}
			}
			memoryReader.Close();
			rrd.Close();
			
			printLapTime("FINISHED: " + count + " samples stored");
		
		
			// GRAPH
			printLapTime("Creating composite graph definition");
			RrdGraphDef gdef = new RrdGraphDef(TIME_START, TIME_END);
			gdef.Title = "Temperatures";
			gdef.VerticalLabel = "Fahrenheit";
			Color [] colors =  {Color.Red, Color.Lime, Color.Blue, Color.Magenta,Color.Cyan, Color.Orange };
			// datasources
			for(int i = 0; i < RRD_DATASOURCE_COUNT; i++) 
			{
				string name = RRD_DATASOURCE_NAME + i;
				gdef.Datasource(name, RRD_PATH, name, "AVERAGE");
			}
			// lines
			for(int i = 0; i < RRD_DATASOURCE_COUNT; i++) 
			{
				string name = RRD_DATASOURCE_NAME + i;
				gdef.Line(name, colors[i], name);
			}
			gdef.Comment("@c");
			gdef.Comment("\nOriginal data provided by diy-zoning.sf.net@c");
			printLapTime("Graph definition created");
			RrdGraph g = new RrdGraph(gdef);
			g.SaveAsPNG(PNG_PATH, PNG_WIDTH, PNG_HEIGHT);
			printLapTime("Graph saved: " + PNG_PATH);
			DateTime testEnd = DateTime.UtcNow;
			printLapTime("Finished at " + testEnd.ToString());
			Console.WriteLine("Total Time: " + testEnd.Subtract(testBegin).ToString());
		}

		
		public void printLapTime(string message) 
		{
			Console.WriteLine(message + " " + Util.LapTime);
		}
	}
}