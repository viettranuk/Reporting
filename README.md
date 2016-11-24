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

The application adhered to best practices where possible (i.e. SOLID principles, etc.) but as usual, there is always room for improvement! All feedback are welcomed and very much appreciated!

Technical notes
- Visual Studio 2013
- .NET 4.6.1
- Dependency injection with Autofac
- nUnit/Moq for unit-tests
- In-memory caching is used to store queried data (MemoryCache/ObjectCache)

Tags: #solid, #autofac, #dependencyInjection, #cSharp, #software, #consoleApp
