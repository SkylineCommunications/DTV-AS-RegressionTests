namespace RT_DTV_Evertz_GeneralPlatform_1.TestCases
{
	using System;
	using System.Collections.Generic;
	using Library.Tests.TestCases;
	using QAPortalAPI.Models.ReportingModels;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	public class BPXRFSatelliteSwitching : ITestCase
	{
		private readonly int modeWritePid = 7353;
		private readonly int activeChannelPid = 7312;

		public BPXRFSatelliteSwitching(string name, string elementName)
		{
			Name = name;
			ElementName = elementName;
		}

		public enum BpxrfModes
		{
			External = 1,
			AutoSwitchBack = 2,
			Auto = 3,
			ChannelA = 4,
			ChannelB = 5,
		}

		public enum BpxrfActiveChannels
		{
			ChannelA = 1,
			ChannelB = 2,
		}

		public string Name { get; set; }

		public string Index { get; set; }

		public string ElementName { get; set; }

		public PerformanceTestCaseReport PerformanceTestCaseReport { get; }

		public TestCaseReport TestCaseReport { get; private set; }

		private List<ColumnFilter> UpdatedStreamFilter { get; set; }

		public void Execute(IEngine engine)
		{
			try
			{
				if (RunTestCaseSatelliteSwitching(engine))
				{
					TestCaseReport.GetSuccessTestCase(Name);
				}
				else
				{
					var failDescription = $"Could not validate new crosspoint data with outputs table.";
					TestCaseReport.GetFailTestCase(Name, failDescription);
				}

			}
			catch (Exception e)
			{
				TestCaseReport = TestCaseReport.GetFailTestCase($"Foreign Keys validation for {Name}", $"Exception: {e}");
			}
		}

		private bool RunTestCaseSatelliteSwitching(IEngine engine)
		{
			var valueToSet = 0;
			var element = engine.FindElement(ElementName);
			var expectedValue = 0;
			if (element != null)
			{
				var activeChannel = Convert.ToInt32(element.GetParameter(activeChannelPid));
				if (activeChannel == (int)BpxrfActiveChannels.ChannelA)
				{
					valueToSet = (int)BpxrfModes.ChannelB;
					expectedValue = (int)BpxrfActiveChannels.ChannelB;
				}
				else
				{
					valueToSet = (int)BpxrfModes.ChannelA;
					expectedValue = (int)BpxrfActiveChannels.ChannelA;
				}

				if (valueToSet != 0)
				{
					element.SetParameter(modeWritePid, valueToSet);
					System.Threading.Thread.Sleep(3000);

					activeChannel = Convert.ToInt32(element.GetParameter(activeChannelPid));

					if (activeChannel == expectedValue)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
