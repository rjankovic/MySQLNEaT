MySQLNEaT
=========

What it is
==========

A database layer for MySQL based on MySQL Connector/Net (http://dev.mysql.com/doc/refman/5.6/en/connector-net.html), 
inspired by dibi (https://github.com/dg/dibi), meant to make querying MySQL databases from C# code more elegant, 
easier and safer while restricting the abstraction to a level where you still clearly know what is going to 
be executed and can optimize your queries directly (this is somewhat more difficult with the Entity Framework).

How to get it
=============
Simply download the DLL from the repo and add a reference to it. If you want to look closer, go ahead for the
source codes. (To run those you will first have to download Connector/NET mentioned above, though.)

How to use it
=============

Create a database driver and let it connect to the database:

<code>DBDriverMySql driver = new DBDriverMySql(connectionString);</code>

Create a factory for query parts:

<code>DbDeployableMySql dbe = new DbDeployableMySql();</code>

Now you can do something like this:

<code>driver.Query("INSERT INTO `users` (`name`, `email`, `salary`) VALUES('Foo' 'Bar', 1234)");</code>

or achieve the same by this:

<pre>
Dictionary<string, object> values = new Dictionary<string, object>{
    {"name", "Foo"},
    {"email", "Bar"},
    {"salary", 1234}
};
driver.Query("INSERT INTO", dbe.Table("users"), dbe.InsVals(values);
</pre>

or try this

<code>var x = driver.FetchSingle("SELECT COUNT(*) FROM ", dbe.Table("users"));</code>

or this

<pre>
DataTable table = driver.FetchAll("SELECT", dbe.Col("name"), 
                                    "FROM", dbe.Table("users"), 
                                    "WHERE", dbe.Col("salary"), ">", 5000);
</pre>

For more options please take a look at Interfaces.cs

Why use these query parts
=========================
- queries will be shorter, with less quoting
- SQL injection protection - all the arguments of driver methods, except from strings, 
  will be quoted or passed as query parameters
- natural C# objects will be transalted into SQL code 
  depending on the DbDeployableMySql method you use
- you maintain full control - there is no guessing as for the meaning 
  of an argument you pass to the driver - look into DbDeployable.cs; 
  if you want an specific query, you can still use plain string and optimize ahead. 

Problems?
=========

In case of any troubles or ideas for the project, feel free to contact me at jankovic.rj@gmail.com

A mere beginning
================

Many imporovements and extensions can be made, such as

- batch queries
- transaction support
- MS SQL Server support
- whatever sensible you come up with

So please let me know if any of these would help you.
