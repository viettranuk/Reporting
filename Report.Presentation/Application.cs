using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Report.Models;
using Report.Services;

namespace Report.Presentation
{
    public class Application : IApplication
    {
        private readonly ILogging _logger;
        private readonly IFloodMonitoringServices _monitor;
        private readonly IConfigurationServices _config;

        public Application(ILogging logger, IFloodMonitoringServices monitor, IConfigurationServices config)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this._logger = logger;

            if (monitor == null)
            {
                throw new ArgumentNullException("monitor");
            }

            this._monitor = monitor;

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this._config = config;
        }

        public void Run(IEnumerable<string> args)
        {
            if ((args == null) || (args.Count() == 0))
            {
                DisplayHelpMenu();

                var commandLine = "";

                while (true)
                {
                    commandLine = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(commandLine))
                    {
                        break;
                    }
                }

                var commandObj = ParseCommandLine(commandLine);

                if (null == commandObj)
                {
                    Console.WriteLine("\nERROR OCCURRED!!!\n");
                    Run(null);
                }
                else
                {
                    DisplayData(GetStationsReportViewModel(commandObj));
                }
            }
            else
            {
                var commandObj = ParseArguments(args.ToList());

                DisplayData(GetStationsReportViewModel(commandObj));
            }
        }

        private CommandLineViewModel ParseCommandLine(string commandLine)
        {
            try
            {
                var splitter = new Regex("(?:^| )(\"(?:[^\"]+|\"\")*\"|[^ ]*)", RegexOptions.Compiled);
                var argsList = new List<string>();

                foreach (Match match in splitter.Matches(commandLine))
                {
                    if (!string.IsNullOrWhiteSpace(match.Value))
                    {
                        argsList.Add(match.Value.Trim());
                    }
                }

                return ParseArguments(argsList);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }

        private CommandLineViewModel ParseArguments(List<string> argsList)
        {
            try
            {
                var commandObj = new CommandLineViewModel
                {
                    RiverName = argsList[0].Trim(new char[] { '"', ' ', '/', '\\', '?' })
                };

                if (argsList.Count > 1)
                {
                    for (int i = 1; i < argsList.Count; i++)
                    {
                        var arg = argsList[i].Split(':');

                        switch (arg[0].ToLower().Trim())
                        {
                            case "days":
                                commandObj.Days = Convert.ToInt32(arg[1]);
                                break;

                            case "parameter":
                                commandObj.Parameter = arg[1].ToLower();
                                break;

                            case "limit":
                                commandObj.Limit = Convert.ToInt32(arg[1]);
                                break;
                        }
                    }
                }

                return commandObj;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }

        private StationsLevelReportViewModel GetStationsReportViewModel(CommandLineViewModel commandLine)
        {
            try
            {
                var reportView = new StationsLevelReportViewModel
                {
                    ReportType = string.IsNullOrEmpty(commandLine.Parameter) ? 
                        _config.GetDefaultParameter() : commandLine.Parameter.ToLower()
                };

                var stations = _monitor.GetStationReferencesAndNamesForRiver(commandLine.RiverName).Result;

                if (stations != null && stations.Any())
                {
                    reportView.RiverBasedStationNames = string.Join(", ", stations.Values);

                    var stationReferences = stations.Keys.ToList();

                    foreach (var stationRef in stationReferences)
                    {
                        reportView.StationBasedRiverLevels.Add(
                            _monitor.GetRiverWaterLevelDataAtStation(stationRef, commandLine).Result);
                    }
                }

                return reportView;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }

        private void DisplayData(StationsLevelReportViewModel reportView)
        {
            try
            {                
                if (reportView == null)
                {
                    Console.WriteLine("\nSorry, there is no data to display");
                }
                else
                {
                    Console.WriteLine("\nSTATION NAMES ===================================");
                    Console.WriteLine();
                    Console.WriteLine(string.IsNullOrEmpty(reportView.RiverBasedStationNames) ? "Sorry, there is no data to display" : reportView.RiverBasedStationNames);
                    Console.WriteLine();
                    Console.WriteLine("DATA FOR EACH STATION ===========================");
                    Console.WriteLine();

                    if (reportView.StationBasedRiverLevels == null || !reportView.StationBasedRiverLevels.Any())
                    {
                        Console.WriteLine("Sorry, there is no data to display\n");
                    }
                    else
                    {
                        foreach (var station in reportView.StationBasedRiverLevels)
                        {
                            if (station != null)
                            {
                                Console.WriteLine("Station name: {0}", station.StationName.ToUpper());

                                Console.WriteLine("Minimum {0}: {1} occurred on {2} at {3}", new string[] { 
                                    reportView.ReportType,
                                    station.MinValue.ToString(), 
                                    station.MinValueDate.ToLongDateString(), 
                                    station.MinValueDate.ToShortTimeString() 
                                });

                                Console.WriteLine("Maximum {0}: {1} occurred on {2} at {3}", new string[] { 
                                    reportView.ReportType,
                                    station.MaxValue.ToString(), 
                                    station.MaxValueDate.ToLongDateString(), 
                                    station.MaxValueDate.ToShortTimeString() 
                                });

                                Console.WriteLine("The average (mean) river {0}: {1}", new string[] { 
                                    reportView.ReportType, 
                                    station.AverageValue.ToString("0.###") 
                                });
                            }
                            else
                            {
                                Console.WriteLine("Sorry, there is no data to display");
                            }

                            Console.WriteLine("\n===============================================\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                Console.WriteLine("\nERROR OCCURRED!!!\n");                
            }
            finally
            {
                Run(null);
            }
        }

        private void DisplayHelpMenu()
        {
            Console.WriteLine("Please enter command or press (Ctrl + C) to exit");
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine("Command format:");
            Console.WriteLine("------ <river-name> [days:filter] [parameter:filter] [limit:filter]");
            Console.WriteLine("Where: ");
            Console.WriteLine("------ [days] number of days from today (max: 7, default: 0)");
            Console.WriteLine("------ [parameter] value can be either 'flow', 'level', or 'temperature'");
            Console.WriteLine("------ [limit] value can be any integer number (max: 10000, default: 500)");
            Console.WriteLine("Examples: ");
            Console.WriteLine("------ Stort days:5 parameter:level limit:10000");
            Console.WriteLine("------ \"Thames River\" days:7 parameter:level limit:700");
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}
