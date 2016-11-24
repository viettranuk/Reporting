# Reporting

This is a small console application that uses the UK government river level API 

http://environment.data.gov.uk/flood-monitoring/doc/reference

to perform the following tasks

- Retrieve a list of all stations on a particular river (name is configurable)
- For each station, display the station name, and using data from the last 7 days (configurable) to date to display

> Its minimum level and when it first occurred

> Its maximum level and when it last occurred

> The average (mean) river level

Parameter is also configurable with values: level / flow / temperature

The application adhered to best practices where possible (i.e. SOLID principles, etc.)

Technical notes
- Visual Studio 2013
- .NET 4.6.1
- Dependency injection with Autofac
- nUnit/Moq for unit-tests

Tags: #retailReport, #demo, #solid, #autofac, #dependencyInjection, #cSharp, #software, #consoleApp
